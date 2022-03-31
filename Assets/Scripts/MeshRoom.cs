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

    public void InstanceTheFloor(Vector3 theStart, Vector3 theIndex, Vector3 theSize)
    {
        var temp = Instantiate(meshFloor, theStart, Quaternion.identity);
        theFloor = temp.GetComponent<AdvancedMesh_Floor>();
        theFloor.CreateNewPanel(theStart, theSize, new Vector3(1,0,1), theIndex);
        
        theFloor.GetComponent<MeshRenderer>().material = baseMaterial;
        
        InstanceNewWall(1,new Vector3(0,0,0));
        
    }

    public void InstanceNewWall(int wallIndex, Vector3 floorIndex)
    {
        var dir = GetWallDirection(wallIndex);
        var thePos = theFloor.GetPositionClockWise(floorIndex);
        var theSize = (thePos[3] - thePos[0]) / 2;

        var panelSize = new Vector3(theSize.x, 4, theSize.z);

        var temp = Instantiate(meshWall,thePos[0], Quaternion.identity, theFloor.transform);
        var aWall = temp.GetComponent<AdvancedMesh_Wall>();
        aWall.CreateNewPanel(thePos[wallIndex], panelSize, dir, 0);
        aWall.AddPanel(panelSize, dir);
        if(!meshWalls.ContainsKey(wallIndex))
            meshWalls.Add(wallIndex, aWall);
        
        var pos = meshWalls[wallIndex].GetPositionAtIndex(0, 1);
        var norm = meshWalls[wallIndex].GetNormalAtIndex(0, 1);
        
        //aWall.AddDoorway(pos, new Vector2(1,2), new Vector3(1,1,0), size.y, 0, norm);

        var poses =theFloor.GetPositionClockWise(floorIndex);

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
                theDir = new Vector3(-1, 1, 0);
                break;
        }

        return theDir;
    }

}
