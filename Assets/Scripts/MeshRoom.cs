using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public enum RoomDirections
{
    XPlus, XMinus, ZPlus, ZMinus
}

public enum RoomTypes
{
    Rectangle, LShape, TShape
}

public class MeshRoom : MonoBehaviour
{
    public Vector3 start;
    public Vector3 size;
    public Material baseMaterial;
    public string roomName;
    public GameObject meshWall;
    
    Mesh roomMesh;
    MeshCollider meshCollider;
    [NonSerialized] List<Vector3> vertices = new ();
    [NonSerialized] private List<int> triangles = new ();
//    [NonSerialized] private List<Color> _colors;

    public Dictionary<int, MeshTiles> MeshTilesList = new ();

    public Dictionary<int,AdvancedMesh_Wall> meshWalls;
    
    
    private void Awake()
    {
        AwakeMethod();
    }


    private void InstanceNewWall()
    {
        var temp = Instantiate(meshWall, transform);
        var aWall = temp.GetComponent<AdvancedMesh_Wall>();
        

    }
    
    private void AwakeMethod()
    {
        GetComponent<MeshFilter>().mesh = roomMesh = new Mesh();
        roomMesh.name = "RoomMesh";
        var meshTile = new MeshTiles();
        
        
        MeshTilesList.Add(0, meshTile);
    }

    public Vector3 GetNormalAtVert(int vertIndex)
        => roomMesh.normals[vertIndex];
    
    
    public Vector3 GetPositionAtVert(int vertIndex)
        => vertices[vertIndex];


    public void DisplayCosine(int index)
    {

        var input = 180f * index;
        var aVector =
            roomMesh.normals[MeshTilesList[1].panels[0].startTriangleIndex];
    }
    
    public void AddRoom()
    {
        GetComponent<MeshFilter>().mesh = roomMesh = new Mesh();
        roomMesh.name = "RoomMesh";
        var meshTile = new MeshTiles()
        {
            TilesType = TilesType.Floor
        };
        
        MeshTilesList.Add(0, meshTile);
    }

    public void AddOuterWalls()
    {
        var addVector = new Vector3(MeshStatic.OuterWallThickness * 1, 0, MeshStatic.OuterWallThickness * 1);
        MakeANewWall(vertices[0] + new Vector3(addVector.x * -1, 0, addVector.z * -1), new Vector3(1,1,0), 2, 5, addVector);
        MakeANewWall(vertices[1] + new Vector3(addVector.x * 1, 0, addVector.z * -1), new Vector3(0,1,1), 2, 6, addVector);
        MakeANewWall(vertices[3] + new Vector3(addVector.x * 1, 0, addVector.z * 1), new Vector3(-1,1,0), 2, 7, addVector);
        MakeANewWall(vertices[2] + new Vector3(addVector.x * -1, 0, addVector.z * 1), new Vector3(0,1,-1), 2, 8, addVector);
    }
    
    private void AddPanelToWall(int wallIndex, int panelIndex, Vector3 newSize)
    {
        var panelOne = MeshTilesList[wallIndex].panels[panelIndex];

        var pos = vertices[panelOne.startTriangleIndex + 1] + new Vector3(0,3,0);
        
        var points =
            MeshStatic.SetVertexPositions(pos,  newSize, true, panelOne.direction);

        AddQuadWithPointList(points);
    }

    public Vector3 GetNewFloorPos(RoomDirections directions, Vector3 newSize)
    {
        var panel = MeshTilesList[0].panels[0];
        var newStart = vertices[panel.startTriangleIndex];
        
        switch (directions)
        {
            case RoomDirections.XPlus:
                newStart = vertices[panel.startTriangleIndex +  1];
                break;
            case RoomDirections.XMinus:
                newStart = vertices[panel.startTriangleIndex] - new Vector3(newSize.x,0,0);
                break;
            case RoomDirections.ZPlus:
                newStart = vertices[panel.startTriangleIndex + 2];
                break;
            case RoomDirections.ZMinus:
                newStart = vertices[panel.startTriangleIndex ] - new Vector3(0,0, newSize.z);
                break;
        }

        return newStart;
    }
    
    public void AddFloorTile(int panelIndex, Vector3 newSize, RoomDirections directionFromTile, Vector3 index)
    {
        var newDirection = new Vector3(1, 0, 1);
        var panel = MeshTilesList[0].panels[panelIndex];
        var newStart = vertices[panel.startTriangleIndex];

        switch (directionFromTile)
        {
            case RoomDirections.XPlus:
                newStart = vertices[panel.startTriangleIndex +  1];
                break;
            case RoomDirections.XMinus:
                newStart = vertices[panel.startTriangleIndex] - new Vector3(newSize.x,0,0);
                break;
            case RoomDirections.ZPlus:
                newStart = vertices[panel.startTriangleIndex + 2];
                break;
            case RoomDirections.ZMinus:
                newStart = vertices[panel.startTriangleIndex ] - new Vector3(0,0, newSize.z);
                break;
        }
        

        var points =
            MeshStatic.SetVertexPositions(newStart, newSize, false, newDirection);

        var newIndex = AddQuadWithPointList(points);

        var newPanel = new MeshPanel(newIndex, newDirection);
        MeshTilesList[0].panels.Add(newPanel);
    }
    
    public void MakeNewFloor(int meshTilesIndex, Vector3 direction)
    {
        var wallOrFloor = direction.y != 0;

        var points =
            MeshStatic.SetVertexPositions(start, size, wallOrFloor, direction);
        
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

    public void RemoveAWall(int wallIndex)
    {
        int start = MeshTilesList[wallIndex].panels[0].startTriangleIndex;
        int start2 = MeshTilesList[wallIndex].panels[1].startTriangleIndex;
        int end = MeshTilesList[wallIndex].panels[^1].startTriangleIndex;
        RemoveQuad(start);
        RemoveQuad(end);
        
        
        
        foreach (var p in MeshTilesList[wallIndex].panels)
        {
        //    RemoveQuad(p.startTriangleIndex);    
        }

        Debug.Log($"number of verts: {vertices.Count}");
        Debug.Log($"number of tris: {triangles.Count}");
        
        
        Debug.Log($"number of verts: {vertices.Count}");
        Debug.Log($"number of tris: {triangles.Count}");
    }
    
    public void MakeWallOpening(int wallIndex, int firstPanel, float openingSize)
    {
        var panelOne = MeshTilesList[wallIndex].panels[firstPanel];
        var panelTwo = MeshTilesList[wallIndex].panels[firstPanel + 1];

        var direction = panelOne.direction;
        var addVec = new Vector3(panelOne.direction.x * openingSize / 2, 0, direction.z * openingSize / 2);

        var theNormal = roomMesh.normals[panelOne.startTriangleIndex + 1];


        vertices[panelOne.startTriangleIndex + 1] -= addVec;
        vertices[panelOne.startTriangleIndex + 3] -= addVec;

        vertices[panelTwo.startTriangleIndex] += addVec;
        vertices[panelTwo.startTriangleIndex + 2] += addVec;
     
        var theNormal2 = new Vector3(-theNormal.x,1, -theNormal.z);
        
        var newPanel = new Vector3(MeshStatic.OuterWallThickness, size.y, MeshStatic.OuterWallThickness);
        var doorSize = new Vector3(Mathf.Abs(openingSize * direction.x), 2, Mathf.Abs(openingSize * direction.z));
        AddDoorway(new Vector3(1,0,1), vertices[panelOne.startTriangleIndex + 1] + new Vector3(-0.1f,0,0), doorSize);
    }

    private void AddDoorway(Vector3 primeDirection, Vector3 aStart, Vector3 openingSize)
    {
        float openingLength = 1;
        float openingWidth = 0.1f;
        float remainingH = size.y - openingSize.y;
        var tile = new MeshTiles();
        MeshTilesList.Add(30, tile);
        CreateNewPanel(aStart, openingSize, primeDirection, 30);
        CreateNewPanel(aStart + new Vector3(openingSize.x,openingSize.y,openingSize.z), new Vector3(0.1f,0, openingLength), new Vector3(1,0,-1), 30);
        CreateNewPanel(aStart + new Vector3(openingWidth,0,0), new Vector3(openingWidth,openingSize.y, openingWidth), new Vector3(-1,1,0), 30);
        CreateNewPanel(aStart + new Vector3(0,0,openingLength), new Vector3(openingWidth,openingSize.y, openingWidth), new Vector3(1,1,0), 30);
        
        CreateNewPanel(aStart + new Vector3(openingWidth,openingSize.y,0), new Vector3(1,2, 1), new Vector3(0,1,1), 30);
    }
    
    public void AddDoorway2(Vector3 aStart, Vector2 openingSize, Vector3 direction)
    {
        var actualSize = new Vector3();
        var aDirection = new Vector3(0,1,1);
        var aDirection2 = new Vector3(-1,0,1);
        if (direction.x != 0)
        {
            actualSize = new Vector3(openingSize.x, openingSize.y, 0.1f);
        }
        
        if (direction.z != 0)
        {
            actualSize = new Vector3(0.1f, openingSize.y, openingSize.x);
            aDirection = new Vector3(1,1,0);
        }
        
        float remainingH = size.y - openingSize.y;
        int wallIndex = 30;
        var tile = new MeshTiles();
        MeshTilesList.Add(wallIndex, tile);
        CreateNewPanel(aStart, actualSize, new Vector3(1,0,1), wallIndex);
        var index = MeshTilesList[wallIndex].panels[0].startTriangleIndex;
        
        
        CreateNewPanel(roomMesh.vertices[index], actualSize, aDirection, wallIndex);
        CreateNewPanel(roomMesh.vertices[index + 3], actualSize, new Vector3(-aDirection.x,1,-aDirection.z), wallIndex);
        
        CreateNewPanel(roomMesh.vertices[index + 1] + new Vector3(0,openingSize.y,0), actualSize, aDirection2, wallIndex);
        
    }
    
    
    private void CreateNewPanel(Vector3 theStart, Vector3 theSize, Vector3 theDirection, int wallIndex)
    {
        var wallOrFloor = theDirection.y != 0;
        
        var points =
            MeshStatic.SetVertexPositions(theStart, theSize, wallOrFloor, theDirection);
        var vertIndex = AddQuadWithPointList(points);
        var meshPanel = new MeshPanel(vertIndex, theDirection);
        MeshTilesList[wallIndex].panels.Add(meshPanel);
    }

    private void MoveAWall(Vector3 moveAmount, int wallIndex)
    {
        foreach (var p in MeshTilesList[wallIndex].panels)
        {
            for (var i = 0; i < 4; i++)
            {
                vertices[p.startTriangleIndex + i] += moveAmount;    
            }
        }
        UpdateMesh();
    }
    
    
    private void MovePanelSide(int wallIndex, int panelIndex, int sideIndex, float moveAmount)
    {
        var panelOne = MeshTilesList[wallIndex].panels[panelIndex];
        
        var wallNormal = roomMesh.normals[panelOne.startTriangleIndex];
        var vertOne = sideIndex;
        var vertTwo = sideIndex + 1;
        
        
        if (sideIndex > 2)
        {
            vertTwo = 0;
        }
        
        if (moveAmount > 0)
        {
                   
        }
        else
        {
            
        }
    }

    public void BuildAllWalls()
    {
        for (int i = 1; i < 5; i++)
        {
            BuildAWall(2, i);
        }
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
    
    private void MakeANewWall(Vector3 startPos, Vector3 direction, int numberPanels, int wallIndex, Vector3 addVector = new Vector3())
    {
        var panelSize = new Vector3(size.x / numberPanels, size.y, size.z / numberPanels) + addVector;

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
    
    
    private void UpdateMesh()
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
    
    private int AddQuadWithPointList(List<Vector3> pointList)
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
    
}
