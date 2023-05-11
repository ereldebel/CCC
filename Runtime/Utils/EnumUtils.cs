using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace CCC.Runtime.Utils
{
	public static class EnumUtils
	{
		/// <summary>
		/// Counts the number of elements in the enum T.
		/// </summary>
		[Pure]
		public static int Count<T>() where T : Enum
		{
			return Enum.GetNames(typeof(T)).Length;
		}

		/// <summary>
		/// Enumerates over all the values of the enum T.
		/// </summary>
		[Pure]
		public static IEnumerable<T> GetValues<T>() where T : Enum
		{
			return Enum.GetValues(typeof(T)).Cast<T>();
		}
	}
}