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
    Lobby, Kitchen, Bedroom
}

public class MeshRoom : MonoBehaviour
{
    public Vector3 start;
    public Vector3 size;
    public Material baseMaterial;
    public string roomName;
    public RoomTypes roomType;
    public GameObject meshWall;
    public GameObject meshFloor;
 

//    [NonSerialized] private List<Color> _colors;

    public Dictionary<int, MeshTiles> MeshTilesList = new ();
 
    public Dictionary<Vector3, FloorTileValues> floorTileValues = new Dictionary<Vector3, FloorTileValues>();

    public Dictionary<int,AdvancedMesh_Wall> meshWalls = new Dictionary<int, AdvancedMesh_Wall>();

    public AdvancedMesh_Floor theFloor;
    
    
    private void Awake()
    {
      
    }

    public void ExpandRoom(Vector3 direction, Vector3 newSize, Vector3 oldIndex)
    {
        var newIndex = oldIndex + direction;
        theFloor.AddFloorTile(newSize, direction, oldIndex);
        var theWall = 2;
        var wallDirection = -meshWalls[theWall].WallTiles[0].direction;
        var aMove = new Vector3(newSize.x * wallDirection.x, 0, newSize.z * wallDirection.z);

        bool flip = false;
        int count = 0;

        var startTri = meshWalls[theWall].WallTiles[0].startTriangleIndex + 1;
        
        Dictionary<int, Vector3> wallPoints = new Dictionary<int, Vector3>();
        wallPoints.Add(startTri, aMove);
        wallPoints.Add(startTri + 2, aMove);

        foreach (var p in meshWalls[2].WallTiles)
        {
            if (flip)
            {
                startTri = meshWalls[theWall].WallTiles[p.Key].startTriangleIndex;
                wallPoints.Add(startTri, aMove);
                wallPoints.Add(startTri + 2, aMove);
                wallPoints.Add(startTri + 1, aMove * 2);
                wallPoints.Add(startTri + 3, aMove * 2);
            }

            flip = true;
            count++;
        }

        meshWalls[2].MoveVertices(wallPoints);

        var pos = theFloor.GetPositionClockWise(newIndex);
        
   
        InstanceNewWall(1, newIndex);
        InstanceNewWall(2, newIndex);
        InstanceNewWall(3, newIndex);
    }
    
    
    
    
    
    public void InstanceTheFloor(Vector3 theStart, Vector3 theIndex, Vector3 theSize)
    {
        var temp = Instantiate(meshFloor, theStart, Quaternion.identity, transform);
        theFloor = temp.GetComponent<AdvancedMesh_Floor>();
        theFloor.CreateNewPanel(theStart, theSize, new Vector3(1,0,1), theIndex);
        
        theFloor.GetComponent<MeshRenderer>().material = baseMaterial;
        
        InstanceNewWall(0, theIndex);
        InstanceNewWall(1, theIndex);
        InstanceNewWall(2, theIndex);
        InstanceNewWall(3, theIndex);
        
        ExpandRoom(new Vector3(1,0,0), new Vector3(5,4,5), theIndex);
    }

    public void InstanceNewWall(int wallIndex, Vector3 floorIndex)
    {
        float deg = 90 + wallIndex * 90;
        float theCos = Mathf.Round(Mathf.Cos(deg * Mathf.PI / 180));
        float theSin = Mathf.Round(Mathf.Sin(deg * Mathf.PI / 180));
        var simpleDir = new Vector3(theCos,1,theSin);
 
        var thePos = theFloor.GetPositionClockWise(floorIndex); 
        var theSize = (thePos[2] - thePos[0]);
        
        var panelSize = new Vector3(theSize.x, 4, theSize.z);
        var floorTrans = theFloor.transform;

        var temp = Instantiate(meshWall,floorTrans.position, Quaternion.identity, floorTrans);
        var aWall = temp.GetComponent<AdvancedMesh_Wall>();
        aWall.CreateNewPanel(thePos[wallIndex], panelSize, simpleDir, 0);
    //    aWall.AddPanel(panelSize, simpleDir);
        if(!meshWalls.ContainsKey(wallIndex))
            meshWalls.Add(wallIndex, aWall);

        aWall.ApplyMaterial(baseMaterial);
    }
    

    public void AddOuterWalls()
    {
        var addVector = new Vector3(MeshStatic.OuterWallThickness * 1, 0, MeshStatic.OuterWallThickness * 1);
    //    MakeANewWall(vertices[0] + new Vector3(addVector.x * -1, 0, addVector.z * -1), new Vector3(1,1,0), 2, 5, addVector);
    //    MakeANewWall(vertices[1] + new Vector3(addVector.x * 1, 0, addVector.z * -1), new Vector3(0,1,1), 2, 6, addVector);
    //    MakeANewWall(vertices[3] + new Vector3(addVector.x * 1, 0, addVector.z * 1), new Vector3(-1,1,0), 2, 7, addVector);
    //    MakeANewWall(vertices[2] + new Vector3(addVector.x * -1, 0, addVector.z * 1), new Vector3(0,1,-1), 2, 8, addVector);
    }
    
    private Vector3 GetWallDirection(int wallIndex)
    {
        var theDir = new Vector3(0, 0, 0);
        switch (wallIndex)
        {
            case 0:
                theDir = new Vector3(0, 1, 1);
                break;
            case 1:
                theDir = new Vector3(-1, 1, 0);
                break;
            case 2:
                theDir = new Vector3(0, 1, -1);
                break;
            case 3:
                theDir = new Vector3(1, 1, 0);
                break;
        }

        return theDir;
    }

}
