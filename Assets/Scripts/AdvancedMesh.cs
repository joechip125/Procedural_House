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
    
    
    private void AddTriangle(Vector3 v1,Vector3 v2, Vector3 v3)
    {
        var vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        
        UpdateMesh();
    }
    
    private int AddTriangleWithPointList(List<Vector3> pointList)
    {
        var vertexIndex = vertices.Count;
        vertices.Add(pointList[0]);
        vertices.Add(pointList[1]);
        vertices.Add(pointList[2]);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        
        UpdateMesh();

        return vertexIndex;
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
    {
        var vertexIndex = vertices.Count;
        vertices.Add(v1);
        vertices.Add(v2);
        vertices.Add(v3);
        vertices.Add(v4);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
        triangles.Add(vertexIndex + 3);
    }
    

    private void RemoveQuad(int firstIndex)
    {

        var tris = triangles.FindAll(x => x == firstIndex);
        triangles.RemoveAt(firstIndex + 5);
        triangles.RemoveAt(firstIndex + 4);
        triangles.RemoveAt(firstIndex + 3);
        triangles.RemoveAt(firstIndex + 2);
        triangles.RemoveAt(firstIndex + 1);
        triangles.RemoveAt(firstIndex);
        UpdateMesh();
    }
    
}
