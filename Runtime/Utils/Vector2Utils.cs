using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	public static class Vector2Utils
	{
		/// <summary>
		/// Embeds the given vector to the XZ Subspace of ℝ³ .
		/// </summary>
		[Pure]
		public static Vector3 ToVector3XZ(this Vector2 v) => new Vector3(v.x, 0, v.y);

		/// <summary>
		/// Switch the components of v=(x,y) to (y,x). The same as reflecting through y=x.
		/// </summary>
		[Pure]
		public static Vector2 Switch(this Vector2 v) => new Vector2(v.y, v.x);

		/// <summary>
		/// Rotates the vector v the given angle (in degrees) around the origin in a clockwise direction.
		/// </summary>
		[Pure]
		public static Vector2 Rotate(this Vector2 v, float degrees) => v.RotateRad(degrees * Mathf.Deg2Rad);

		/// <summary>
		/// Rotates the vector v the given angle (in radians) around the origin in a clockwise direction.
		/// </summary>
		[Pure]
		public static Vector2 RotateRad(this Vector2 v, float rad)
		{
			var sinRad = Mathf.Sin(rad);
			var cosRad = Mathf.Cos(rad);
			return new Vector2(cosRad * v.x - sinRad * v.y, sinRad * v.x + cosRad * v.y);
		}
	}
}