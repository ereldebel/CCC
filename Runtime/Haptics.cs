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
	/// <summary>
	/// Manages haptic feedback for gamepad controllers, providing vibration patterns and intensity control.
	/// Supports multiple concurrent haptic effects with automatic frequency management and pause/resume functionality.
	/// </summary>
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

		/// <summary>
		/// Gets or sets the intensity multiplier for all haptic effects (0-2 range).
		/// </summary>
		/// <value>The intensity multiplier, clamped between 0 and 2.</value>
		public float Intensity
		{
			get => _intensity;
			set
			{
				_intensity = Mathf.Clamp(value, 0, 2);
				HapticsFrequencies = HapticsFrequencies;
			}
		}

		/// <summary>
		/// Gets or sets the current haptic frequencies and applies them to the gamepad motors.
		/// </summary>
		/// <value>The current haptic frequency settings.</value>
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

		/// <summary>
		/// Initializes a new Haptics instance with the specified InputManager.
		/// </summary>
		/// <param name="parentInputManager">The InputManager that owns this Haptics instance.</param>
		public Haptics(InputManager parentInputManager)
		{
			_inputManager = parentInputManager;
			_inputManager.DeviceTypeChanged += DeviceChanged;
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Resets all haptic effects, cancels running timers, and stops all motor vibration.
		/// </summary>
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

		/// <summary>
		/// Pauses or resumes all haptic effects based on the specified state.
		/// </summary>
		/// <param name="paused">True to pause all haptics, false to resume them.</param>
		public void Paused(bool paused)
		{
			if (paused)
				PauseAllHaptics();
			else
				ResumeAllHaptics();
		}

		/// <summary>
		/// Starts a haptic effect with the specified reference, optionally allowing it to work while paused.
		/// </summary>
		/// <param name="hapticsReference">The haptic effect reference containing frequency and duration data.</param>
		/// <param name="worksOnPause">Whether this haptic effect should continue while the system is paused.</param>
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

		/// <summary>
		/// Pauses all haptic effects and stops motor vibration.
		/// </summary>
		public void PauseAllHaptics()
		{
			_paused = true;
			HapticsFrequencies = Frequencies.Zero;
		}

		/// <summary>
		/// Resumes all haptic effects and restores the maximum frequency from running effects.
		/// </summary>
		public void ResumeAllHaptics()
		{
			_paused = false;
			SetToCurrentMaxFrequency();
		}

		/// <summary>
		/// Stops a specific haptic effect and removes it from the running effects.
		/// </summary>
		/// <param name="hapticsReference">The haptic effect reference to stop.</param>
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

		/// <summary>
		/// Disposes of the Haptics instance, cleaning up resources and stopping all motor vibration.
		/// </summary>
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

		/// <summary>
		/// Handles device type changes and updates haptic frequencies accordingly.
		/// </summary>
		/// <param name="deviceType">The new device type.</param>
		private void DeviceChanged(InputManager.DeviceType deviceType)
		{
			if (deviceType != InputManager.DeviceType.Keyboard)
				HapticsFrequencies = HapticsFrequencies;
		}

		/// <summary>
		/// Sets the haptic frequencies to the maximum values from all currently running effects.
		/// </summary>
		private void SetToCurrentMaxFrequency()
		{
			HapticsFrequencies = Frequencies.Max(_runningHaptics.Select(e => e.Key.Frequencies));
		}

		/// <summary>
		/// Stops a haptic effect after its specified duration timeout.
		/// </summary>
		/// <param name="hapticsReference">The haptic effect reference.</param>
		/// <param name="worksOnPause">Whether this effect works while paused.</param>
		/// <param name="cancellationToken">Token for cancelling the operation.</param>
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

		/// <summary>
		/// Represents a haptic feedback effect with frequency and duration settings.
		/// </summary>
		[Serializable]
		public class HapticsReference
		{
			/// <summary>
			/// Gets whether this haptic effect has a finite duration.
			/// </summary>
			/// <value>True if the effect has a finite duration, false if infinite.</value>
			public bool Timed => !float.IsInfinity(Duration);
			
			[field: SerializeField, Tooltip("The frequency settings for low and high frequency motors")]
			internal Frequencies Frequencies { get; private set; }

			[field: SerializeField, Tooltip("Duration of the haptic effect in seconds (use infinity for continuous)")]
			[field: Range(0, float.PositiveInfinity)]
			public float Duration { get; private set; }

			/// <summary>
			/// Initializes a new HapticsReference with separate high and low frequency values.
			/// </summary>
			/// <param name="high">High frequency motor value (0-1).</param>
			/// <param name="low">Low frequency motor value (0-1).</param>
			/// <param name="duration">Duration in seconds (use infinity for continuous).</param>
			public HapticsReference(float high, float low, float duration = float.PositiveInfinity)
			{
				Frequencies = new Frequencies(high, low);
				Duration = duration;
			}

			/// <summary>
			/// Initializes a new HapticsReference with the same value for both high and low frequency motors.
			/// </summary>
			/// <param name="singleValue">Value to use for both high and low frequency motors (0-1).</param>
			public HapticsReference(float singleValue) : this(singleValue, singleValue)
			{
			}
		}

		/// <summary>
		/// Represents frequency settings for the low and high frequency motors of a gamepad.
		/// </summary>
		[Serializable]
		internal struct Frequencies
		{
			/// <summary>
			/// Zero frequency setting (no vibration).
			/// </summary>
			public static Frequencies Zero = new(0, 0);

			[field: SerializeField, Tooltip("Low frequency motor value (0-1)")]
			public float Low { get; private set; }

			[field: SerializeField, Tooltip("High frequency motor value (0-1)")]
			public float High { get; private set; }

			/// <summary>
			/// Initializes a new Frequencies struct with the specified low and high frequency values.
			/// </summary>
			/// <param name="low">Low frequency motor value (0-1).</param>
			/// <param name="high">High frequency motor value (0-1).</param>
			public Frequencies(float low, float high)
			{
				Low = low;
				High = high;
			}

			/// <summary>
			/// Returns the maximum frequency values between two Frequencies structs.
			/// </summary>
			/// <param name="a">First frequency struct.</param>
			/// <param name="b">Second frequency struct.</param>
			/// <returns>A new Frequencies struct with the maximum values.</returns>
			public static Frequencies Max(Frequencies a, Frequencies b)
			{
				return new Frequencies(Mathf.Max(a.Low, b.Low), Mathf.Max(a.High, b.High));
			}

			/// <summary>
			/// Returns the maximum frequency values from an array of Frequencies structs.
			/// </summary>
			/// <param name="frequencies">Array of frequency structs.</param>
			/// <returns>A new Frequencies struct with the maximum values.</returns>
			public static Frequencies Max(params Frequencies[] frequencies) =>
				Max((IEnumerable<Frequencies>)frequencies);

			/// <summary>
			/// Returns the maximum frequency values from a collection of Frequencies structs.
			/// </summary>
			/// <param name="frequencies">Collection of frequency structs.</param>
			/// <returns>A new Frequencies struct with the maximum values.</returns>
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