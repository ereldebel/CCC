using UnityEngine;

namespace CCC.Runtime
{
	[RequireComponent(typeof(CanvasGroup))]
	public class UIGroup : MonoBehaviour
	{
		#region Private Fields

		private CanvasGroup _canvasGroup;
		private RectTransform _rectTransform;

		private bool _canvasGroupSet;
		private bool _rectTransformSet;

		private Vector2 _baseOffset;

		#endregion

		#region Properties

		private RectTransform RectTransform
		{
			get
			{
				if (!_rectTransformSet)
				{
					_rectTransform = GetComponent<RectTransform>();
					_baseOffset = _rectTransform.anchoredPosition;
					_rectTransformSet = true;
				}

				return _rectTransform;
			}
		}
		private CanvasGroup CanvasGroup
		{
			get
			{
				if (!_canvasGroupSet)
				{
					_canvasGroup = GetComponent<CanvasGroup>();
					_canvasGroupSet = true;
				}

				return _canvasGroup;
			}
		}

		public float Opacity
		{
			set => CanvasGroup.alpha = value;
		}

		public Vector2 Offset
		{
			set => RectTransform.anchoredPosition = _baseOffset + value;
		}

		#endregion
	}
}