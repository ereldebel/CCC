using NUnit.Framework;
using CCC.Runtime;

namespace CCC.Tests
{
	/// <summary>
	/// Unit tests for InputManager DeviceType enum and related functionality.
	/// </summary>
	public class InputManagerTests
	{
		[Test]
		public void DeviceType_AllValues_AreDefinedInEnum()
		{
			// Assert that all expected device types exist
			Assert.IsTrue(System.Enum.IsDefined(typeof(InputManager.DeviceType), InputManager.DeviceType.DualShock));
			Assert.IsTrue(System.Enum.IsDefined(typeof(InputManager.DeviceType), InputManager.DeviceType.XboxController));
			Assert.IsTrue(System.Enum.IsDefined(typeof(InputManager.DeviceType), InputManager.DeviceType.Keyboard));
		}

		[Test]
		public void DeviceType_EnumValues_HasExpectedCount()
		{
			// Arrange
			var values = System.Enum.GetValues(typeof(InputManager.DeviceType));

			// Assert
			Assert.AreEqual(3, values.Length);
		}

		[Test]
		public void DeviceType_EnumValues_HaveCorrectIntegerValues()
		{
			// Assert enum values are in expected order
			Assert.AreEqual(0, (int)InputManager.DeviceType.DualShock);
			Assert.AreEqual(1, (int)InputManager.DeviceType.XboxController);
			Assert.AreEqual(2, (int)InputManager.DeviceType.Keyboard);
		}

		[Test]
		public void DeviceType_ToString_ReturnsCorrectNames()
		{
			// Assert
			Assert.AreEqual("DualShock", InputManager.DeviceType.DualShock.ToString());
			Assert.AreEqual("XboxController", InputManager.DeviceType.XboxController.ToString());
			Assert.AreEqual("Keyboard", InputManager.DeviceType.Keyboard.ToString());
		}

		[Test]
		public void DeviceType_ExplicitCastToInt_ReturnsCorrectValues()
		{
			// Act & Assert
			Assert.AreEqual(0, (int)InputManager.DeviceType.DualShock);
			Assert.AreEqual(1, (int)InputManager.DeviceType.XboxController);
			Assert.AreEqual(2, (int)InputManager.DeviceType.Keyboard);
		}

		[Test]
		public void DeviceType_ExplicitCastFromInt_ReturnsCorrectEnumValues()
		{
			// Act & Assert
			Assert.AreEqual(InputManager.DeviceType.DualShock, (InputManager.DeviceType)0);
			Assert.AreEqual(InputManager.DeviceType.XboxController, (InputManager.DeviceType)1);
			Assert.AreEqual(InputManager.DeviceType.Keyboard, (InputManager.DeviceType)2);
		}

		// Note: Testing the full InputManager singleton behavior would require more complex
		// setup with Unity's Input System and is better suited for integration tests
		// rather than unit tests. The singleton pattern itself would need careful
		// consideration for testing to avoid side effects between tests.
	}
}
