using CCC.Runtime.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

namespace CCC.Runtime
{
	public class Haptics : IDisposable
	{
		#region Private Fields

		private bool _paused;
		private readonly InputManager _inputManager;
		private Frequencies _hapticsFrequencies;
		private float _intensity = 1;
		private CancellationTokenSource _mainCancellationToken;
		private readonly Dictionary<HapticsReference, CancellationTokenSource> _runningHaptics = new();

		#endregion

		#region Properties

		public float Intensity
		{
			get => _intensity;
			set
			{
				_intensity = Mathf.Clamp(value, 0, 2);
				HapticsFrequencies = HapticsFrequencies;
			}
		}

		private Frequencies HapticsFrequencies
		{
			get => _hapticsFrequencies;
			set
			{
				_hapticsFrequencies = value;
				Gamepad.current?.SetMotorSpeeds(value.Low * Intensity, value.High * Intensity);
			}
		}

		#endregion

		#region Constructors

		public Haptics(InputManager parentInputManager)
		{
			_inputManager = parentInputManager;
			_inputManager.DeviceTypeChanged += DeviceChanged;
		}

		#endregion

		#region Public Methods

		public void Reset()
		{
			_mainCancellationToken?.Cancel();
			_mainCancellationToken?.Dispose();

			foreach (var cts in _runningHaptics.Values)
			{
				cts?.Dispose();
			}

			_runningHaptics.Clear();
			HapticsFrequencies = Frequencies.Zero;
		}

		public void Paused(bool paused)
		{
			if (paused)
				PauseAllHaptics();
			else
				ResumeAllHaptics();
		}

		public void StartHaptics(HapticsReference hapticsReference, bool worksOnPause = false)
		{
			bool alreadyRunning = _runningHaptics.ContainsKey(hapticsReference);
			CancellationTokenSource mainCts = _mainCancellationToken ??= new();
			CancellationTokenSource timedCts = null;
			if (hapticsReference.Timed)
			{
				if (alreadyRunning && _runningHaptics[hapticsReference] != null)
				{
					_runningHaptics[hapticsReference].Cancel();
					_runningHaptics[hapticsReference].Dispose();
				}
				timedCts = CancellationTokenSource.CreateLinkedTokenSource(mainCts.Token);
				StopHapticsAfterTimeout(hapticsReference, worksOnPause, timedCts.Token).Forget();
			}
			else if (alreadyRunning)
			{
				return;
			}

			if (!_paused || worksOnPause)
				HapticsFrequencies = Frequencies.Max(HapticsFrequencies, hapticsReference.Frequencies);

			_runningHaptics[hapticsReference] = timedCts;
		}

		public void PauseAllHaptics()
		{
			_paused = true;
			HapticsFrequencies = Frequencies.Zero;
		}

		public void ResumeAllHaptics()
		{
			_paused = false;
			SetToCurrentMaxFrequency();
		}

		public void StopHaptics(HapticsReference hapticsReference)
		{
			if (_runningHaptics.TryGetValue(hapticsReference, out var cts))
			{
				cts?.Cancel();
				cts?.Dispose();
				_runningHaptics.Remove(hapticsReference);
			}

			if (_paused)
				HapticsFrequencies = Frequencies.Zero;
			else
				SetToCurrentMaxFrequency();
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose()
		{
			_inputManager.DeviceTypeChanged -= DeviceChanged;
			Reset();

			foreach (var gamepad in Gamepad.all)
			{
				gamepad.SetMotorSpeeds(0, 0);
			}
		}

		#endregion

		#region Private Methods

		private void DeviceChanged(InputManager.DeviceType deviceType)
		{
			if (deviceType != InputManager.DeviceType.Keyboard)
				HapticsFrequencies = HapticsFrequencies;
		}

		private void SetToCurrentMaxFrequency()
		{
			HapticsFrequencies = Frequencies.Max(_runningHaptics.Select(e => e.Key.Frequencies));
		}

		private async UniTask StopHapticsAfterTimeout(HapticsReference hapticsReference, bool worksOnPause, CancellationToken cancellationToken)
		{
			try
			{
				await UniTaskUtils.Delay(hapticsReference.Duration, ignoreTimeScale: worksOnPause, cancellationToken: cancellationToken);
			}
			catch (OperationCanceledException) { }
			finally
			{
				StopHaptics(hapticsReference);
			}
		}

		#endregion

		#region Types

		[Serializable]
		public class HapticsReference
		{
			public bool Timed => !float.IsInfinity(Duration);
			[field: SerializeField] internal Frequencies Frequencies { get; private set; }

			[field: SerializeField]
			[field: Range(0, float.PositiveInfinity)]
			public float Duration { get; private set; }

			public HapticsReference(float high, float low, float duration = float.PositiveInfinity)
			{
				Frequencies = new Frequencies(high, low);
				Duration = duration;
			}

			public HapticsReference(float singleValue) : this(singleValue, singleValue)
			{
			}
		}

		[Serializable]
		internal struct Frequencies
		{
			public static Frequencies Zero = new(0, 0);

			[field: SerializeField]
			public float Low { get; private set; }

			[field: SerializeField]
			public float High { get; private set; }

			public Frequencies(float low, float high)
			{
				Low = low;
				High = high;
			}

			public static Frequencies Max(Frequencies a, Frequencies b)
			{
				return new Frequencies(Mathf.Max(a.Low, b.Low), Mathf.Max(a.High, b.High));
			}

			public static Frequencies Max(params Frequencies[] frequencies) =>
				Max((IEnumerable<Frequencies>)frequencies);

			public static Frequencies Max(IEnumerable<Frequencies> frequencies)
			{
				var maxFreq = new Frequencies();
				foreach (var freq in frequencies)
				{
					if (freq.Low > maxFreq.Low)
						maxFreq.Low = freq.Low;
					if (freq.High > maxFreq.High)
						maxFreq.High = freq.High;
				}

				return maxFreq;
			}
		}

		#endregion
	}
}