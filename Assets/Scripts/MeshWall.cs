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

    public Vector3 start;
    public Vector3 direction;
    public bool startMoving = false;

    private void Start()
    {
        var meshTile = new MeshTiles()
        {
            TilesType = TilesType.Floor
        };
        
        MeshTilesList.Add(meshTile);
    }

    private void Update()
    {
        if (startMoving)
        {
        }
    }


    public Vector3 GetEdgePosition(int vertOne, int vertTwo, float alpha)
    {
        return Vector3.Lerp(vertices[vertOne], vertices[vertTwo], alpha);
    }
    

    public void LogANormal()
    {
        Debug.Log(testMesh.normals[3]);
    }
    
    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = testMesh = new Mesh();
        testMesh.name = "testMesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        _colors = new List<Color>();
    }

    public void AddPanels(int startIndex)
    {
       
    }
    
    public void AddPanels()
    {
        
    }

    public void SetMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
        UpdateMesh();
    }
    
    public void AddMeshCollider()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }

    public void ResizePanel(int panelIndex, Vector3 moveVector, float moveAmount, bool startEnd)
    {
     
        var xMove = moveAmount;
        var zMove = moveAmount;
        
        
        if (moveVector.x != 0 || moveVector.z != 0)
        {
            if (moveVector.x == 0)
            {
                xMove = 0;
            }
            if (moveVector.z == 0)
            {
                zMove = 0;
            }
            
            if (startEnd)
            {
                
            }
            else
            {
                
            }
        }

        if (moveVector.y != 0)
        {
        }

        UpdateMesh();
    }
    public void MovePanel(int panelIndex, Vector3 moveVector)
    {


     
       
       UpdateMesh();
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


    public int AddQuadWithMeshPanel(List<Vector3> pointList)
    {
        int vertexIndex = vertices.Count;
        
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
    
    public void AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
    {
        int vertexIndex = vertices.Count;
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

 
}
