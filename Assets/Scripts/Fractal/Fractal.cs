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
            if (depth < 1) return;
            
            var child = Instantiate(this, transform,false);
            child.transform.localPosition = 0.75f * Vector3.right;
            child.transform.localScale = 0.5f * Vector3.one;
            child.depth = -1;
            
            child = Instantiate(this);
            child.depth = depth - 1;
            child.transform.SetParent(transform, false);
            child.transform.localPosition = 0.75f * Vector3.up;
            child.transform.localScale = 0.5f* Vector3.one;
        }
    }
}