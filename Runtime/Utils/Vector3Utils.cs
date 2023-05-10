using System.Diagnostics.Contracts;
using UnityEngine;

namespace CCC.Runtime.Utils
{
	public static class Vector3Utils
	{
		[Pure]
		public static Vector2 ToVector2XZ(this Vector3 v) => new Vector2(v.x, v.z);
	}
}