using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace RobotGame.Scripts.Math
{
    public static class MathHelpers 
    {
        public static bool IsPointInSquare(Vector3 min, Vector3 max, Vector3 point)
        {
            if (point.x < min.x || point.y < min.y || point.z < min.z) return false;
            if (point.x > max.x || point.y > max.y || point.z > max.z) return false;
        
            return true;
        }
        
        public static float4x3 TransformVectors (this float3x4 trs, float4x3 p, float w = 1f) 
            => float4x3(
                trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x * w,
                trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y * w,
                trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z * w
            );
        
        public static float3x4 Get3x4 (this float4x4 m) =>
            float3x4(m.c0.xyz, m.c1.xyz, m.c2.xyz, m.c3.xyz);

        public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
        {
            float Func(float x) => -4 * height * x * x + 4 * height * x;
            var mid = Vector3.Lerp(start, end, t);
            return new Vector3(mid.x, Func(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
        }
    
        public static Vector2 WhereIsPoint(Vector3 min, Vector3 max, Vector3 point)
        {
            var outVec = new Vector2();
        
            var calc = point.x + point.z;
            var calc2 = min.x + min.z;
            var calc3 = max.x + max.z;
        
            if (point.y < min.y) outVec.y = -1;
            else if (point.y >= min.y) outVec.x = point.y < max.y ? 0 : 1;
        
            if (calc < calc2) outVec.x = -1;
            else if (calc >= calc2) outVec.x = calc < calc3 ? 0 : 1;
            return outVec;
        }
    
        public static void PlaneDirections(Vector3 normal, out Vector3 planeUp, out Vector3 planeRight)
        {
            planeUp = Vector3.ProjectOnPlane(normal.x + normal.z == 0 
                ? new Vector2(1,0) : new Vector2(0,1), normal);
            planeRight = Vector3.Cross(normal, planeUp);
        }
    }
}
