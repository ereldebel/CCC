using NUnit.Framework;
using UnityEngine;
using CCC.Runtime.Utils;

namespace CCC.Tests.Utils
{
	/// <summary>
	/// Unit tests for Vector2IntUtils extension methods.
	/// </summary>
	public class Vector2IntUtilsTests
	{
		[Test]
		public void L1Norm_MixedSignComponents_ReturnsCorrectSum()
		{
			// Arrange
			var vector = new Vector2Int(3, -4);
			var expected = 7; // |3| + |-4| = 3 + 4 = 7

			// Act
			var result = vector.L1Norm();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Norm_ZeroVector_ReturnsZero()
		{
			// Arrange
			var vector = Vector2Int.zero;
			var expected = 0;

			// Act
			var result = vector.L1Norm();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Norm_PositiveComponents_ReturnsCorrectSum()
		{
			// Arrange
			var vector = new Vector2Int(5, 7);
			var expected = 12; // |5| + |7| = 5 + 7 = 12

			// Act
			var result = vector.L1Norm();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Distance_DifferentPoints_ReturnsCorrectDistance()
		{
			// Arrange
			var vectorA = new Vector2Int(1, 2);
			var vectorB = new Vector2Int(4, 6);
			var expected = 7; // |1-4| + |2-6| = 3 + 4 = 7

			// Act
			var result = Vector2IntUtils.L1Distance(vectorA, vectorB);

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Distance_SamePoints_ReturnsZero()
		{
			// Arrange
			var vector = new Vector2Int(3, 5);
			var expected = 0;

			// Act
			var result = Vector2IntUtils.L1Distance(vector, vector);

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Distance_NegativeComponents_ReturnsCorrectDistance()
		{
			// Arrange
			var vectorA = new Vector2Int(-2, -3);
			var vectorB = new Vector2Int(1, 2);
			var expected = 8; // |-2-1| + |-3-2| = 3 + 5 = 8

			// Act
			var result = Vector2IntUtils.L1Distance(vectorA, vectorB);

			// Assert
			Assert.AreEqual(expected, result);
		}
	}
}
