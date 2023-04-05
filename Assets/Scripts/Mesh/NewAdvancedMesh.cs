using System;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class NewAdvancedMesh : MonoBehaviour
{
    protected Mesh TheMesh;
    MeshCollider meshCollider;
    [NonSerialized] protected List<Vector3> Vertices = new ();
    [NonSerialized] protected List<int> Triangles = new ();

    protected virtual void InitMesh()
    {
        GetComponent<MeshFilter>().mesh = TheMesh = new Mesh();
        TheMesh.name = "TheMesh";
    }
    
    protected void UpdateMesh()
    {
        TheMesh.Clear();
        TheMesh.SetVertices(Vertices);
        TheMesh.SetTriangles(Triangles, 0);
        TheMesh.RecalculateNormals();
    }
    
    public void AddCollider()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }
}
