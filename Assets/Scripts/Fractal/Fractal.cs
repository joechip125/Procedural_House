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
        [SerializeField, Range(1, 8)] 
        private int depth = 4;

        private void Start()
        {
            name = $"Fractal {depth}";
            if (depth <= 1) return;

            var childB = CreateChild(Vector3.up, Quaternion.identity);
            var childA = CreateChild(Vector3.right, Quaternion.Euler(0,0, -90f));
            var childC = CreateChild(Vector3.left, Quaternion.Euler(0f, 0f, 90f));
            var childD = CreateChild(Vector3.forward, Quaternion.Euler(90f, 0f, 0f));
            var childE = CreateChild(Vector3.back, Quaternion.Euler(-90f, 0f, 0f));

            childA.transform.SetParent(transform, false);
            childB.transform.SetParent(transform, false);
            childC.transform.SetParent(transform, false);
            childD.transform.SetParent(transform, false);
            childE.transform.SetParent(transform, false);
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