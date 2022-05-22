using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum VectorTypes
{
    Normal, InvertNormal, Forward, Back
}

public class AdvancedMesh : MonoBehaviour
{
    protected Mesh theMesh;
    MeshCollider meshCollider;
    [NonSerialized] List<Vector3> vertices = new ();
    [NonSerialized] private List<int> triangles = new ();
    
    
    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = theMesh = new Mesh();
        theMesh.name = "TheMesh";
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

    protected void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
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
        
        UpdateMesh();
    }

    private void UpdateMesh()
    {
        theMesh.Clear();
        theMesh.SetVertices(vertices);
        theMesh.SetTriangles(triangles, 0);
        theMesh.RecalculateNormals();
    }


    public void ClearMesh()
    {
        vertices.Clear();
        triangles.Clear();
        theMesh.SetVertices(vertices);
        theMesh.SetTriangles(triangles, 0);
    }


    public void MoveVertices(Dictionary<int, Vector3> pointsAndMove)
    {
        foreach (var p in pointsAndMove)
        {
            vertices[p.Key] += p.Value;
        }
        UpdateMesh();
    }

    public void MoveVertices(Vector3 moveAmount, List<int> indexes)
    {
        foreach (var i in indexes)
        {
            vertices[i] += moveAmount;
        }
        
        UpdateMesh();
    }

    protected void MoveTwoVerts(int indexOne, int indexTwo, Vector3 moveAmount)
    {
        vertices[indexOne] += moveAmount;
        vertices[indexTwo] += moveAmount;
        
        UpdateMesh();
    }

    protected void SetTwoVerts(int indexOne, int indexTwo, Vector3 setVector, float offset)
    {
        
        vertices[indexOne] = setVector;
        vertices[indexTwo] = setVector + new Vector3(0, offset, 0);
        
        UpdateMesh();
    }

    protected void AddTriangle(Vector3 v1,Vector3 v2, Vector3 v3)
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

    protected int AddTriangleWithPointList(List<Vector3> pointList)
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
