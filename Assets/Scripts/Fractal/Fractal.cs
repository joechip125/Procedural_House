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
            
           var childA = CreateFractal(Vector3.right);
           var childB = CreateFractal(Vector3.up);
            
        }


        private Fractal CreateFractal(Vector3 direction)
        {
            var child = Instantiate(this, transform,false);
            child.transform.localPosition = 0.75f * direction;
            child.transform.localScale = 0.5f * Vector3.one;
            child.depth = -1;

            return child;
        }
    }
}