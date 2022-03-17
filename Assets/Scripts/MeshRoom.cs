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
    
    public List<MeshTiles> MeshTilesList = new ();

    private void Start()
    {
        GetComponent<MeshFilter>().mesh = roomMesh = new Mesh();
        roomMesh.name = "RoomMesh";
        var meshTile = new MeshTiles()
        {
            TilesType = TilesType.Floor
        };
        
        MeshTilesList.Add(meshTile);
        
        MakeWallNewStyle(0, new Vector3(1,0,1), new Vector3(10,4,10), new Vector3(0,0,0));
        AddPanel(0, new Vector3(5,4,5));
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
            MeshTilesList.Add(newTile);
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
            case 0:
                MakeAWall(start, new Vector3(0,1,1), numberPanels);
                break;
            case 1:
                MakeAWall(start + new Vector3(0,0,size.z), new Vector3(1,1,0), numberPanels);
                break;
            case 2:
                MakeAWall(start + new Vector3(size.x,0,size.z), new Vector3(0,1,-1), numberPanels);
                break;
            case 3:
                MakeAWall(start + new Vector3(size.x,0,0), new Vector3(-1,1,0), numberPanels);
                break;
        }
    }
    
    private void BuildAllWalls()
    {
        MakeAWall(start, new Vector3(0,1,1));
        MakeAWall(start + new Vector3(0,0,size.z), new Vector3(1,1,0));
        MakeAWall(start + new Vector3(size.x,0,size.z), new Vector3(0,1,-1));
        MakeAWall(start + new Vector3(size.x,0,0), new Vector3(-1,1,0));
    }

    private void AddFloorPanel(Vector3 newSize, Vector3 newDirection, int tileIndex)
    {
        var floorTrans = GetComponentsInChildren<Transform>()
            .SingleOrDefault(x => x.CompareTag("FloorContainer"));
        if (floorTrans == default) return;
        
        var floorTile = floorTrans.GetComponentsInChildren<MeshWall>()[0];

        floorTile.AddFloorPanel(newSize, 1, newDirection, new Vector3(1,0,-1), 0);
        floorTile.AddPanels();
        
    }
    
    private void MakeFloor()
    {
        var floorTrans = GetComponentsInChildren<Transform>()
            .SingleOrDefault(x => x.CompareTag("FloorContainer"));
        
        var aFloor = Instantiate(theMeshWall, new Vector3(0, 0, 0), Quaternion.identity, floorTrans);
        aFloor.GetComponent<MeshWall>().BuildFloor(new Vector3(1,0,1), size, start);
        aFloor.GetComponent<MeshWall>().AddPanels(0);
        aFloor.GetComponent<MeshWall>().SetMaterial(baseMaterial);
    }

    private void MakeAWall(Vector3 startPos, Vector3 direction)
    {
        var wallTrans = GetComponentsInChildren<Transform>()
            .SingleOrDefault(x => x.CompareTag("WallContainer"));
        
        var aWall = Instantiate(theMeshWall, new Vector3(0, 0, 0), Quaternion.identity, wallTrans);
        aWall.GetComponent<MeshWall>().start = startPos;
        aWall.GetComponent<MeshWall>().direction = direction;
        aWall.GetComponent<MeshWall>().AddWallPanel(direction, new Vector3(0,0,0), size, 0);
        aWall.GetComponent<MeshWall>().AddPanels(0);
        aWall.GetComponent<MeshWall>().SetMaterial(baseMaterial);
    }
    
    private void MakeAWall(Vector3 startPos, Vector3 direction, int numberPanels)
    {
        var wallTrans = GetComponentsInChildren<Transform>()
            .SingleOrDefault(x => x.CompareTag("WallContainer"));
        
        var aWall = Instantiate(theMeshWall, new Vector3(0, 0, 0), Quaternion.identity, wallTrans);
        aWall.GetComponent<MeshWall>().start = startPos;
        aWall.GetComponent<MeshWall>().direction = direction;
        var theSize = new Vector3(size.x / numberPanels, size.y, size.z / numberPanels);
        
        for (int i = 0; i < numberPanels; i++)
        {
            aWall.GetComponent<MeshWall>().AddWallPanel(theSize, 1, new Vector3(0,0,0));
            aWall.GetComponent<MeshWall>().AddPanels(i);
        }
        aWall.GetComponent<MeshWall>().SetMaterial(baseMaterial);    
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
