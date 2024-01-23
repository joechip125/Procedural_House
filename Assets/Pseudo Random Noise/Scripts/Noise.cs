using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Pseudo_Random_Noise.Scripts
{
    public static class Noise
    {
        public interface INoise 
        {
            float4 GetNoise4 (float4x3 positions, SmallXXHash4 hash);
        }

        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        public struct Job<N> : IJobFor where N : struct, INoise
        {
            public void Execute(int index)
            {
                
            }
        }
    }
    
}