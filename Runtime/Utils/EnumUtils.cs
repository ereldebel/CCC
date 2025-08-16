using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CCC.Runtime.Utils
{
	/// <summary>
	/// Utility methods for enum operations, providing convenient functions for counting enum values
	/// and enumerating all values of a given enum type.
	/// </summary>
	public static class EnumUtils
	{
		/// <summary>
		/// Counts the number of elements in the enum T.
		/// Uses reflection to determine the total count of enum values.
		/// </summary>
		/// <typeparam name="T">The enum type to count values for.</typeparam>
		/// <returns>The number of enum values defined in the enum type.</returns>
		[Pure]
		public static int Count<T>() where T : Enum
		{
			return Enum.GetNames(typeof(T)).Length;
		}

		/// <summary>
		/// Enumerates over all the values of the enum T.
		/// Provides a strongly-typed way to iterate through all enum values.
		/// </summary>
		/// <typeparam name="T">The enum type to enumerate values for.</typeparam>
		/// <returns>An enumerable collection of all enum values.</returns>
		[Pure]
		public static IEnumerable<T> GetValues<T>() where T : Enum
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
	}
}