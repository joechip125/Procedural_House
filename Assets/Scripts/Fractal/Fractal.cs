using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;

using static Unity.Mathematics.math;
using quaternion = Unity.Mathematics.quaternion;

namespace Fractal
{
    public class Fractal : MonoBehaviour
    {
        
        [BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
        struct UpdateFractalLevelJob : IJobFor 
        {
            public float spinAngleDelta;
            public float scale;

            [ReadOnly]
            public NativeArray<FractalPart> parents;

            public NativeArray<FractalPart> parts;

            [WriteOnly]
            public NativeArray<float3x4> matrices;
            

            public void Execute (int i) 
            {
                FractalPart parent = parents[i / 5];
                FractalPart part = parts[i];
                part.spinAngle += spinAngleDelta;
                part.worldRotation = mul(parent.worldRotation,
                    mul(part.rotation, quaternion.RotateY(part.spinAngle)));
                part.worldPosition = parent.worldPosition + 
                mul(parent.worldRotation, 1.5f * scale * part.direction);
                parts[i] = part;

                float3x3 r = float3x3(part.worldRotation) * scale;
                matrices[i] = float3x4(r.c0, r.c1, r.c2, part.worldPosition);
            }
        }

        
        private struct FractalPart
        {
            public Vector3 direction, worldPosition;
            public Quaternion rotation, worldRotation;
            public float spinAngle;
        }
        
        static readonly int matricesId = Shader.PropertyToID("_Matrices");
        private static MaterialPropertyBlock propertyBlock;

        [SerializeField, Range(1, 8)] 
        private int depth = 4;

        [SerializeField]
        private Mesh mesh;
        
        [SerializeField]
        private Material material;

       
        NativeArray<FractalPart>[] parts;
        NativeArray<float3x4>[] matrices;
        
        
        private ComputeBuffer[] matricesBuffers;

        static Vector3[] directions = 
        {
            Vector3.up, Vector3.right, Vector3.left, Vector3.forward, Vector3.back
        };

        static Quaternion[] rotations = 
        {
            Quaternion.identity,
            Quaternion.Euler(0f, 0f, -90f), Quaternion.Euler(0f, 0f, 90f),
            Quaternion.Euler(90f, 0f, 0f), Quaternion.Euler(-90f, 0f, 0f)
        };
        
        void Update () 
        {
            float spinAngleDelta = 0.125f * PI * Time.deltaTime;
            FractalPart rootPart = parts[0][0];
            rootPart.spinAngle += spinAngleDelta;
            rootPart.worldRotation = mul(transform.rotation,
                mul(rootPart.rotation, quaternion.RotateY(rootPart.spinAngle)));
            rootPart.worldPosition = transform.position;
            parts[0][0] = rootPart;
            
            float objectScale = transform.lossyScale.x;
            float3x3 r = float3x3(rootPart.worldRotation) * objectScale;
            matrices[0][0] = float3x4(r.c0, r.c1, r.c2, rootPart.worldPosition);

            float scale = objectScale;
            JobHandle jobHandle = default;
            for (int li = 1; li < parts.Length; li++) 
            {
                scale *= 0.5f;
                jobHandle = new UpdateFractalLevelJob 
                {
                    spinAngleDelta = spinAngleDelta,
                    scale = scale,
                    parents = parts[li - 1],
                    parts = parts[li],
                    matrices = matrices[li]
                }.ScheduleParallel(parts[li].Length, 5, jobHandle);
            }
            jobHandle.Complete();

            var bounds = new Bounds(rootPart.worldPosition, 3f * objectScale * Vector3.one);
            for (int i = 0; i < matricesBuffers.Length; i++) 
            {
                ComputeBuffer buffer = matricesBuffers[i];
                buffer.SetData(matrices[i]);
                propertyBlock.SetBuffer(matricesId, buffer);
                Graphics.DrawMeshInstancedProcedural(
                    mesh, 0, material, bounds, buffer.count, propertyBlock
                );
            }
        }

        void OnEnable () 
        {
            parts = new NativeArray<FractalPart>[depth];
            matrices = new NativeArray<float3x4>[depth];
            matricesBuffers = new ComputeBuffer[depth];
            int stride = 12 * 4;
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) {
                parts[i] = new NativeArray<FractalPart>(length, Allocator.Persistent);
                matrices[i] = new NativeArray<float3x4>(length, Allocator.Persistent);
                matricesBuffers[i] = new ComputeBuffer(length, stride);
            }

            parts[0][0] = CreatePart(0);
            for (int li = 1; li < parts.Length; li++) 
            {
                NativeArray<FractalPart> levelParts = parts[li];
                for (int fpi = 0; fpi < levelParts.Length; fpi += 5) 
                {
                    for (int ci = 0; ci < 5; ci++) 
                    {
                        levelParts[fpi + ci] = CreatePart(ci);
                    }
                }
            }

            propertyBlock ??= new MaterialPropertyBlock();
        }
        private void OnValidate()
        {
            if (parts == null || !enabled) return;
            OnDisable();
            OnEnable();
        }

        void OnDisable () 
        {
            for (int i = 0; i < matricesBuffers.Length; i++) 
            {
                matricesBuffers[i].Release();
                parts[i].Dispose();
                matrices[i].Dispose();
            }
            parts = null;
            matrices = null;
            matricesBuffers = null;
        }

        private FractalPart CreatePart(int childIndex) => new FractalPart()
        {
            direction = directions[childIndex],
            rotation = rotations[childIndex],
        };

        private FractalPart CreatePart(int levelIndex, int childIndex, float scale)
        {
            var go = new GameObject($"Fractal part L{levelIndex} C{childIndex}");
            go.transform.SetParent(transform, false);
            go.transform.localScale = Vector3.one * scale; 
            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshRenderer>().material = material;
            return new FractalPart()
            {
                direction = directions[childIndex],
                rotation = rotations[childIndex],
            };
        }
        
        private Fractal CreateChild(Vector3 direction, Quaternion rotation)
        {
            var child = Instantiate(this);
            child.depth = depth - 1;
            child.transform.localPosition = 0.75f * direction;
            child.transform.localScale = 0.5f * Vector3.one;
            child.transform.localRotation = rotation;

            return child;
        }
    }
}