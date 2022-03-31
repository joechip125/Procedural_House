using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMesh : MonoBehaviour
{
    protected Mesh theMesh;
    MeshCollider meshCollider;
    [NonSerialized] List<Vector3> vertices = new ();
    [NonSerialized] private List<int> triangles = new ();
    
    
    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = theMesh = new Mesh();
        theMesh.name = "RoomMesh";
    }

    public void ApplyMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
    }
    
    protected int AddQuadWithPointList(List<Vector3> pointList)
    {
        var vertexIndex = vertices.Count;
        
        vertices.Add(pointList[0]);
        vertices.Add(pointList[1]);
        vertices.Add(pointList[2]);
        vertices.Add(pointList[3]);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
        UpdateMesh();

        return vertexIndex;
    }
    
    private void UpdateMesh()
    {
        theMesh.Clear();
        theMesh.SetVertices(vertices);
        theMesh.SetTriangles(triangles, 0);
        theMesh.RecalculateNormals();
    }
    
}
