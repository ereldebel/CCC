using System.Linq;
using NUnit.Framework;
using CCC.Runtime.Utils;

namespace CCC.Tests.Utils
{
	/// <summary>
	/// Unit tests for EnumUtils utility methods.
	/// </summary>
	public class EnumUtilsTests
	{
		/// <summary>
		/// Test enum for validation purposes.
		/// </summary>
		private enum TestEnum
		{
			First,
			Second,
			Third,
			Fourth
		}

		/// <summary>
		/// Single value enum for edge case testing.
		/// </summary>
		private enum SingleValueEnum
		{
			OnlyValue
		}

		[Test]
		public void Count_MultiValueEnum_ReturnsCorrectCount()
		{
			// Act
			var result = EnumUtils.Count<TestEnum>();

			// Assert
			Assert.AreEqual(4, result);
		}

		[Test]
		public void Count_SingleValueEnum_ReturnsOne()
		{
			// Act
			var result = EnumUtils.Count<SingleValueEnum>();

			// Assert
			Assert.AreEqual(1, result);
		}

		[Test]
		public void GetValues_MultiValueEnum_ReturnsAllValues()
		{
			// Act
			var result = EnumUtils.GetValues<TestEnum>().ToArray();

			// Assert
			Assert.AreEqual(4, result.Length);
			Assert.Contains(TestEnum.First, result);
			Assert.Contains(TestEnum.Second, result);
			Assert.Contains(TestEnum.Third, result);
			Assert.Contains(TestEnum.Fourth, result);
		}

		[Test]
		public void GetValues_SingleValueEnum_ReturnsOneValue()
		{
			// Act
			var result = EnumUtils.GetValues<SingleValueEnum>().ToArray();

			// Assert
			Assert.AreEqual(1, result.Length);
			Assert.Contains(SingleValueEnum.OnlyValue, result);
		}

		[Test]
		public void GetValues_MultiValueEnum_ReturnsInDefinitionOrder()
		{
			// Act
			var result = EnumUtils.GetValues<TestEnum>().ToArray();

			// Assert
			Assert.AreEqual(TestEnum.First, result[0]);
			Assert.AreEqual(TestEnum.Second, result[1]);
			Assert.AreEqual(TestEnum.Third, result[2]);
			Assert.AreEqual(TestEnum.Fourth, result[3]);
		}

		[Test]
		public void GetValues_AnyEnum_ReturnsEnumerableCollection()
		{
			// Act & Assert - Should not throw
			foreach (var value in EnumUtils.GetValues<TestEnum>())
			{
#pragma warning disable CS0183
				Assert.IsTrue(value is TestEnum);
#pragma warning restore CS0183
			}
		}

		[Test]
		public void Count_ComparedToGetValues_ReturnsMatchingCount()
		{
			// Act
			var countResult = EnumUtils.Count<TestEnum>();
			var valuesCount = EnumUtils.GetValues<TestEnum>().Count();

			// Assert
			Assert.AreEqual(countResult, valuesCount);
		}
	}
}
