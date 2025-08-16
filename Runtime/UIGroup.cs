using UnityEngine;

namespace CCC.Runtime
{
	/// <summary>
	/// A utility component that provides easy access to CanvasGroup and RectTransform components
	/// with lazy initialization and convenient properties for opacity and offset manipulation.
	/// </summary>
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

		/// <summary>
		/// Sets the opacity of the UI group by modifying the CanvasGroup's alpha value.
		/// </summary>
		/// <value>A value between 0 (transparent) and 1 (opaque).</value>
		public float Opacity
		{
			set => CanvasGroup.alpha = value;
		}

		/// <summary>
		/// Sets the offset position of the UI group relative to its base anchored position.
		/// </summary>
		/// <value>The offset vector to apply to the base position.</value>
		public Vector2 Offset
		{
			set => RectTransform.anchoredPosition = _baseOffset + value;
		}

		#endregion
	}
}