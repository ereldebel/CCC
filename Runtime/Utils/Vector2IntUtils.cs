using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	/// <summary>
	/// Extension methods for Vector2Int, providing utility functions for integer vector operations
	/// and distance calculations using the L₁ (Manhattan) norm.
	/// </summary>
	public static class Vector2IntUtils
	{
		/// <summary>
		/// Calculates the L₁ norm (Manhattan norm) of the vector v=(x,y) defined by |x|+|y|.
		/// This represents the sum of the absolute values of both components.
		/// </summary>
		/// <param name="v">The 2D integer vector.</param>
		/// <returns>The L₁ norm as an integer.</returns>
		[Pure]
		public static int L1Norm(this Vector2Int v) => Mathf.Abs(v.x) + Mathf.Abs(v.y);

		/// <summary>
		/// Calculates the L₁ distance (Manhattan distance) between two points a=(x₁,y₁) and b=(x₂,y₂)
		/// defined by |x₁-x₂|+|y₁-y₂|. This represents the sum of absolute differences in each dimension.
		/// </summary>
		/// <param name="a">The first 2D integer vector.</param>
		/// <param name="b">The second 2D integer vector.</param>
		/// <returns>The L₁ distance between the two points as an integer.</returns>
		[Pure]
		public static int L1Distance(Vector2Int a, Vector2Int b) => (a - b).L1Norm();
	}
}