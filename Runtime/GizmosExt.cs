using UnityEngine;

namespace CCC.Runtime
{
	/// <summary>
	/// Extension methods for Unity's Gizmos class, providing additional drawing utilities
	/// for debugging and visualization purposes in the Scene view.
	/// </summary>
	public static class GizmosExt
	{
		/// <summary>
		/// Draws a wireframe capsule between two points with the specified radius.
		/// Creates a visual representation of a capsule shape using spheres and connecting lines.
		/// </summary>
		/// <param name="a">The start point of the capsule.</param>
		/// <param name="b">The end point of the capsule.</param>
		/// <param name="radius">The radius of the capsule.</param>
		public static void DrawWireCapsule(Vector3 a, Vector3 b, float radius)
		{
			Gizmos.DrawWireSphere(a, radius);
			Gizmos.DrawWireSphere(b, radius);
			var dir = (b - a).normalized;
			var normal = Vector3.Cross(dir, Vector3.up);
			if (normal == Vector3.zero)
				normal = Vector3.Cross(dir, Vector3.right);
			var binormal = Vector3.Cross(dir, normal);
			normal = normal.normalized * radius;
			binormal = binormal.normalized * radius;
			Gizmos.DrawLine(a + normal, b + normal);
			Gizmos.DrawLine(a - normal, b - normal);
			Gizmos.DrawLine(a + binormal, b + binormal);
			Gizmos.DrawLine(a - binormal, b - binormal);
		}
	}
}