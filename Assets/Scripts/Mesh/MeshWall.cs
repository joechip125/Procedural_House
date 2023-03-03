using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MeshWall : MonoBehaviour
{
    Mesh testMesh;
    MeshCollider meshCollider;
    [NonSerialized] List<Vector3> vertices;
    [NonSerialized] private List<int> triangles;
    [NonSerialized] private List<Color> _colors;
    public List<MeshTiles> MeshTilesList = new ();

    private void Start()
    {
        var meshTile = new MeshTiles()
        {
            TilesType = TilesType.Floor
        };
        
        MeshTilesList.Add(meshTile);
    }
    
    public Vector3 GetEdgePosition(int vertOne, int vertTwo, float alpha)
    {
        return Vector3.Lerp(vertices[vertOne], vertices[vertTwo], alpha);
    }
    
    
    private void RaisePanel(MeshPanel panel, float raiseAmount)
    {
        vertices[panel.startTriangleIndex] += new Vector3(0,raiseAmount);
        vertices[panel.startTriangleIndex + 1] += new Vector3(0,raiseAmount);
    }
    
    private void LowerPanel(MeshPanel panel, float lowerAmount)
    {
        vertices[panel.startTriangleIndex + 2] -= new Vector3(0,lowerAmount);
        vertices[panel.startTriangleIndex + 3] -= new Vector3(0,lowerAmount);
    }
    
    private void SetPointAt(MeshPanel panel)
    {
       LowerPanel(panel, 1);
      
        UpdateMesh();
    }
    
    public void UpdateMesh()
    {
        testMesh.Clear();
        testMesh.SetVertices(vertices);
        testMesh.SetTriangles(triangles, 0);
        testMesh.RecalculateNormals();
    }
    
}
