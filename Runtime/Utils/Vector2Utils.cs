using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	/// <summary>
	/// Extension methods for Vector2, providing utility functions for vector operations,
	/// coordinate system transformations, and geometric calculations.
	/// </summary>
	public static class Vector2Utils
	{
		/// <summary>
		/// Embeds the given 2D vector into the XZ subspace of ℝ³ by setting the Y component to zero.
		/// Maps the X and Y components of the Vector2 to X and Z components of the resulting Vector3.
		/// </summary>
		/// <param name="v">The 2D vector to embed.</param>
		/// <returns>A new Vector3 with X and Y components mapped to X and Z, and Y set to zero.</returns>
		[Pure]
		public static Vector3 ToVector3XZ(this Vector2 v) => new Vector3(v.x, 0, v.y);

		/// <summary>
		/// Switches the components of the vector v=(x,y) to (y,x), effectively reflecting it through the line y=x.
		/// This is equivalent to a 90-degree rotation followed by a reflection.
		/// </summary>
		/// <param name="v">The 2D vector to switch.</param>
		/// <returns>A new Vector2 with swapped X and Y components.</returns>
		[Pure]
		public static Vector2 Switch(this Vector2 v) => new Vector2(v.y, v.x);

		/// <summary>
		/// Rotates the vector v by the given angle in degrees around the origin in a clockwise direction.
		/// </summary>
		/// <param name="v">The 2D vector to rotate.</param>
		/// <param name="degrees">The rotation angle in degrees (positive for clockwise).</param>
		/// <returns>A new Vector2 representing the rotated vector.</returns>
		[Pure]
		public static Vector2 Rotate(this Vector2 v, float degrees) => v.RotateRad(degrees * Mathf.Deg2Rad);

		/// <summary>
		/// Rotates the vector v by the given angle in radians around the origin in a clockwise direction.
		/// Uses the standard 2D rotation matrix transformation.
		/// </summary>
		/// <param name="v">The 2D vector to rotate.</param>
		/// <param name="rad">The rotation angle in radians (positive for clockwise).</param>
		/// <returns>A new Vector2 representing the rotated vector.</returns>
		[Pure]
		public static Vector2 RotateRad(this Vector2 v, float rad)
		{
			var sinRad = Mathf.Sin(rad);
			var cosRad = Mathf.Cos(rad);
			return new Vector2(cosRad * v.x - sinRad * v.y, sinRad * v.x + cosRad * v.y);
		}
	}
}