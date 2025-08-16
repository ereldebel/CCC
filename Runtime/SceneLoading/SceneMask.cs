using System.Collections;
using System.Collections.Generic;
using System.Linq;
using CCC.Runtime.Utils;

namespace CCC.Runtime.SceneLoading
{
	/// <summary>
	/// A mask of scene types.
	/// </summary>
	public readonly struct SceneMask : IEnumerable<SceneType>
	{
		#region Private Fields

		private readonly int _mask;

		#endregion

		#region Constructors

		public SceneMask(int mask) => _mask = mask;

		public SceneMask(params SceneType[] types) =>
			_mask = types.Aggregate(0, (mask, type) => mask | (1 << (int)type));

		public static SceneMask InverseMask(SceneType type) =>
			new(~(1 << (int)type));

		public static SceneMask InverseMask(params SceneType[] types) =>
			new(~types.Aggregate(0, (mask, type) => mask | (1 << (int)type)));

		#endregion

		#region Operator Overloads

		public static explicit operator SceneMask(int mask) => new(mask);

		public static explicit operator SceneMask(SceneType type) => new(type);

		public static SceneMask operator &(SceneMask lhs, SceneMask rhs) => new(lhs._mask & rhs._mask);

		public static SceneMask operator |(SceneMask lhs, SceneMask rhs) => new(lhs._mask | rhs._mask);

		#endregion

		#region Public Methods

		/// <summary>
		/// Returns an enumerator that iterates through the scene types in this mask.
		/// </summary>
		/// <returns>An enumerator for the scene types.</returns>
		public IEnumerator<SceneType> GetEnumerator()
		{
			int bit = 1;
			for (int type = 0; type < EnumUtils.Count<SceneType>(); ++type, bit <<= 1)
				if ((_mask & bit) != 0)
					yield return (SceneType)type;
		}

		/// <summary>
		/// Returns a non-generic enumerator that iterates through the scene types in this mask.
		/// </summary>
		/// <returns>A non-generic enumerator for the scene types.</returns>
		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

		#endregion
	}
}