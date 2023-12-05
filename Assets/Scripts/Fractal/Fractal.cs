using UnityEngine;

namespace Fractal
{
    public class Fractal : MonoBehaviour
    {
        [SerializeField, Range(1, 8)] 
        private int depth = 4;
    }
}