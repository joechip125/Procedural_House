using System;
using UnityEngine;
using static UnityEngine.Mathf;

namespace CustomGraph
{
    public static class FunctionLibrary
    {
        public delegate float Function(float x, float z, float t);

        private static Function[] functions = {Wave, MultiWave, Ripple};
        public enum FunctionName {Wave, MultiWave, Ripple}

        public static Function GetFunction(FunctionName index)
        {
            return functions[(int)index];
        }

        private static float Wave(float x,float z, float t) 
            => Sin(PI * (x + t));

        private static float MultiWave(float x,float z, float t)
        {
            var y = Sin(PI * (x + t));
            y += 0.5f * Sin(2f * PI * (x + t));
            return y * (2f / 3f);
        }

        private static float Ripple (float x,float z, float t) 
        {
            var d = Abs(x);
            var y = Sin(PI * (4f * d - t));
            return y / (1f + 10f * d);
        }
    }
}