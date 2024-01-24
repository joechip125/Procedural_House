using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Pseudo_Random_Noise.Scripts
{
    public static partial class Noise
    {
        struct LatticeSpan4 
        {
            public int4 p0, p1;
            public float4 t;
        }
        static LatticeSpan4 GetLatticeSpan4 (float4 coordinates) 
        {
            float4 points = floor(coordinates);
            LatticeSpan4 span;
            span.p0 = (int4)points;
            span.p1 = span.p0 + 1;
            span.t = coordinates - points;
            return span;
        }
        
        public struct Lattice1D : INoise 
        {
            public float4 GetNoise4(float4x3 positions, SmallXXHash4 hash) 
            {
                int4 p0 = (int4)floor(positions.c0);
                int4 p1 = p0 + 1;
                float4 t = positions.c0 - p0;
                return lerp(hash.Eat(p0).Floats01A, hash.Eat(p1).Floats01A, t) * 2f - 1f;
            }
        }
    }
}