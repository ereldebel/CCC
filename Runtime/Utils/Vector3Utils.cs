using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	public static class Vector3Utils
	{
		/// <summary>
		/// Projects the given vector to the XZ Subspace of ℝ³.
		/// </summary>
		[Pure]
		public static Vector3 ToVector3XZ(this Vector3 v) => new Vector3(v.x, 0, v.z);

		/// <summary>
		/// Projects the given vector to the XZ Subspace of ℝ³ and Convert it to ℝ².
		/// </summary>
		[Pure]
		public static Vector2 ToVector2XZ(this Vector3 v) => new Vector2(v.x, v.z);
	}
}