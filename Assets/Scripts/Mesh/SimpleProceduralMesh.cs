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
            Vector3.zero, Vector3.right, Vector3.up
        };

        mesh.triangles = new int[] 
        {
            0, 1, 2
        };
        
        mesh.normals = new Vector3[] 
        {
            Vector3.back, Vector3.back, Vector3.back
        };
    }
}
