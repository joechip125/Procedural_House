using Unity.Mathematics;
using UnityEngine;

public class MeshHandler : MonoBehaviour
{
    [SerializeField] 
    private Mesh mesh;
    
    [SerializeField] 
    private Material material;
    
    private struct MeshPart
    {
        public float3 worldPosition;
        public Quaternion rotation;
    }
    
}
