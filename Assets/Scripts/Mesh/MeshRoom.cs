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

    public Dictionary<int,AdvancedMesh_Wall> meshWalls = new Dictionary<int, AdvancedMesh_Wall>();
    public AdvancedMesh_Floor theFloor;
    
    public Vector3 GetWallForward(int wallIndex)
        => -meshWalls[wallIndex].GetNormalAtIndex(0);
    
    
    public void ExpandRoom(Vector3 direction, Vector3 newSize, Vector3 oldIndex, int wallDir = 0)
    {
        var aMoveDir = GetWallForward(wallDir);
        
        var newIndex = oldIndex + aMoveDir;
        theFloor.AddFloorTile(newSize, aMoveDir, oldIndex);
        var theWall = 2;
        var wallDirection = -meshWalls[theWall].WallTiles[0].direction;
        var aMove = new Vector3(newSize.x * wallDirection.x, 0, newSize.z * wallDirection.z);
        

        meshWalls[wallDir].ShrinkAllWallTiles(true, newSize.x);

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
    }


    public void ShrinkAWall(int wallIndex)
    {
        meshWalls[2].ShrinkAllWallTiles(true, 1);
        meshWalls[3].ShrinkAllWallTiles(true, 1);
    }
    
    public void InstanceNewWall(int wallIndex, Vector3 floorIndex)
    {
        if (meshWalls.ContainsKey(wallIndex)) return;
   
        var simpleDir = GetDirectionFromCosine(wallIndex);
 
        var thePos = theFloor.GetPositionClockWise(floorIndex); 
        var theSize = (thePos[2] - thePos[0]) / 3;
        
        var panelSize = new Vector3(theSize.x, 4, theSize.z);
        var floorTrans = theFloor.transform;

        var temp = Instantiate(meshWall,floorTrans.position, Quaternion.identity, floorTrans);
        var aWall = temp.GetComponent<AdvancedMesh_Wall>();
        aWall.CreateNewPanel(thePos[wallIndex], panelSize, simpleDir, 0);
        aWall.AddPanel(panelSize, simpleDir);
        aWall.AddPanel(panelSize, simpleDir);

        meshWalls.Add(wallIndex, aWall);

        aWall.ApplyMaterial(baseMaterial);
    }

    
    
    private Vector3 GetDirectionFromCosine(float wallIndex)
    {
        var deg = 90 + wallIndex * 90;
        var theCos = Mathf.Round(Mathf.Cos(deg * Mathf.PI / 180));
        var theSin = Mathf.Round(Mathf.Sin(deg * Mathf.PI / 180));
        var simpleDir = new Vector3(theCos,1,theSin);

        return simpleDir;
    }
}
