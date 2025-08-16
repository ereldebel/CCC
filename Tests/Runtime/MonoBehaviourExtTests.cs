using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CCC.Runtime;

namespace CCC.Tests
{
	/// <summary>
	/// Unit tests for MonoBehaviourExt cached component access.
	/// </summary>
	public class MonoBehaviourExtTests
	{
		private MonoBehaviourExt testComponent;
		private GameObject testGameObject;

		[SetUp]
		public void SetUp()
		{
			testGameObject = new GameObject("TestGameObject");
			testComponent = testGameObject.AddComponent<MonoBehaviourExt>();
		}

		[TearDown]
		public void TearDown()
		{
			if (testGameObject != null)
			{
				Object.DestroyImmediate(testGameObject);
			}
		}

		[Test]
		public void Transform_FirstAccess_ReturnsCorrectTransform()
		{
			// Act
			var result = testComponent.transform;

			// Assert
			Assert.AreSame(testGameObject.transform, result);
		}

		[Test]
		public void Transform_MultipleAccesses_ReturnsCachedInstance()
		{
			// Act
			var result1 = testComponent.transform;
			var result2 = testComponent.transform;

			// Assert
			Assert.AreSame(result1, result2);
		}

		[Test]
		public void GameObject_FirstAccess_ReturnsCorrectGameObject()
		{
			// Act
			var result = testComponent.gameObject;

			// Assert
			Assert.AreSame(testGameObject, result);
		}

		[Test]
		public void GameObject_MultipleAccesses_ReturnsCachedInstance()
		{
			// Act
			var result1 = testComponent.gameObject;
			var result2 = testComponent.gameObject;

			// Assert
			Assert.AreSame(result1, result2);
		}

		[Test]
		public void Transform_AnyAccess_ReturnsNonNullReference()
		{
			// Act
			var result = testComponent.transform;

			// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		public void GameObject_AnyAccess_ReturnsNonNullReference()
		{
			// Act
			var result = testComponent.gameObject;

			// Assert
			Assert.IsNotNull(result);
		}

		[Test]
		public void CachedProperties_RepeatedAccess_MaintainsFunctionality()
		{
			// This test demonstrates that cached access should be at least as fast as base access
			// While we can't easily measure performance in unit tests, we can ensure functionality

			// Act & Assert - Multiple accesses should work without issues
			for (int i = 0; i < 100; i++)
			{
				Assert.IsNotNull(testComponent.transform);
				Assert.IsNotNull(testComponent.gameObject);
			}
		}
	}
}
