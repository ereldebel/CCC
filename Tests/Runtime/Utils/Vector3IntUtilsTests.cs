using NUnit.Framework;
using UnityEngine;
using CCC.Runtime.Utils;

namespace CCC.Tests.Utils
{
	/// <summary>
	/// Unit tests for Vector3IntUtils extension methods.
	/// </summary>
	public class Vector3IntUtilsTests
	{
		[Test]
		public void L1Norm_MixedSignComponents_ReturnsCorrectSum()
		{
			// Arrange
			var vector = new Vector3Int(3, -4, 5);
			var expected = 12; // |3| + |-4| + |5| = 3 + 4 + 5 = 12

			// Act
			var result = vector.L1Norm();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Norm_ZeroVector_ReturnsZero()
		{
			// Arrange
			var vector = Vector3Int.zero;
			var expected = 0;

			// Act
			var result = vector.L1Norm();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Norm_AllNegativeComponents_ReturnsCorrectSum()
		{
			// Arrange
			var vector = new Vector3Int(-2, -3, -1);
			var expected = 6; // |-2| + |-3| + |-1| = 2 + 3 + 1 = 6

			// Act
			var result = vector.L1Norm();

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Distance_DifferentPoints_ReturnsCorrectDistance()
		{
			// Arrange
			var vectorA = new Vector3Int(1, 2, 3);
			var vectorB = new Vector3Int(4, 6, 1);
			var expected = 9; // |1-4| + |2-6| + |3-1| = 3 + 4 + 2 = 9

			// Act
			var result = Vector3IntUtils.L1Distance(vectorA, vectorB);

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Distance_SamePoints_ReturnsZero()
		{
			// Arrange
			var vector = new Vector3Int(3, 5, 7);
			var expected = 0;

			// Act
			var result = Vector3IntUtils.L1Distance(vector, vector);

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Distance_NegativeComponents_ReturnsCorrectDistance()
		{
			// Arrange
			var vectorA = new Vector3Int(-2, -3, 1);
			var vectorB = new Vector3Int(1, 2, -4);
			var expected = 13; // |-2-1| + |-3-2| + |1-(-4)| = 3 + 5 + 5 = 13

			// Act
			var result = Vector3IntUtils.L1Distance(vectorA, vectorB);

			// Assert
			Assert.AreEqual(expected, result);
		}

		[Test]
		public void L1Distance_DifferentVectors_ReturnsSymmetricDistance()
		{
			// Arrange
			var vectorA = new Vector3Int(1, 5, -2);
			var vectorB = new Vector3Int(3, -1, 4);

			// Act
			var distanceAB = Vector3IntUtils.L1Distance(vectorA, vectorB);
			var distanceBA = Vector3IntUtils.L1Distance(vectorB, vectorA);

			// Assert
			Assert.AreEqual(distanceAB, distanceBA);
		}
	}
}
