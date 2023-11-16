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
    }
}
