using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;



public class MeshRoom : MonoBehaviour
{
    public Vector3 start;
    public Vector3 size;
    public Material baseMaterial;
    public GameObject theMeshWall;
    
    Mesh roomMesh;
    MeshCollider meshCollider;
    [NonSerialized] List<Vector3> vertices = new List<Vector3>();
    [NonSerialized] private List<int> triangles = new List<int>();
    [NonSerialized] private List<Color> _colors;
    
    public Dictionary<int, MeshTiles> MeshTilesList = new ();

    private void Start()
    {
        GetComponent<MeshFilter>().mesh = roomMesh = new Mesh();
        roomMesh.name = "RoomMesh";
        var meshTile = new MeshTiles()
        {
            TilesType = TilesType.Floor
        };
        
        MeshTilesList.Add(0, meshTile);
        
        MakeWallNewStyle(0, new Vector3(1,0,1), size, start);
        BuildAWall(3, 2);
    }

    
    
    
    private void MakeWallNewStyle(int meshTilesIndex, Vector3 direction, Vector3 newSize, Vector3 newStart)
    {
        var wallOrFloor = direction.y != 0;

        var points =
            MeshStatic.SetVertexPositions(newStart, newSize, wallOrFloor, direction);
        
        var vertIndex = AddQuadWithPointList(points);
        var panel = new MeshPanel(vertIndex, direction);
        MeshTilesList[meshTilesIndex].panels.Add(panel);
        
        GetComponent<MeshRenderer>().material = baseMaterial;
        UpdateMesh();
    }

    private void AddPanel(int meshTilesIndex, Vector3 newSize)
    {
        if (MeshTilesList.Count < meshTilesIndex)
        {
            var newTile = new MeshTiles();
            
        }
        
        var finalPanel = MeshTilesList[meshTilesIndex].panels[^1];
        var newStart = vertices[finalPanel.startTriangleIndex + 1];
        
        var points =
            MeshStatic.SetVertexPositions(newStart, newSize, false, new Vector3(1,0,1));
    
        var vertIndex = AddQuadWithPointList(points);
        UpdateMesh();
    }
    
    private void MakeWallOpening(int wallIndex, int firstPanel, float openingSize)
    {
        var wall = gameObject.GetComponentsInChildren<MeshWall>()[wallIndex];
        wall.ResizePanel(firstPanel, new Vector3(wall.direction.x,0,wall.direction.z), -openingSize / 2, false);
        wall.ResizePanel(firstPanel + 1, new Vector3(wall.direction.x,0,wall.direction.z), openingSize / 2, true);
    }

    private void BuildAWall(int numberPanels, int wallIndex)
    {

        switch (wallIndex)
        {
            case 1:
                MakeANewWall(start, new Vector3(0,1,1), numberPanels, wallIndex);
                break;
            case 2:
                MakeANewWall(start + new Vector3(0,0,size.z), new Vector3(1,1,0), numberPanels, wallIndex);
                break;
            case 3:
                MakeANewWall(start + new Vector3(size.x,0,size.z), new Vector3(0,1,-1), numberPanels, wallIndex);
                break;
            case 4:
                MakeANewWall(start + new Vector3(size.x,0,0), new Vector3(-1,1,0), numberPanels, wallIndex);
                break;
        }
    }
    
    private void MakeANewWall(Vector3 startPos, Vector3 direction, int numberPanels, int wallIndex)
    {
        var panelSize = new Vector3(size.x / numberPanels, size.y, size.z / numberPanels);
        wallIndex++;
        
        if (!MeshTilesList.ContainsKey(wallIndex))
        {
            var newTiles = new MeshTiles();
            MeshTilesList.Add(wallIndex, newTiles);
        }
        
        for (int i = 0; i < numberPanels; i++)
        {
            var points =
                MeshStatic.SetVertexPositions(startPos, panelSize, true, direction);
            
            var vertIndex = AddQuadWithPointList(points);

            var meshPanel = new MeshPanel(vertIndex, direction);
            MeshTilesList[wallIndex].panels.Add(meshPanel);
            startPos = points[1];
        }
    }
    

    private void AddPanelToWall(int wallIndex, Vector3 panelSize)
    {
        var aWall = gameObject.GetComponentsInChildren<MeshWall>()[wallIndex];
    }
    
    private void ResizeWall(int wallIndex, bool moveStartEnd, int panelIndex, float  moveAmount)
    {
        var aWall = gameObject.GetComponentsInChildren<MeshWall>()[wallIndex];
        Vector3 direction = aWall.direction;

        if (aWall.direction.z != 0 && aWall.direction.y != 0)
        {
            direction.y = 0;
        }
        
        if (aWall.direction.x != 0 && aWall.direction.y != 0)
        {
            direction.y = 0;
        }
        
        aWall.ResizePanel(panelIndex, direction, moveAmount, moveStartEnd);
    }
    
    
    public void UpdateMesh()
    {
        roomMesh.Clear();
        roomMesh.SetVertices(vertices);
        roomMesh.SetTriangles(triangles, 0);
        roomMesh.RecalculateNormals();
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


    public int AddQuadWithPointList(List<Vector3> pointList)
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
