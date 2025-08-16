using System;
using System.Threading;
using NUnit.Framework;
using UnityEngine;
using CCC.Runtime.Utils;

namespace CCC.Tests.Utils
{
	/// <summary>
	/// Unit tests for UniTaskUtils utility methods.
	/// Note: These tests focus on API validation rather than actual async timing,
	/// as timing-based tests are better suited for integration testing.
	/// </summary>
	public class UniTaskUtilsTests
	{
		[Test]
		public void Delay_ValidSeconds_ReturnsUniTask()
		{
			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Delay(0.1f);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Delay_ZeroSeconds_ReturnsUniTask()
		{
			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Delay(0f);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Delay_WithIgnoreTimeScale_ReturnsUniTask()
		{
			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Delay(0.1f, ignoreTimeScale: true);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Delay_WithCancellationToken_ReturnsUniTask()
		{
			var cancellationToken = new CancellationTokenSource().Token;

			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Delay(0.1f, cancellationToken: cancellationToken);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Interpolate_ValidParameters_ReturnsUniTask()
		{
			Action<float> interpolator = t => { /* Do nothing for test */ };
			float duration = 1.0f;

			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Interpolate(interpolator, duration);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Interpolate_WithPredicate_ReturnsUniTask()
		{
			Func<float, bool> interpolator = t => true;
			float duration = 1.0f;

			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Interpolate(interpolator, duration);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void PerformWhile_ValidParameters_ReturnsUniTask()
		{
			Func<int> action = () => 1;
			Predicate<int> predicate = value => value > 0;

			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.PerformWhile(action, predicate);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void PerformWhile_WithEndAction_ReturnsUniTask()
		{
			Func<int> action = () => 1;
			Predicate<int> predicate = value => value > 0;
			Action endAction = () => { /* Do nothing for test */ };

			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.PerformWhile(action, predicate, endAction);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Delay_NegativeSeconds_ReturnsUniTask()
		{
			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Delay(-1.0f);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Interpolate_ZeroDuration_ReturnsUniTask()
		{
			Action<float> interpolator = t => { /* Do nothing for test */ };

			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Interpolate(interpolator, 0f);
				Assert.IsNotNull(task);
			});
		}

		[Test]
		public void Interpolate_NegativeDuration_ReturnsUniTask()
		{
			Action<float> interpolator = t => { /* Do nothing for test */ };

			Assert.DoesNotThrow(() =>
			{
				var task = UniTaskUtils.Interpolate(interpolator, -1f);
				Assert.IsNotNull(task);
			});
		}
	}
}
