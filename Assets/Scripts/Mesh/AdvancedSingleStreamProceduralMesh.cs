using System.Runtime.InteropServices;
using Unity.Mathematics;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedSingleStreamProceduralMesh : MonoBehaviour
{
    
    [StructLayout(LayoutKind.Sequential)]
    struct Vertex 
    {
        public float3 position, normal;
        public half4 tangent;
        public half2 texCoord0;
    }
}
