using UnityEngine;

namespace CRL.Utils
{
    public static class Vector3Utils
    {
        public static Vector2 ToVector2XZ(this Vector3 v) => new Vector2(v.x, v.z);
    }
}