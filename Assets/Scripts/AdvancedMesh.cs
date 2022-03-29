using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMesh : MonoBehaviour
{
    Mesh theMesh;
    MeshCollider meshCollider;
    [NonSerialized] List<Vector3> vertices = new ();
    [NonSerialized] private List<int> triangles = new ();
    public Dictionary<int, MeshTiles> MeshTilesList = new ();

    
    public void CreateNewPanel(Vector3 theStart, Vector3 theSize, Vector3 theDirection, int wallIndex)
    {
        var wallOrFloor = theDirection.y != 0;
        
        var points =
            MeshStatic.SetVertexPositions(theStart, theSize, wallOrFloor, theDirection);
        var vertIndex = AddQuadWithPointList(points);
        var meshPanel = new MeshPanel(vertIndex, theDirection);
        MeshTilesList[wallIndex].panels.Add(meshPanel);
    }
    
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
