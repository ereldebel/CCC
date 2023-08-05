using System;
using System.Collections;
using UnityEngine;

namespace CCC.Runtime
{
	public class InactivityTimer : MonoBehaviour
	{
		#region Serialized Fields

		[Tooltip("Time before inactivity action in seconds")]
		[field: SerializeField]
		public float TimeBeforeAction { get; set; } = 60;

		#endregion

		#region Private Fields

		private float _lastUsedTime = 0;
		private Coroutine _coroutine;

		#endregion

		#region Properties

		public Action InactivityAction { get; set; }

		#endregion

		#region Event Functions

		private void OnEnable()
		{
			_lastUsedTime = Time.time;
			if (_coroutine != null)
				StopCoroutine(_coroutine);
			_coroutine = StartCoroutine(RunTimer());
		}

		#endregion
		
		#region Public Methods

		public void Used()
		{
			_lastUsedTime = Time.time;
		}
		
		#endregion

		#region Private Methods

		private IEnumerator RunTimer()
		{
			float timeLeft;
			while ((timeLeft = (_lastUsedTime + TimeBeforeAction) - Time.time) > 0)
				yield return new WaitForSecondsRealtime(timeLeft);
			InactivityAction();
			_coroutine = null;
			enabled = false;
		}

		#endregion
	}
}