using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	/// <summary>
	/// Extension methods for Vector3Int, providing utility functions for integer vector operations
	/// and distance calculations using the L₁ (Manhattan) norm.
	/// </summary>
	public static class Vector3IntUtils
	{
		/// <summary>
		/// Calculates the L₁ norm (Manhattan norm) of the vector v=(x,y,z) defined by |x|+|y|+|z|.
		/// This represents the sum of the absolute values of all components.
		/// </summary>
		/// <param name="v">The 3D integer vector.</param>
		/// <returns>The L₁ norm as an integer.</returns>
		[Pure]
		public static int L1Norm(this Vector3Int v) => Mathf.Abs(v.x) + Mathf.Abs(v.y) + Mathf.Abs(v.z);

		/// <summary>
		/// Calculates the L₁ distance (Manhattan distance) between two points a=(x₁,y₁,z₁) and b=(x₂,y₂,z₂)
		/// defined by |x₁-x₂|+|y₁-y₂|+|z₁-z₂|. This represents the sum of absolute differences in each dimension.
		/// </summary>
		/// <param name="a">The first 3D integer vector.</param>
		/// <param name="b">The second 3D integer vector.</param>
		/// <returns>The L₁ distance between the two points as an integer.</returns>
		[Pure]
		public static int L1Distance(Vector3Int a, Vector3Int b) => (a - b).L1Norm();
	}
}