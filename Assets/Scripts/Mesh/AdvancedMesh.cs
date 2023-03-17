using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum VectorTypes
{
    Normal, InvertNormal, Forward, Back
}

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AdvancedMesh : MonoBehaviour
{
    protected Mesh TheMesh;
    MeshCollider meshCollider;
    [NonSerialized] protected List<Vector3> Vertices = new ();
    [NonSerialized] protected List<int> Triangles = new ();
    
    
    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = TheMesh = new Mesh();
        TheMesh.name = "TheMesh";
    }

    public void GetPanels()
    {
      var some =  Vertices.OrderBy(x => x.z).ThenBy(x => x.x).ToArray();

      foreach (var s in some)
      {
          Debug.Log(s);
      }
      
    }

    public Vector3 GetPanelCenter(int panel)
    {
        panel *= 4;
        var first = Vertices[panel];
        var second = Vertices[panel + 2];
        var size = second - first;
        var pos = first + size / 2;
        Debug.Log(pos);
        return pos;
    }
    
    public void InstanceMesh()
    {
        GetComponent<MeshFilter>().mesh = TheMesh = new Mesh();
        TheMesh.name = "TheMesh";
    }
    
    public void ApplyMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
        UpdateMesh();
    }
    
    public int AddQuadWithPointList(List<Vector3> pointList)
    {
        var vertexIndex = Vertices.Count;
        
        Vertices.Add(pointList[0]);
        Vertices.Add(pointList[1]);
        Vertices.Add(pointList[2]);
        Vertices.Add(pointList[3]);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 3);
        UpdateMesh();

        return vertexIndex;
    }

    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
    {
        var vertexIndex = Vertices.Count;
        Vertices.Add(v1);
        Vertices.Add(v2);
        Vertices.Add(v3);
        Vertices.Add(v4);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 3);
        
        UpdateMesh();
    }
    
    public int AddQuad2(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
    {
        var vertexIndex = Vertices.Count;
        Vertices.Add(v1);
        Vertices.Add(v2);
        Vertices.Add(v3);
        Vertices.Add(v4);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex );
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 3);
        
        UpdateMesh();

        return vertexIndex;
    }

    protected void UpdateMesh()
    {
        TheMesh.Clear();
        TheMesh.SetVertices(Vertices);
        TheMesh.SetTriangles(Triangles, 0);
        TheMesh.RecalculateNormals();
    }


    public void ClearMesh()
    {
        Vertices.Clear();
        Triangles.Clear();
        TheMesh.SetVertices(Vertices);
        TheMesh.SetTriangles(Triangles, 0);
    }


    public void MoveVertices(Dictionary<int, Vector3> pointsAndMove)
    {
        foreach (var p in pointsAndMove)
        {
            Vertices[p.Key] += p.Value;
        }
        UpdateMesh();
    }

    public void MoveVertices(Vector3 moveAmount, List<int> indexes)
    {
        foreach (var i in indexes)
        {
            Vertices[i] += moveAmount;
        }
        
        UpdateMesh();
    }

    protected void MoveTwoVerts(int indexOne, int indexTwo, Vector3 moveAmount)
    {
        Vertices[indexOne] += moveAmount;
        Vertices[indexTwo] += moveAmount;
        
        UpdateMesh();
    }

    protected void SetTwoVerts(int indexOne, int indexTwo, Vector3 setVector, float offset)
    {
        
        Vertices[indexOne] = setVector;
        Vertices[indexTwo] = setVector + new Vector3(0, offset, 0);
        
        UpdateMesh();
    }

    protected void AddTriangle(Vector3 v1,Vector3 v2, Vector3 v3)
    {
        var vertexIndex = Vertices.Count;
        Vertices.Add(v1);
        Vertices.Add(v2);
        Vertices.Add(v3);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        
        UpdateMesh();
    }
    
    public void AddTriangle2(Vector3 v1,Vector3 v2, Vector3 v3)
    {
        var vertexIndex = Vertices.Count;
        Vertices.Add(v1);
        Vertices.Add(v2);
        Vertices.Add(v3);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        
        UpdateMesh();
    }

    protected int AddTriangleWithPointList(List<Vector3> pointList)
    {
        var vertexIndex = Vertices.Count;
        Vertices.Add(pointList[0]);
        Vertices.Add(pointList[1]);
        Vertices.Add(pointList[2]);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        
        UpdateMesh();

        return vertexIndex;
    }


    private void RemoveQuad(int firstIndex)
    {
        var tris = Triangles.FindAll(x => x == firstIndex);
        Triangles.RemoveAt(firstIndex + 5);
        Triangles.RemoveAt(firstIndex + 4);
        Triangles.RemoveAt(firstIndex + 3);
        Triangles.RemoveAt(firstIndex + 2);
        Triangles.RemoveAt(firstIndex + 1);
        Triangles.RemoveAt(firstIndex);
        UpdateMesh();
    }
    
}
