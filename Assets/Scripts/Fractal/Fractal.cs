using System;
using System.Numerics;
using Unity.Mathematics;
using UnityEngine;
using Quaternion = UnityEngine.Quaternion;
using Vector3 = UnityEngine.Vector3;

namespace Fractal
{
    public class Fractal : MonoBehaviour
    {
        private struct FractalPart
        {
            public Vector3 direction, worldDirection;
            public Quaternion rotation, worldRotation;
        }
        
        [SerializeField, Range(1, 8)] 
        private int depth = 4;

        [SerializeField]
        private Mesh mesh;
        
        [SerializeField]
        private Material material;

        private FractalPart[][] parts;

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
            for (int li = 1; li < parts.Length; li++)
            {
                var parentParts = parts[li - 1];
                var levelParts = parts[li];
                for (int fpi = 0; fpi < levelParts.Length; fpi++)
                {
                    var parentTransform = parentParts[fpi / 5].transform;
                    var part = levelParts[fpi];
                    part.transform.localRotation = parentTransform.localRotation * part.rotation;
                    part.transform.localPosition = parentTransform.localRotation *
                                                   (1.5f * part.transform.localScale.x * part.direction);
                }
            }
        }

        private void Awake()
        {
            parts = new FractalPart[depth][];
            parts[0] = new FractalPart[1];
            
            for (int i = 0, length = 1; i < parts.Length; i++, length *= 5) 
            {
                parts[i] = new FractalPart[length];
            }

            var scale = 1f;
            parts[0][0] = CreatePart(0,0, scale);
            for (int li = 1; li < parts.Length; li++)
            {
                scale *= 0.5f;
                var levelParts = parts[li];
                for (int fpi = 0; fpi < levelParts.Length; fpi += 5)
                {
                    for (int c = 0; c < 5; c++)
                    {
                        levelParts[fpi + c]=CreatePart(li,c, scale);
                    }
                }
            }
        }

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