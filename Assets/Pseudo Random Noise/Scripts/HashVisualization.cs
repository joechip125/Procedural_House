using System;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using Unity.Mathematics;
using static Unity.Mathematics.math;

namespace RandomNoise
{
    public class HashVisualization : MonoBehaviour
    {
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        struct HashJob : IJobFor
        {
            [WriteOnly] 
            public NativeArray<uint> hashes;
            
            public int resolution;
            public float invResolution;

            public void Execute(int i)
            {
                float v = floor(invResolution * i + 0.00001f);
                float u = i - resolution * v;
                hashes[i] = (uint)(frac(u * v * 0.381f) * 255f);
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

        void OnEnable () 
        {
            int length = resolution * resolution;
            hashes = new NativeArray<uint>(length, Allocator.Persistent);
            hashesBuffer = new ComputeBuffer(length, 4);

            new HashJob 
            {
                hashes = hashes,
                resolution = resolution,
                invResolution = 1f / resolution
            }.ScheduleParallel(hashes.Length, resolution, default).Complete();

            hashesBuffer.SetData(hashes);

            propertyBlock ??= new MaterialPropertyBlock();
            propertyBlock.SetBuffer(hashesId, hashesBuffer);
            propertyBlock.SetVector(configId, new Vector4(resolution, 1f / resolution));
        }
        
        void OnDisable () 
        {
            hashes.Dispose();
            hashesBuffer.Release();
            hashesBuffer = null;
        }

        void OnValidate () 
        {
            if (hashesBuffer != null && enabled)
            {
                OnDisable();
                OnEnable();
            }
        }
        void Update () 
        {
            Graphics.DrawMeshInstancedProcedural
            (instanceMesh, 0, material, new Bounds(Vector3.zero, Vector3.one),
                hashes.Length, propertyBlock);
        }
    }
}