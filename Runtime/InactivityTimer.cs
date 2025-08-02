using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using CCC.Runtime.Utils;

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
		private CancellationTokenSource _cancellationTokenSource;

		#endregion

		#region Properties

		public Action InactivityAction { get; set; }

		#endregion

		#region Event Functions

		private void OnEnable()
		{
			_lastUsedTime = Time.time;
			StopTimer();
			_cancellationTokenSource = new CancellationTokenSource();
			RunTimerAsync(_cancellationTokenSource.Token).Forget();
		}

		private void OnDisable()
		{
			StopTimer();
		}

		#endregion
		
		#region Public Methods

		public void Used()
		{
			_lastUsedTime = Time.time;
		}
		
		#endregion

		#region Private Methods

		private void StopTimer()
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
			_cancellationTokenSource = null;
		}

		private async UniTaskVoid RunTimerAsync(CancellationToken cancellationToken)
		{
			try
			{
				float timeLeft;
				while ((timeLeft = (_lastUsedTime + TimeBeforeAction) - Time.time) > 0)
				{
					await UniTaskUtils.Delay(timeLeft, ignoreTimeScale: true, cancellationToken: cancellationToken);
				}
				
				if (!cancellationToken.IsCancellationRequested)
				{
					InactivityAction?.Invoke();
					enabled = false;
				}
			}
			catch (OperationCanceledException) {}
		}

		#endregion
	}
}