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
            
            public static JobHandle ScheduleParallel 
                (NativeArray<float3> positions, int resolution, float4x4 trs,JobHandle dependency) 
            {
                return new Job 
                {
                    positions = positions,
                    resolution = resolution,
                    invResolution = 1f / resolution,
                    positionTRS = float3x4(trs.c0.xyz, trs.c1.xyz, trs.c2.xyz, trs.c3.xyz)
                }.ScheduleParallel(positions.Length, resolution, dependency);
            }

            [WriteOnly]
            NativeArray<float3> positions;

            public float resolution, invResolution;
            
            public float3x4 positionTRS;

            public void Execute (int i) 
            {
                float2 uv;
                uv.y = floor(invResolution * i + 0.00001f);
                uv.x = invResolution * (i - resolution * uv.y + 0.5f) - 0.5f;
                uv.y = invResolution * (uv.y + 0.5f) - 0.5f;
                
                positions[i] = mul(positionTRS, float4(uv.x, 0f, uv.y, 1f));
            }
        }
    }
}