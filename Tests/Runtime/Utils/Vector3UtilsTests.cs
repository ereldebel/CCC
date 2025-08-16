using NUnit.Framework;
using UnityEngine;
using CCC.Runtime.Utils;

namespace CCC.Tests.Utils
{
	/// <summary>
	/// Unit tests for Vector3Utils extension methods.
	/// </summary>
	public class Vector3UtilsTests
	{
		[Test]
		public void ToVector3XZ_ValidVector3_ReturnsProjectedVector()
		{
			// Arrange
			var vector = new Vector3(3f, 7f, 5f);
			var expected = new Vector3(3f, 0f, 5f);

			// Act
			var result = vector.ToVector3XZ();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ToVector3XZ_ZeroYComponent_RemainsUnchanged()
		{
			// Arrange
			var vector = new Vector3(2f, 0f, 4f);
			var expected = new Vector3(2f, 0f, 4f);

			// Act
			var result = vector.ToVector3XZ();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ToVector2XZ_ValidVector3_ReturnsXZComponents()
		{
			// Arrange
			var vector = new Vector3(3f, 7f, 5f);
			var expected = new Vector2(3f, 5f); // X and Z components

			// Act
			var result = vector.ToVector2XZ();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ToVector2XZ_NegativeComponents_ReturnsCorrectMapping()
		{
			// Arrange
			var vector = new Vector3(-2f, 10f, -8f);
			var expected = new Vector2(-2f, -8f);

			// Act
			var result = vector.ToVector2XZ();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void ToVector3XZ_AnyVector_PreservesXAndZComponents()
		{
			// Arrange
			var vector = new Vector3(1.5f, 100f, -3.7f);

			// Act
			var result = vector.ToVector3XZ();

			// Assert
			Assert.AreEqual(vector.x, result.x);
			Assert.AreEqual(0f, result.y);
			Assert.AreEqual(vector.z, result.z);
		}

		[Test]
		public void ToVector2XZ_AnyVector_MapsXZToXY()
		{
			// Arrange
			var vector = new Vector3(2.5f, 50f, -1.2f);

			// Act
			var result = vector.ToVector2XZ();

			// Assert
			Assert.AreEqual(vector.x, result.x);
			Assert.AreEqual(vector.z, result.y);
		}
	}
}
