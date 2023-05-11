using UnityEngine;

namespace CCC.Runtime
{
	public static class GizmosExt
	{
		/// <summary>
		/// Draws a wireframe capsule with between a and b with the given radius.
		/// </summary>
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