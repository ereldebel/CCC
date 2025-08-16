using NUnit.Framework;
using UnityEngine;
using CCC.Runtime;

namespace CCC.Tests
{
	/// <summary>
	/// Unit tests for Haptics system functionality.
	/// Focus on testing the logic around timed vs infinite haptics.
	/// </summary>
	public class HapticsTests
	{
		[Test]
		public void Constructor_SingleValue_CreatesInfiniteHaptic()
		{
			var hapticRef = new Haptics.HapticsReference(0.6f);

			Assert.AreEqual(float.PositiveInfinity, hapticRef.Duration);
			Assert.IsFalse(hapticRef.Timed);
		}

		[Test]
		public void Constructor_WithDuration_CreatesTimedHaptic()
		{
			var hapticRef = new Haptics.HapticsReference(0.8f, 0.3f, 2.5f);

			Assert.AreEqual(2.5f, hapticRef.Duration);
			Assert.IsTrue(hapticRef.Timed);
		}

		[Test]
		public void Timed_InfiniteDuration_ReturnsFalse()
		{
			var infiniteHaptic = new Haptics.HapticsReference(0.5f, 0.3f, float.PositiveInfinity);

			Assert.IsFalse(infiniteHaptic.Timed);
		}

		[Test]
		public void Constructor_ZeroDuration_CreatesTimedHaptic()
		{
			var hapticRef = new Haptics.HapticsReference(0.5f, 0.3f, 0f);

			Assert.AreEqual(0f, hapticRef.Duration);
			Assert.IsTrue(hapticRef.Timed);
		}
	}
}
