using NUnit.Framework;
using UnityEngine;
using CCC.Runtime.Utils;

namespace CCC.Tests.Utils
{
	/// <summary>
	/// Unit tests for Vector2Utils extension methods.
	/// </summary>
	public class Vector2UtilsTests
	{
		[Test]
		public void ToVector3XZ_ValidVector2_ReturnsCorrectVector3()
		{
			// Arrange
			var vector2 = new Vector2(3f, 5f);
			var expected = new Vector3(3f, 0f, 5f);

			// Act
			var result = vector2.ToVector3XZ();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void Switch_ValidVector2_ReturnsSwappedComponents()
		{
			// Arrange
			var vector = new Vector2(3f, 7f);
			var expected = new Vector2(7f, 3f);

			// Act
			var result = vector.Switch();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void Rotate_90DegreesClockwise_ReturnsRotatedVector()
		{
			// Arrange
			var vector = new Vector2(1f, 0f); // Unit vector pointing right
			var degrees = 90f; // Quarter turn counterclockwise
			var expected = new Vector2(0f, 1f); // Should point up

			// Act
			var result = vector.Rotate(degrees);

			// Assert
			Assert.AreEqual(expected.x, result.x, 0.001f);
			Assert.AreEqual(expected.y, result.y, 0.001f);
		}

		[Test]
		public void RotateRad_QuarterTurnRadians_ReturnsRotatedVector()
		{
			// Arrange
			var vector = new Vector2(1f, 0f); // Unit vector pointing right
			var radians = Mathf.PI / 2f; // Quarter turn counterclockwise
			var expected = new Vector2(0f, 1f); // Should point up

			// Act
			var result = vector.RotateRad(radians);

			// Assert
			Assert.AreEqual(expected.x, result.x, 0.001f);
			Assert.AreEqual(expected.y, result.y, 0.001f);
		}

		[Test]
		public void Rotate_ZeroDegrees_ReturnsOriginalVector()
		{
			// Arrange
			var vector = new Vector2(3f, 4f);

			// Act
			var result = vector.Rotate(0f);

			// Assert
			Assert.AreEqual(vector, result);
		}

		[Test]
		public void Rotate_360Degrees_ReturnsOriginalVector()
		{
			// Arrange
			var vector = new Vector2(3f, 4f);

			// Act
			var result = vector.Rotate(360f);

			// Assert
			Assert.AreEqual(vector.x, result.x, 0.001f);
			Assert.AreEqual(vector.y, result.y, 0.001f);
		}
	}
}
