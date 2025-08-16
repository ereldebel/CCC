using System;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CCC.Runtime;

namespace CCC.Tests
{
	/// <summary>
	/// Unit tests for InactivityTimer component functionality.
	/// Focus on testing the public API and component behavior.
	/// </summary>
	public class InactivityTimerTests
	{
		private InactivityTimer inactivityTimer;
		private GameObject testGameObject;

		[SetUp]
		public void SetUp()
		{
			testGameObject = new GameObject("TestInactivityTimer");
			inactivityTimer = testGameObject.AddComponent<InactivityTimer>();
		}

		[TearDown]
		public void TearDown()
		{
			if (testGameObject != null)
			{
				UnityEngine.Object.DestroyImmediate(testGameObject);
			}
		}

		[Test]
		public void TimeBeforeAction_DefaultValue_IsCorrect()
		{
			Assert.AreEqual(60u, inactivityTimer.TimeBeforeAction);
		}

		[Test]
		public void Used_Called_DoesNotThrow()
		{
			// Act & Assert - This tests that the method executes without error
			Assert.DoesNotThrow(() => inactivityTimer.Used());
		}

		[Test]
		public void InactivityAction_DefaultValue_IsNull()
		{
			// Assert - InactivityAction should be null by default since it has private setter
			Assert.IsNull(inactivityTimer.InactivityAction);
		}
	}
}
