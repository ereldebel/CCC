using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	/// <summary>
	/// Extension methods for Vector3, providing utility functions for vector operations
	/// and coordinate system transformations.
	/// </summary>
	public static class Vector3Utils
	{
		/// <summary>
		/// Projects the given vector to the XZ subspace of ℝ³ by setting the Y component to zero.
		/// Useful for 2D-style movement in a 3D world where Y represents height.
		/// </summary>
		/// <param name="v">The 3D vector to project.</param>
		/// <returns>A new Vector3 with the Y component set to zero.</returns>
		[Pure]
		public static Vector3 ToVector3XZ(this Vector3 v) => new Vector3(v.x, 0, v.z);

		/// <summary>
		/// Projects the given vector to the XZ subspace of ℝ³ and converts it to ℝ².
		/// Maps the X and Z components to X and Y respectively in the resulting Vector2.
		/// </summary>
		/// <param name="v">The 3D vector to project and convert.</param>
		/// <returns>A new Vector2 with X and Z components mapped to X and Y.</returns>
		[Pure]
		public static Vector2 ToVector2XZ(this Vector3 v) => new Vector2(v.x, v.z);
	}
}