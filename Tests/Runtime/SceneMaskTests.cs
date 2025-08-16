using System.Linq;
using NUnit.Framework;
using CCC.Runtime.SceneLoading;

namespace CCC.Tests
{
	/// <summary>
	/// Unit tests for SceneMask structure and its operations.
	/// </summary>
	public class SceneMaskTests
	{
		[Test]
		public void Constructor_SingleSceneType_CreatesMaskWithOneType()
		{
			// Arrange & Act
			var mask = new SceneMask(SceneType.Dynamic);

			// Assert
			var types = mask.ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.Contains(SceneType.Dynamic, types);
		}

		[Test]
		public void Constructor_MultipleSceneTypes_CreatesMaskWithAllTypes()
		{
			// Arrange & Act
			var mask = new SceneMask(SceneType.Dynamic, SceneType.Constant);

			// Assert
			var types = mask.ToArray();
			Assert.AreEqual(2, types.Length);
			Assert.Contains(SceneType.Dynamic, types);
			Assert.Contains(SceneType.Constant, types);
		}

		[Test]
		public void Constructor_AllSceneTypes_CreatesMaskWithAllTypes()
		{
			// Arrange & Act
			var mask = new SceneMask(SceneType.SceneLoader, SceneType.Constant, SceneType.Dynamic, SceneType.ConstantReload);

			// Assert
			var types = mask.ToArray();
			Assert.AreEqual(4, types.Length);
			Assert.Contains(SceneType.SceneLoader, types);
			Assert.Contains(SceneType.Constant, types);
			Assert.Contains(SceneType.Dynamic, types);
			Assert.Contains(SceneType.ConstantReload, types);
		}

		[Test]
		public void InverseMask_SingleType_ReturnsAllExceptSpecifiedType()
		{
			// Arrange & Act
			var mask = SceneMask.InverseMask(SceneType.Dynamic);

			// Assert
			var types = mask.ToArray();
			Assert.Contains(SceneType.SceneLoader, types);
			Assert.Contains(SceneType.Constant, types);
			Assert.Contains(SceneType.ConstantReload, types);
			Assert.IsFalse(types.Contains(SceneType.Dynamic));
		}

		[Test]
		public void InverseMask_MultipleTypes_ReturnsAllExceptSpecifiedTypes()
		{
			// Arrange & Act
			var mask = SceneMask.InverseMask(SceneType.Dynamic, SceneType.Constant);

			// Assert
			var types = mask.ToArray();
			Assert.Contains(SceneType.SceneLoader, types);
			Assert.Contains(SceneType.ConstantReload, types);
			Assert.IsFalse(types.Contains(SceneType.Dynamic));
			Assert.IsFalse(types.Contains(SceneType.Constant));
		}

		[Test]
		public void BitwiseAnd_OverlappingMasks_ReturnsIntersectionMask()
		{
			// Arrange
			var mask1 = new SceneMask(SceneType.Dynamic, SceneType.Constant);
			var mask2 = new SceneMask(SceneType.Constant, SceneType.ConstantReload);

			// Act
			var result = mask1 & mask2;

			// Assert
			var types = result.ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.Contains(SceneType.Constant, types);
		}

		[Test]
		public void BitwiseOr_TwoMasks_ReturnsUnionMask()
		{
			// Arrange
			var mask1 = new SceneMask(SceneType.Dynamic);
			var mask2 = new SceneMask(SceneType.Constant);

			// Act
			var result = mask1 | mask2;

			// Assert
			var types = result.ToArray();
			Assert.AreEqual(2, types.Length);
			Assert.Contains(SceneType.Dynamic, types);
			Assert.Contains(SceneType.Constant, types);
		}

		[Test]
		public void ExplicitCast_FromInt_CreatesMaskWithCorrectTypes()
		{
			// Arrange
			int maskValue = 5; // Binary: 101, represents SceneLoader (bit 0) and Dynamic (bit 2)

			// Act
			var mask = (SceneMask)maskValue;

			// Assert
			var types = mask.ToArray();
			Assert.AreEqual(2, types.Length);
			Assert.Contains(SceneType.SceneLoader, types);
			Assert.Contains(SceneType.Dynamic, types);
		}

		[Test]
		public void ExplicitCast_FromSceneType_CreatesMaskWithSingleType()
		{
			// Arrange & Act
			var mask = (SceneMask)SceneType.ConstantReload;

			// Assert
			var types = mask.ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.Contains(SceneType.ConstantReload, types);
		}

		[Test]
		public void GetEnumerator_EmptyMask_ReturnsEmptyCollection()
		{
			// Arrange
			var mask = new SceneMask(0); // Empty mask

			// Act
			var types = mask.ToArray();

			// Assert
			Assert.AreEqual(0, types.Length);
		}
	}
}
