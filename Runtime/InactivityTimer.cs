using System;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using CCC.Runtime.Utils;

namespace CCC.Runtime
{
	/// <summary>
	/// Monitors user activity and triggers an action after a specified period of inactivity.
	/// Uses UniTask for asynchronous operation and can be paused/resumed as needed.
	/// </summary>
	public class InactivityTimer : MonoBehaviour
	{
		#region Serialized Fields

		[Tooltip("Time in seconds before the inactivity action is triggered")]
		[field: SerializeField]
		public uint TimeBeforeAction { get; set; } = 60;

		#endregion

		#region Private Fields

		private float _lastUsedTime = 0;
		private CancellationTokenSource _cancellationTokenSource;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the action to be executed when the inactivity timer expires.
		/// </summary>
		/// <value>The action to perform after the specified inactivity period.</value>
		public Action InactivityAction { get; private set; }

		#endregion

		#region Event Functions

		/// <summary>
		/// Initializes the timer when the component is enabled, starting the inactivity monitoring.
		/// </summary>
		private void OnEnable()
		{
			_lastUsedTime = Time.time;
			StopTimer();
			_cancellationTokenSource = new CancellationTokenSource();
			RunTimerAsync(_cancellationTokenSource.Token).Forget();
		}

		/// <summary>
		/// Stops the timer when the component is disabled to prevent memory leaks.
		/// </summary>
		private void OnDisable()
		{
			StopTimer();
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Resets the inactivity timer, indicating that the user has been active.
		/// Call this method whenever user activity is detected.
		/// </summary>
		public void Used()
		{
			_lastUsedTime = Time.time;
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Stops the current timer and disposes of the cancellation token source.
		/// </summary>
		private void StopTimer()
		{
			_cancellationTokenSource?.Cancel();
			_cancellationTokenSource?.Dispose();
			_cancellationTokenSource = null;
		}

		/// <summary>
		/// Runs the inactivity timer asynchronously, monitoring for user activity.
		/// </summary>
		/// <param name="cancellationToken">Token used to cancel the timer operation.</param>
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
			catch (OperationCanceledException) { }
		}

		#endregion
	}
}