using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using TMPro;
using CCC.Runtime;

namespace CCC.Tests
{
	/// <summary>
	/// Unit tests for DeviceControlsListener component functionality.
	/// </summary>
	public class DeviceControlsListenerTests
	{
		private DeviceControlsListener deviceControlsListener;
		private GameObject testGameObject;
		private TextMeshProUGUI textMeshPro;

		[SetUp]
		public void SetUp()
		{
			testGameObject = new GameObject("TestDeviceControlsListener");
			textMeshPro = testGameObject.AddComponent<TextMeshProUGUI>();
			deviceControlsListener = testGameObject.AddComponent<DeviceControlsListener>();
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
		public void Component_InitializesWithTextMeshPro_SuccessfullyAdded()
		{
			Assert.IsNotNull(deviceControlsListener);
			var textMeshComponent = deviceControlsListener.GetComponent<TextMeshProUGUI>();
			Assert.IsNotNull(textMeshComponent);
		}
	}
}
