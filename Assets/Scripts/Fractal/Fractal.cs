using System;
using System.Numerics;
using UnityEngine;
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

            var childB = CreateFractal(Vector3.up);
            var childA = CreateFractal(Vector3.right);

            childA.transform.SetParent(transform, false);
            childB.transform.SetParent(transform, false);
        }


        private Fractal CreateFractal(Vector3 direction)
        {
            var child = Instantiate(this);
            child.depth = depth - 1;
            child.transform.localPosition = 0.75f * direction;
            child.transform.localScale = 0.5f * Vector3.one;

            return child;
        }
    }
}