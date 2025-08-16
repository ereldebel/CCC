using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using CCC.Runtime;

namespace CCC.Tests
{
	/// <summary>
	/// Unit tests for UIGroup component functionality.
	/// </summary>
	public class UIGroupTests
	{
		private UIGroup uiGroup;
		private GameObject testGameObject;
		private CanvasGroup canvasGroup;
		private RectTransform rectTransform;

		[SetUp]
		public void SetUp()
		{
			testGameObject = new GameObject("TestUIGroup");
			canvasGroup = testGameObject.AddComponent<CanvasGroup>();
			rectTransform = testGameObject.AddComponent<RectTransform>();
			uiGroup = testGameObject.AddComponent<UIGroup>();
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
		public void Opacity_SetValue_UpdatesCanvasGroupAlpha()
		{
			// Test with a typical value
			uiGroup.Opacity = 0.7f;
			Assert.AreEqual(0.7f, canvasGroup.alpha, 0.001f);
		}

		[Test]
		public void Opacity_SetZero_UpdatesCanvasGroupAlpha()
		{
			// Test boundary case
			uiGroup.Opacity = 0f;
			Assert.AreEqual(0f, canvasGroup.alpha, 0.001f);
		}

		[Test]
		public void Offset_SetValue_UpdatesRectTransformPosition()
		{
			// Arrange
			Vector2 initialPosition = new Vector2(100f, 50f);
			Vector2 offset = new Vector2(20f, 30f);
			Vector2 expectedPosition = initialPosition + offset;
			
			rectTransform.anchoredPosition = initialPosition;

			// Act
			uiGroup.Offset = offset;

			// Assert
			Assert.AreEqual(expectedPosition.x, rectTransform.anchoredPosition.x, 0.001f);
			Assert.AreEqual(expectedPosition.y, rectTransform.anchoredPosition.y, 0.001f);
		}

		[Test]
		public void Offset_MultipleSetOperations_CalculatesFromBasePosition()
		{
			// Arrange
			Vector2 initialPosition = new Vector2(100f, 50f);
			rectTransform.anchoredPosition = initialPosition;

			// Act & Assert - First offset
			Vector2 firstOffset = new Vector2(10f, 20f);
			uiGroup.Offset = firstOffset;
			Vector2 expectedFirst = initialPosition + firstOffset;
			Assert.AreEqual(expectedFirst.x, rectTransform.anchoredPosition.x, 0.001f);
			Assert.AreEqual(expectedFirst.y, rectTransform.anchoredPosition.y, 0.001f);

			// Act & Assert - Second offset (should be relative to initial position, not current)
			Vector2 secondOffset = new Vector2(30f, 40f);
			uiGroup.Offset = secondOffset;
			Vector2 expectedSecond = initialPosition + secondOffset;
			Assert.AreEqual(expectedSecond.x, rectTransform.anchoredPosition.x, 0.001f);
			Assert.AreEqual(expectedSecond.y, rectTransform.anchoredPosition.y, 0.001f);
		}


	}
}
