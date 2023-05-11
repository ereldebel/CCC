using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	public static class Vector3IntUtils
	{
		/// <summary>
		/// Calculates the L₁ norm of v=(x,y,z) defined by |x|+|y|+|z|.
		/// </summary>
		[Pure]
		public static int L1Norm(this Vector3Int v) => Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);

		/// <summary>
		/// Calculates the L₁ distance between a=(x₁,y₁,z₁) and b=(x₂,y₂,z₂) defined by |x₁-x₂|+|y₁-y₂|+|z₁-z₂|.
		/// </summary>
		[Pure]
		public static int L1Distance(Vector3Int a, Vector3Int b) => (a - b).L1Norm();
	}
}