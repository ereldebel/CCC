using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	/// <summary>
	/// Utility methods for UniTask operations, providing convenient wrappers for common async patterns
	/// including delays, interpolation, and conditional execution with cancellation support.
	/// </summary>
	public static class UniTaskUtils
	{
		/// <summary>
		/// Delays execution for the specified number of seconds using UniTask.
		/// Provides a more convenient interface than the base UniTask.Delay method.
		/// </summary>
		/// <param name="secondsDelay">The delay duration in seconds.</param>
		/// <param name="ignoreTimeScale">Whether to ignore Unity's time scale.</param>
		/// <param name="delayTiming">The timing for the delay operation.</param>
		/// <param name="cancellationToken">Token for cancelling the delay operation.</param>
		/// <param name="cancelImmediately">Whether to cancel immediately when the token is cancelled.</param>
		/// <returns>A UniTask representing the delay operation.</returns>
		public static UniTask Delay(float secondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
		{
			return UniTask.Delay(TimeSpan.FromSeconds(secondsDelay), ignoreTimeScale, delayTiming, cancellationToken, cancelImmediately);
		}

		/// <summary>
		/// An async method that passes interpolated values [0,1] to the interpolator through the given duration.
		/// Uses unscaled time, making it suitable for UI animations and effects that should not be affected by time scale.
		/// </summary>
		/// <param name="interpolator">Action that receives the interpolation progress (0-1).</param>
		/// <param name="duration">The duration of the interpolation in seconds.</param>
		/// <param name="cancellationToken">Token for cancelling the interpolation.</param>
		/// <returns>A UniTask representing the interpolation operation.</returns>
		public static UniTask InterpolateUnscaledTime(Action<float> interpolator, float duration, CancellationToken cancellationToken = default)
		{
			return InterpolateInternal(t => { interpolator(t); return true; }, duration, () => Time.unscaledTime, cancellationToken);
		}

		/// <summary>
		/// An async method that passes interpolated values [0,1] to the interpolator through the given duration
		/// or until the interpolator returns false. Uses unscaled time.
		/// </summary>
		/// <param name="interpolator">Function that receives the interpolation progress (0-1) and returns whether to continue.</param>
		/// <param name="duration">The duration of the interpolation in seconds.</param>
		/// <param name="cancellationToken">Token for cancelling the interpolation.</param>
		/// <returns>A UniTask representing the interpolation operation.</returns>
		public static UniTask InterpolateUnscaledTime(Func<float, bool> interpolator, float duration, CancellationToken cancellationToken = default)
		{
			return InterpolateInternal(interpolator, duration, () => Time.unscaledTime, cancellationToken);
		}

		/// <summary>
		/// An async method that passes interpolated values [0,1] to the interpolator through the given duration.
		/// Uses scaled time, making it suitable for gameplay animations that should be affected by time scale.
		/// </summary>
		/// <param name="interpolator">Action that receives the interpolation progress (0-1).</param>
		/// <param name="duration">The duration of the interpolation in seconds.</param>
		/// <param name="cancellationToken">Token for cancelling the interpolation.</param>
		/// <returns>A UniTask representing the interpolation operation.</returns>
		public static UniTask Interpolate(Action<float> interpolator, float duration, CancellationToken cancellationToken = default)
		{
			return InterpolateInternal(t => { interpolator(t); return true; }, duration, () => Time.time, cancellationToken);
		}

		/// <summary>
		/// An async method that passes interpolated values [0,1] to the interpolator through the given duration
		/// or until the interpolator returns false.
		/// </summary>
		/// <param name="interpolator">Function that receives the interpolation progress (0-1) and returns whether to continue.</param>
		/// <param name="duration">The duration of the interpolation in seconds.</param>
		/// <param name="cancellationToken">Token for cancelling the interpolation.</param>
		/// <returns>A UniTask representing the interpolation operation.</returns>
		public static UniTask Interpolate(Func<float, bool> interpolator, float duration, CancellationToken cancellationToken = default)
		{
			return InterpolateInternal(interpolator, duration, () => Time.time, cancellationToken);
		}

		/// <summary>
		/// Internal implementation for interpolation methods.
		/// Handles the core interpolation logic with configurable time source and cancellation support.
		/// </summary>
		/// <param name="interpolator">Function that receives the interpolation progress and returns whether to continue.</param>
		/// <param name="duration">The duration of the interpolation in seconds.</param>
		/// <param name="getTime">Function that returns the current time value.</param>
		/// <param name="cancellationToken">Token for cancelling the interpolation.</param>
		/// <returns>A UniTask representing the interpolation operation.</returns>
		private static async UniTask InterpolateInternal(Func<float, bool> interpolator, float duration, Func<float> getTime, CancellationToken cancellationToken)
		{
			float startTime = getTime();
			float t;

			while ((t = (getTime() - startTime) / duration) < 1)
			{
				cancellationToken.ThrowIfCancellationRequested();

				if (!interpolator.Invoke(t))
					return;

				await UniTask.Yield(cancellationToken);
			}

			cancellationToken.ThrowIfCancellationRequested();
			interpolator.Invoke(1f);
		}

		/// <summary>
		/// An async method that performs the given action while the given predicate evaluates to false.
		/// Continues execution until the predicate returns true, then optionally executes an end action.
		/// </summary>
		/// <typeparam name="T">The type of the value returned by the action.</typeparam>
		/// <param name="action">The action to perform repeatedly.</param>
		/// <param name="actionPredicate">The predicate that determines when to stop.</param>
		/// <param name="endAction">Optional: an action to perform after the predicate evaluates to true.</param>
		/// <param name="cancellationToken">Token for cancelling the operation.</param>
		/// <returns>A UniTask representing the operation.</returns>
		public static async UniTask PerformWhile<T>(Func<T> action, Predicate<T> actionPredicate, Action endAction = null, CancellationToken cancellationToken = default)
		{
			while (!actionPredicate.Invoke(action.Invoke()))
			{
				cancellationToken.ThrowIfCancellationRequested();
				await UniTask.Yield(cancellationToken);
			}
			endAction?.Invoke();
		}
	}
}
