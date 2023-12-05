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
            public Vector3 direction;
            public Quaternion rotation;
        }
        
        [SerializeField, Range(1, 8)] 
        private int depth = 4;

        [SerializeField]
        private Mesh mesh;
        
        [SerializeField]
        private Material material;
        
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

        private void CreatePart()
        {
            var go = new GameObject($"Fractal part {depth}");
            go.transform.SetParent(transform, false);
            go.AddComponent<MeshFilter>().mesh = mesh;
            go.AddComponent<MeshRenderer>().material = material;
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