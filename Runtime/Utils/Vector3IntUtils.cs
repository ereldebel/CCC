using UnityEngine;

namespace CCC.Runtime.Utils
{
    public static class Vector3IntUtils
    {
        public static int L1Norm(this Vector3Int v) => Mathf.Abs(v.x) + Mathf.Abs(v.y);

        public static int L1Distance(Vector3Int a, Vector3Int b) => (a - b).L1Norm();
    }
}