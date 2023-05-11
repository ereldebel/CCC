using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	public static class Vector2IntUtils
	{
		/// <summary>
		/// Calculates the L₁ norm of v=(x,y) defined by |x|+|y|.
		/// </summary>
		[Pure]
		public static int L1Norm(this Vector2Int v) => Mathf.Abs(v.x) + Mathf.Abs(v.y);

		/// <summary>
		/// Calculates the L₁ distance between a=(x₁,y₁) and b=(x₂,y₂) defined by |x₁-x₂|+|y₁-y₂|.
		/// </summary>
		[Pure]
		public static int L1Distance(Vector2Int a, Vector2Int b) => (a - b).L1Norm();
	}
}