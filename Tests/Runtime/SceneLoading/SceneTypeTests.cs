using NUnit.Framework;
using CCC.Runtime.SceneLoading;

namespace CCC.Tests.SceneLoading
{
	/// <summary>
	/// Unit tests for SceneType enum values and behavior.
	/// </summary>
	public class SceneTypeTests
	{
		[Test]
		public void EnumValues_ExpectedCount_IsCorrect()
		{
			// Assert that we have the expected number of scene types
			var values = System.Enum.GetValues(typeof(SceneType));
			Assert.AreEqual(4, values.Length);
		}

		[Test]
		public void EnumValues_CorrectOrder_MaintainsExpectedSequence()
		{
			// Assert enum values maintain expected order for compatibility
			Assert.AreEqual(0, (int)SceneType.SceneLoader);
			Assert.AreEqual(1, (int)SceneType.Constant);
			Assert.AreEqual(2, (int)SceneType.Dynamic);
			Assert.AreEqual(3, (int)SceneType.ConstantReload);
		}
	}
}
