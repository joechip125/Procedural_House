using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;

namespace RandomNoise
{
    public class HashVisualization : MonoBehaviour
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        struct HashJob : IJobFor
        {

            [WriteOnly] 
            public NativeArray<uint> hashes;

            
            
            public void Execute(int i)
            {
                hashes[i] = (uint)i;
            }
            
        }
        
        static int hashesId = Shader.PropertyToID("_Hashes"),
            configId = Shader.PropertyToID("_Config");

        [SerializeField]
        Mesh instanceMesh;

        [SerializeField]
        Material material;

        [SerializeField, Range(1, 512)]
        int resolution = 16;

        NativeArray<uint> hashes;

        ComputeBuffer hashesBuffer;

        MaterialPropertyBlock propertyBlock;
    }
}