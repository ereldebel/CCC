using System;
using System.Collections;
using UnityEngine;

namespace CCC.Runtime
{
    public static class Coroutines
    {
        /// <summary>
        /// A coroutine that passes the interpolated values [0,1] to the interpolator through the given duration.
        /// Uses unscaled time.
        /// </summary>
        public static IEnumerator InterpolateUnscaledTime(Action<float> interpolator, float duration)
        {
            float startTime = Time.unscaledTime;
            float t;
            while ((t = (Time.unscaledTime - startTime) / duration) < 1)
            {
                interpolator.Invoke(t);
                yield return null;
            }
            interpolator.Invoke(1);
        }
        
        /// <summary>
        /// A coroutine that passes the interpolated values [0,1] to the interpolator through the given duration
        /// or until the interpolator returns false. Uses unscaled time.
        /// </summary>
        public static IEnumerator InterpolateUnscaledTime(Func<float, bool> interpolator, float duration)
        {
            float startTime = Time.unscaledTime;
            float t;
            while ((t = (Time.unscaledTime - startTime) / duration) < 1)
            {
                if (!interpolator.Invoke(t))
                    yield break;
                yield return null;
            }
            interpolator.Invoke(1);
        }
        
        /// <summary>
        /// A coroutine that passes the interpolated values [0,1] to the interpolator through the given duration.
        /// </summary>
        public static IEnumerator Interpolate(Action<float> interpolator, float duration)
        {
            float startTime = Time.time;
            float t;
            while ((t = (Time.time - startTime) / duration) < 1)
            {
                interpolator.Invoke(t);
                yield return null;
            }
            interpolator.Invoke(1);
        }
        
        /// <summary>
        /// A coroutine that passes the interpolated values [0,1] to the interpolator through the given duration
        /// or until the interpolator returns false.
        /// </summary>
        public static IEnumerator Interpolate(Func<float, bool> interpolator, float duration)
        {
            float startTime = Time.time;
            float t;
            while ((t = (Time.time - startTime) / duration) < 1)
            {
                if (!interpolator.Invoke(t))
                    yield break;
                yield return null;
            }
            interpolator.Invoke(1);
        }
        
        /// <summary>
        /// A coroutine that performs the given action while the given predicate evaluates to false.
        /// </summary>
        /// <param name="endAction">Optional: an action to perform after the predicate evaluates to true</param>
        public static IEnumerator PerformWhile<T>(Func<T> action, Predicate<T> actionPredicate, Action endAction = null)
        {
            while (!actionPredicate.Invoke(action.Invoke()))
                yield return null;
            endAction?.Invoke();
        }
    }
}