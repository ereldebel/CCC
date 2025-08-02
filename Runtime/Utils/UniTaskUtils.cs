using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace CCC.Runtime.Utils
{
    public static class UniTaskUtils
    {
        private const int MillisecondsPerSecond = 1000;
        
        /// <summary>
        /// Delays execution for the specified number of seconds.
        /// </summary>
        public static UniTask Delay(float secondsDelay, bool ignoreTimeScale = false, PlayerLoopTiming delayTiming = PlayerLoopTiming.Update, CancellationToken cancellationToken = default(CancellationToken), bool cancelImmediately = false)
        {
            return UniTask.Delay(TimeSpan.FromSeconds(secondsDelay), ignoreTimeScale, delayTiming, cancellationToken, cancelImmediately);
        }

        /// <summary>
        /// An async method that passes the interpolated values [0,1] to the interpolator through the given duration.
        /// Uses unscaled time.
        /// </summary>
        public static UniTask InterpolateUnscaledTime(Action<float> interpolator, float duration, CancellationToken cancellationToken = default)
        {
            return InterpolateInternal(t => { interpolator(t); return true; }, duration, () => Time.unscaledTime, cancellationToken);
        }
        
        /// <summary>
        /// An async method that passes the interpolated values [0,1] to the interpolator through the given duration
        /// or until the interpolator returns false. Uses unscaled time.
        /// </summary>
        public static UniTask InterpolateUnscaledTime(Func<float, bool> interpolator, float duration, CancellationToken cancellationToken = default)
        {
            return InterpolateInternal(interpolator, duration, () => Time.unscaledTime, cancellationToken);
        }
        
        /// <summary>
        /// An async method that passes the interpolated values [0,1] to the interpolator through the given duration.
        /// </summary>
        public static UniTask Interpolate(Action<float> interpolator, float duration, CancellationToken cancellationToken = default)
        {
            return InterpolateInternal(t => { interpolator(t); return true; }, duration, () => Time.time, cancellationToken);
        }
        
        /// <summary>
        /// An async method that passes the interpolated values [0,1] to the interpolator through the given duration
        /// or until the interpolator returns false.
        /// </summary>
        public static UniTask Interpolate(Func<float, bool> interpolator, float duration, CancellationToken cancellationToken = default)
        {
            return InterpolateInternal(interpolator, duration, () => Time.time, cancellationToken);
        }
        
        /// <summary>
        /// Internal implementation for interpolation methods.
        /// </summary>
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
        /// </summary>
        /// <param name="endAction">Optional: an action to perform after the predicate evaluates to true</param>
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
