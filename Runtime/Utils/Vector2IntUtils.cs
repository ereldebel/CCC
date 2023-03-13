using UnityEngine;

namespace CCC.Runtime.Utils
{
    public static class Vector2IntUtils
    {
        public static int L1Norm(this Vector2Int v) => Mathf.Abs(v.x) + Mathf.Abs(v.y);

        public static int L1Distance(Vector2Int a, Vector2Int b) => (a - b).L1Norm();
    }
}