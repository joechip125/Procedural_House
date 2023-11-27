﻿using UnityEngine;
using static UnityEngine.Mathf;

namespace CustomGraph
{
    public static class FunctionLibrary
    {
        public static float Wave(float x, float t) 
            => Sin(PI * (x + t));

        public static float MultiWave(float x, float t)
        {
            var y = Sin(PI * (x + t));
            y += 0.5f * Sin(2f * PI * (x + t));
            return y * (2f / 3f);
        }
        
        
        public static float Ripple (float x, float t) 
        {
            var d = Abs(x);
            var y = Sin(PI * (4f * d - t));
            return y / (1f + 10f * d);
        }
    }
}