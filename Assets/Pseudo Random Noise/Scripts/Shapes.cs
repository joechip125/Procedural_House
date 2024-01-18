using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace RandomNoise
{
    public static class Shapes
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        public struct Job : IJobFor 
        {

            [WriteOnly]
            NativeArray<float3> positions;

            public float resolution, invResolution;

            public void Execute (int i) 
            {
                float2 uv;
                uv.y = floor(invResolution * i + 0.00001f);
                uv.x = invResolution * (i - resolution * uv.y + 0.5f) - 0.5f;
                uv.y = invResolution * (uv.y + 0.5f) - 0.5f;

                positions[i] = float3(uv.x, 0f, uv.y);
            }
        }
    }
}