using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class SimpleProceduralMesh : MonoBehaviour
{
    void OnEnable () 
    {
        var mesh = new Mesh 
        {
            name = "Procedural Mesh"
        };
        GetComponent<MeshFilter>().mesh = mesh;
        
        mesh.vertices = new Vector3[] 
        {
            Vector3.zero, Vector3.right, Vector3.up, new Vector3(1f, 1f)
        };

        mesh.triangles = new int[] 
        {
            0, 1, 2,1,3,2
        };
        
        mesh.normals = new Vector3[] 
        {
            Vector3.back, Vector3.back, Vector3.back, Vector3.back
        };
        
        mesh.uv = new Vector2[] 
        {
            Vector2.zero, Vector2.right, Vector2.up, Vector2.one
        };

        
        mesh.tangents = new Vector4[] 
        {
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f),
            new Vector4(1f, 0f, 0f, -1f)
        };
    }
}
