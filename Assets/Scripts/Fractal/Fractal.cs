using System;
using UnityEngine;

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
            var child = Instantiate(this);
            child.depth = -1;
        }
    }
}