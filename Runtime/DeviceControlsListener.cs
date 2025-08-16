using TMPro;
using UnityEngine;
using static CCC.Runtime.InputManager.DeviceType;

namespace CCC.Runtime
{
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