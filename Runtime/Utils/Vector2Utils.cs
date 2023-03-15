using UnityEngine;

namespace CCC.Runtime.Utils
{
    public static class Vector2Utils
    {
        public static Vector3 ToVector3XZ(this Vector2 v) => new Vector3(v.x, 0, v.y);

        public static Vector2 Switch(this Vector2 v) => new Vector2(v.y, v.x);

        public static Vector2 Rotate(this Vector2 v, float degrees) => v.RotateRad(degrees * Mathf.Deg2Rad);

        public static Vector2 RotateRad(this Vector2 v, float rad)
        {
            var sinRad = Mathf.Sin(rad); 
            var cosRad = Mathf.Cos(rad);
            return new Vector2(cosRad * v.x - sinRad * v.y, sinRad * v.x + cosRad * v.y);
        }
    }
}