using System.Linq;
using NUnit.Framework;
using CCC.Runtime.SceneLoading;

namespace CCC.Tests.SceneLoading
{
	/// <summary>
	/// Unit tests for SceneMask structure and its operations.
	/// </summary>
	public class SceneMaskTests
	{
		[Test]
		public void Constructor_SingleSceneType_CreatesMaskWithOneType()
		{
			var mask = new SceneMask(SceneType.Dynamic);

			var types = mask.ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.Contains(SceneType.Dynamic, types);
		}

		[Test]
		public void Constructor_MultipleSceneTypes_CreatesMaskWithAllTypes()
		{
			var mask = new SceneMask(SceneType.Dynamic, SceneType.Constant);

			var types = mask.ToArray();
			Assert.AreEqual(2, types.Length);
			Assert.Contains(SceneType.Dynamic, types);
			Assert.Contains(SceneType.Constant, types);
		}

		[Test]
		public void Constructor_AllSceneTypes_CreatesMaskWithAllTypes()
		{
			var mask = new SceneMask(SceneType.SceneLoader, SceneType.Constant, SceneType.Dynamic, SceneType.ConstantReload);

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
			var mask = SceneMask.InverseMask(SceneType.Dynamic);

			var types = mask.ToArray();
			Assert.Contains(SceneType.SceneLoader, types);
			Assert.Contains(SceneType.Constant, types);
			Assert.Contains(SceneType.ConstantReload, types);
			Assert.IsFalse(types.Contains(SceneType.Dynamic));
		}

		[Test]
		public void InverseMask_MultipleTypes_ReturnsAllExceptSpecifiedTypes()
		{
			var mask = SceneMask.InverseMask(SceneType.Dynamic, SceneType.Constant);

			var types = mask.ToArray();
			Assert.Contains(SceneType.SceneLoader, types);
			Assert.Contains(SceneType.ConstantReload, types);
			Assert.IsFalse(types.Contains(SceneType.Dynamic));
			Assert.IsFalse(types.Contains(SceneType.Constant));
		}

		[Test]
		public void BitwiseAnd_OverlappingMasks_ReturnsIntersectionMask()
		{
			var mask1 = new SceneMask(SceneType.Dynamic, SceneType.Constant);
			var mask2 = new SceneMask(SceneType.Constant, SceneType.ConstantReload);

			var result = mask1 & mask2;

			var types = result.ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.Contains(SceneType.Constant, types);
		}

		[Test]
		public void BitwiseOr_TwoMasks_ReturnsUnionMask()
		{
			var mask1 = new SceneMask(SceneType.Dynamic);
			var mask2 = new SceneMask(SceneType.Constant);

			var result = mask1 | mask2;

			var types = result.ToArray();
			Assert.AreEqual(2, types.Length);
			Assert.Contains(SceneType.Dynamic, types);
			Assert.Contains(SceneType.Constant, types);
		}

		[Test]
		public void ExplicitCast_FromInt_CreatesMaskWithCorrectTypes()
		{
			int maskValue = 0b_101; // Represents SceneLoader (bit 0) and Dynamic (bit 2)

			var mask = (SceneMask)maskValue;

			var types = mask.ToArray();
			Assert.AreEqual(2, types.Length);
			Assert.Contains(SceneType.SceneLoader, types);
			Assert.Contains(SceneType.Dynamic, types);
		}

		[Test]
		public void ExplicitCast_FromSceneType_CreatesMaskWithSingleType()
		{
			var mask = (SceneMask)SceneType.ConstantReload;

			var types = mask.ToArray();
			Assert.AreEqual(1, types.Length);
			Assert.Contains(SceneType.ConstantReload, types);
		}

		[Test]
		public void GetEnumerator_EmptyMask_ReturnsEmptyCollection()
		{
			var mask = new SceneMask(0); // Empty mask

			var types = mask.ToArray();

			Assert.AreEqual(0, types.Length);
		}
	}
}
