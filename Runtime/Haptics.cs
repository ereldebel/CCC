using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

		private readonly Dictionary<HapticsReference, Coroutine> _runningHaptics = new();

		#endregion

		#region Properties

		private Frequencies HapticsFrequencies
		{
			get => _hapticsFrequencies;
			set
			{
				_hapticsFrequencies = value;
				Gamepad.current?.SetMotorSpeeds(value.Low, value.High);
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

		public void Paused(bool paused)
		{
			if (paused)
				PauseAllHaptics();
			else
				ResumeAllHaptics();
		}

		public void StartHaptics(HapticsReference hapticsReference)
		{
			bool alreadyRunning = _runningHaptics.ContainsKey(hapticsReference);
			Coroutine timedCoroutine = null;
			if (hapticsReference.Timed)
			{
				if (alreadyRunning && _runningHaptics[hapticsReference] != null)
					_inputManager.StopCoroutine(_runningHaptics[hapticsReference]);
				timedCoroutine = _inputManager.StartCoroutine(TimedHapticsStop(hapticsReference));
			}
			else if (alreadyRunning)
			{
				return;
			}

			if (!_paused)
				HapticsFrequencies = Frequencies.Max(HapticsFrequencies, hapticsReference.Frequencies);
			_runningHaptics[hapticsReference] = timedCoroutine;
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
			_runningHaptics.Remove(hapticsReference);
			SetToCurrentMaxFrequency();
		}

		#endregion

		#region IDisposable Implementation

		public void Dispose()
		{
			_inputManager.DeviceTypeChanged -= DeviceChanged;
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

		private IEnumerator TimedHapticsStop(HapticsReference hapticsReference)
		{
			yield return new WaitForSeconds(hapticsReference.Duration);
			StopHaptics(hapticsReference);
		}

		#endregion

		#region Types

		[Serializable]
		public class HapticsReference
		{
			public bool Timed => !float.IsInfinity(Duration);
			[field: SerializeField] internal Frequencies Frequencies { get; private set; }

			[field: SerializeField] [field: Range(0, float.PositiveInfinity)]
			public float Duration { get; private set; }

			public HapticsReference(float high, float low, float duration = float.NaN)
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