using System;
using System.Numerics;
using Unity.Mathematics;
using Unity.VisualScripting.Dependencies.NCalc;
using UnityEngine;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Fractal
{
    public class Fractal : MonoBehaviour
    {
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

        private FractalPart[][] parts;
        private Matrix4x4[][] matrices;
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
            
        }

        private void OnEnable()
        {
            parts = new FractalPart[depth][];
            parts[0] = new FractalPart[1];
            matrices = new Matrix4x4[depth][];
            matricesBuffers = new ComputeBuffer[depth];
            int stride = 16 * 4;

            propertyBlock ??= new MaterialPropertyBlock();
            
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) 
            {
                parts[i] = new FractalPart[length];
                matrices[i] = new Matrix4x4[length];
                matricesBuffers[i] = new ComputeBuffer(length, stride);
            }
            
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