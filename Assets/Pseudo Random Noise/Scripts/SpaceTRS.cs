using System;
using Unity.Mathematics;

namespace RandomNoise
{
    [Serializable]
    public struct SpaceTRS
    {
        public float3 rotation, translation, scale;
        
        public float4x4 Matrix 
        {
            get 
            {
                return float4x4.TRS(translation, quaternion.EulerZXY(math.radians(rotation)), scale);
            }
        }
    }
}