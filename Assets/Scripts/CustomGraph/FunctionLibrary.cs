using System;
using UnityEngine;
using static UnityEngine.Mathf;

namespace CustomGraph
{
    public static class FunctionLibrary
    {
        //public delegate float Function(float x, float z, float t);
        public delegate Vector3 Function (float u, float v, float t);

        private static Function[] functions = {Wave, MultiWave, Ripple};
        public enum FunctionName {Wave, MultiWave, Ripple}

        public static Function GetFunction(FunctionName index)
        {
            return functions[(int)index];
        }

        public static Vector3 Wave (float u, float v, float t) 
        {
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (u + v + t));
            p.z = v;
            return p;
        }


        private static Vector3 MultiWave (float u, float v, float t) 
        {
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (u + 0.5f * t));
            p.y += 0.5f * Sin(2f * PI * (v + t));
            p.y += Sin(PI * (u + v + 0.25f * t));
            p.y *= 1f / 2.5f;
            p.z = v;
            return p;
        }

        private static Vector3 Ripple (float u, float v, float t) 
        {
            float d = Sqrt(u * u + v * v);
            Vector3 p;
            p.x = u;
            p.y = Sin(PI * (4f * d - t));
            p.y /= 1f + 10f * d;
            p.z = v;
            return p;
        }
    }
}