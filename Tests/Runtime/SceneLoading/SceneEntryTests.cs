using NUnit.Framework;
using CCC.Runtime.SceneLoading;

namespace CCC.Tests.SceneLoading
{
	/// <summary>
	/// Unit tests for SceneEntry configuration class.
	/// </summary>
	public class SceneEntryTests
	{
		[Test]
		public void DefaultConstructor_NewInstance_HasExpectedDefaults()
		{
			// Act
			var sceneEntry = new SceneEntry();

			// Assert - Test the meaningful defaults
			Assert.IsNull(sceneEntry.sceneName);
			Assert.AreEqual(SceneType.SceneLoader, sceneEntry.type);
			Assert.IsFalse(sceneEntry.loadOnStart);
		}
	}
}
