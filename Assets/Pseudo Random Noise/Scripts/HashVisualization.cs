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
            [ReadOnly]
            public NativeArray<float3> positions;
            
            [WriteOnly] 
            public NativeArray<uint> hashes;
            
            public SmallXXHash hash;
            
            public float3x4 domainTRS;
            
            public void Execute(int i)
            {
                float3 p = mul(domainTRS, float4(positions[i], 1f));

                int u = (int)floor(p.x);
                int v = (int)floor(p.y);
                int w = (int)floor(p.z);
                
                hashes[i] = hash.Eat(u).Eat(v).Eat(w);
            }
        }

        static int hashesId = Shader.PropertyToID("_Hashes"),
            positionsId = Shader.PropertyToID("_Positions"),
            configId = Shader.PropertyToID("_Config");

        [SerializeField]
        Mesh instanceMesh;
        [SerializeField]
        Material material;
        [SerializeField, Range(1, 512)]
        int resolution = 16;

        [SerializeField]
        private int seed;
        
        [SerializeField, Range(-2f, 2f)]
        float verticalOffset = 1f;
        
        [SerializeField]
        SpaceTRS domain = new SpaceTRS()
        {
            scale = 8
        };

        NativeArray<uint> hashes;
        NativeArray<float3> positions;
        
        ComputeBuffer hashesBuffer, positionsBuffer;
        
        MaterialPropertyBlock propertyBlock;

        void OnEnable () 
        {
            int length = resolution * resolution;
            hashes = new NativeArray<uint>(length, Allocator.Persistent);
            positions = new NativeArray<float3>(length, Allocator.Persistent);
            hashesBuffer = new ComputeBuffer(length, 4);
            positionsBuffer = new ComputeBuffer(length, 3 * 4);
            
            JobHandle handle = Shapes.Job.ScheduleParallel(positions, resolution, transform.localToWorldMatrix,default);

            new HashJob 
            {
                hashes = hashes,
                positions = positions,
                hash = SmallXXHash.Seed(seed),
                domainTRS = domain.Matrix
            }.ScheduleParallel(hashes.Length, resolution, handle).Complete();

            hashesBuffer.SetData(hashes);
            positionsBuffer.SetData(positions);

            propertyBlock ??= new MaterialPropertyBlock();
            propertyBlock.SetBuffer(hashesId, hashesBuffer);
            propertyBlock.SetBuffer(positionsId, positionsBuffer);
            
            propertyBlock.SetVector(configId, new Vector4(resolution, 1f / resolution, verticalOffset / resolution));
        }
        
        void OnDisable () 
        {
            hashes.Dispose();
            positions.Dispose();
            hashesBuffer.Release();
            positionsBuffer.Release();
            hashesBuffer = null;
            positionsBuffer = null;
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