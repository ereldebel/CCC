using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XInput;
using UnityEngine.InputSystem.DualShock;

namespace CCC.Runtime
{
	/// <summary>
	/// Manages input device detection and provides centralized access to input-related functionality
	/// including device type changes, haptic feedback, and sprite asset management for different input devices.
	/// </summary>
	public class InputManager : MonoBehaviour
	{
		#region Serialized Properties

		[field: SerializeField, Tooltip("Path to the Xbox controller button sprite asset in Resources folder")]
		public string xBoxSpriteAsset = "Textures/Xbox Buttons";

		[field: SerializeField, Tooltip("Path to the DualShock controller button sprite asset in Resources folder")]
		public string dualShockSpriteAsset = "Textures/DualShock Buttons";

		[field: SerializeField, Tooltip("Path to the keyboard button sprite asset in Resources folder")]
		public string keyboardSpriteAsset = "Textures/Keyboard Buttons";

		#endregion

		#region Private Fields

		private DeviceType _deviceType = DeviceType.Keyboard;
		private Haptics _haptics;

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the haptic feedback controller for the current input device.
		/// </summary>
		/// <value>The Haptics instance for managing controller vibration and feedback.</value>
		public Haptics Haptics
		{
			get => _haptics ??= new Haptics(this);
			set => _haptics = value;
		}

		/// <summary>
		/// Gets the current input device type and automatically updates when the device changes.
		/// </summary>
		/// <value>The currently detected input device type.</value>
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

		/// <summary>
		/// The singleton instance of the InputManager.
		/// </summary>
		/// <value>The global InputManager instance.</value>
		public static InputManager Instance { get; private set; }

		#endregion

		#region Events

		/// <summary>
		/// Event triggered when the input device type changes.
		/// </summary>
		public event Action<DeviceType> DeviceTypeChanged;
		
		/// <summary>
		/// Event triggered when the current input device is disconnected.
		/// </summary>
		public event Action DeviceDisconnected;

		#endregion

		#region Event Functions

		/// <summary>
		/// Initializes the singleton instance and subscribes to input device change events.
		/// </summary>
		private void Awake()
		{
			if (Instance != null)
				throw new Exception("Multiple InputManager instances");
			Instance = this;
			InputSystem.onDeviceChange += OnUserChange;
		}

		/// <summary>
		/// Initializes the current device type and haptic feedback system.
		/// </summary>
		private void Start()
		{
			CurrentDeviceType = GetDeviceType();
			Haptics = new Haptics(this);
		}

		/// <summary>
		/// Cleans up event subscriptions and disposes of haptic resources.
		/// </summary>
		private void OnDestroy()
		{
			InputSystem.onDeviceChange -= OnUserChange;
			Haptics?.Dispose();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Handles input device changes and updates the current device type accordingly.
		/// </summary>
		/// <param name="device">The input device that changed.</param>
		/// <param name="change">The type of change that occurred.</param>
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

		/// <summary>
		/// Determines the current input device type based on the active gamepad.
		/// </summary>
		/// <returns>The detected input device type.</returns>
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

		/// <summary>
		/// Represents different types of input devices supported by the InputManager.
		/// </summary>
		public enum DeviceType
		{
			/// <summary>
			/// PlayStation DualShock controller.
			/// </summary>
			DualShock,

			/// <summary>
			/// Xbox controller.
			/// </summary>
			XboxController,

			/// <summary>
			/// Keyboard and mouse input.
			/// </summary>
			Keyboard,
		}
		#endregion
	}
}