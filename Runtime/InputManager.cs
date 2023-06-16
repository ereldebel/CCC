using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;

namespace CCC.Runtime
{
	public class InputManager : MonoBehaviour
	{
		#region Serialized Properties

		[field: SerializeField] public string xBoxSpriteAsset = "Textures/Xbox Buttons";
		[field: SerializeField] public string dualShockSpriteAsset = "Textures/DualShock Buttons";
		[field: SerializeField] public string keyboardSpriteAsset = "Textures/Keyboard Buttons";
		
		#endregion
		
		#region Private Fields

		private DeviceType _deviceType = DeviceType.Keyboard;

		#endregion
		
		#region Properties

		public DeviceType CurrentDeviceType
		{
			get
			{
				var currDevice = GetDeviceType();
				if (_deviceType != currDevice)
					CurrentDeviceType = currDevice;
				return currDevice;
			}
			private set
			{
				if (_deviceType == value) return;
				DeviceTypeChanged?.Invoke(_deviceType = value);
			}
		}
		
		#endregion
		
		#region Singleton Implementation

		public static InputManager Instance { get; private set; }

		#endregion

		#region Events

		public event Action<DeviceType> DeviceTypeChanged;
		public event Action DeviceDisconnected;

		#endregion

		#region Event Functions

		private void Awake()
		{
			if (Instance != null)
				throw new Exception("Multiple InputManager instances");
			Instance = this;
			InputSystem.onDeviceChange += OnUserChange;
		}

		private void Start()
		{
			CurrentDeviceType = GetDeviceType();
		}

		private void OnDestroy()
		{
			InputSystem.onDeviceChange -= OnUserChange;
		}

		#endregion

		#region Private Methods

		private void OnUserChange(InputDevice device, InputDeviceChange change)
		{
			switch (change)
			{
				case InputDeviceChange.Added:
					CurrentDeviceType = GetDeviceType();
					break;
				case InputDeviceChange.Removed:
					CurrentDeviceType = GetDeviceType();
					goto case InputDeviceChange.Disconnected;
				case InputDeviceChange.Disconnected:
					DeviceDisconnected?.Invoke();
					break;
			}
		}

		private static DeviceType GetDeviceType()
		{
			return Gamepad.current switch
			{
				XInputController => DeviceType.XboxController,
				DualShockGamepad => DeviceType.DualShock,
				_ => DeviceType.Keyboard
			};
		}

		#endregion

		#region Types

		public enum DeviceType
		{
			DualShock,
			XboxController,
			Keyboard,
		}

		#endregion
	}
}