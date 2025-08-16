using TMPro;
using UnityEngine;
using static CCC.Runtime.InputManager.DeviceType;

namespace CCC.Runtime
{
	/// <summary>
	/// Automatically updates UI text components to display the appropriate input control icons
	/// based on the currently detected input device type. Supports DualShock, Xbox Controller,
	/// and Keyboard input methods with automatic sprite asset switching and caching for performance.
	/// 
	/// This component listens for device type changes and dynamically loads the corresponding
	/// sprite assets from Resources, ensuring players always see the correct button/input icons
	/// for their current input method.
	/// </summary>
	public class DeviceControlsListener : MonoBehaviour
	{
		private TextMeshProUGUI _tmp;
		private static (TMP_SpriteAsset spriteAsset, InputManager.DeviceType type)? _cachedSpriteAsset = null;
		private void Awake()
		{
			_tmp = GetComponent<TextMeshProUGUI>();
			InputManager.Instance.DeviceTypeChanged += ChangeSpriteAsset;
			ChangeSpriteAsset(InputManager.Instance.CurrentDeviceType);
		}

		private void OnDestroy()
		{
			InputManager.Instance.DeviceTypeChanged -= ChangeSpriteAsset;
		}

		private void ChangeSpriteAsset(InputManager.DeviceType deviceType)
		{
			if (_cachedSpriteAsset.HasValue)
			{
				var cachedSpriteAsset = _cachedSpriteAsset.Value;
				if (cachedSpriteAsset.type == deviceType)
				{
					_tmp.spriteAsset = cachedSpriteAsset.spriteAsset;
					return;
				}
			}
			var spriteAsset = LoadSpriteAsset(deviceType);
			_tmp.spriteAsset = spriteAsset;
			_cachedSpriteAsset = (spriteAsset, deviceType);
		}

		private static TMP_SpriteAsset LoadSpriteAsset(InputManager.DeviceType deviceType)
		{
			return Resources.Load<TMP_SpriteAsset>(deviceType switch
			{
				DualShock => InputManager.Instance.dualShockSpriteAsset,
				XboxController => InputManager.Instance.xBoxSpriteAsset,
				_ => InputManager.Instance.keyboardSpriteAsset
			});
		}
	}
}