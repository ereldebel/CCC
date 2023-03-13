using System;
using System.Collections;
using UnityEngine;

namespace CCC.Runtime
{
    public static class Coroutines
    {
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
        
        public static IEnumerator PerformWhile<T>(Func<T> action, Predicate<T> actionPredicate, Action endAction = null)
        {
            while (!actionPredicate.Invoke(action.Invoke()))
                yield return null;
            endAction?.Invoke();
        }
    }
}