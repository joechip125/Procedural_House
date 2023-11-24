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
    }
}