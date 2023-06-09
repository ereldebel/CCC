using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;

namespace CCC.Runtime
{
	public class InputManager : MonoBehaviour
	{
		#region Private Fields

		private DeviceType _deviceType = DeviceType.Keyboard;

		#endregion
		
		#region Properties

		public DeviceType CurrentDeviceType
		{
			get => _deviceType;
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
				throw new Exception("Multiple GameManager instances");
			Instance = this;
			InputSystem.onDeviceChange += OnUserChange;
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
				case InputDeviceChange.Removed:
					CurrentDeviceType = GetDeviceType();
					break;
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