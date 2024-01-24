using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Pseudo_Random_Noise.Scripts
{
    public static partial class Noise
    {
        public struct Lattice1D : INoise 
        {

            public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) 
            {
                int4 p0 = (int4)floor(positions.c0);
                int4 p1 = p0 + 1;
                float4 v = (uint4)hash.Eat(p1) & 255;
                return v * (2f / 255f) - 1f;
            }
        }
    }
}