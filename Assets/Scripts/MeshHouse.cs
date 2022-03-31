using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshHouse : MonoBehaviour
{
    private Dictionary<Vector3, MeshRoom> _rooms = new();
    public GameObject meshRoom;
    private AdvancedMesh_Floor theFloorTiles;

    public GameObject meshFloor;

    public Material simpleMaterial;

    private void Start()
    {
        AddAFloor();
        AddAFloorTile(new Vector3(0,0,0), new Vector3(1,0,0));
        AddWallsToFloor(new Vector3(0,0,0), 4);
    }

    private void AddAFloor()
    {
        theFloorTiles = Instantiate(meshFloor, new Vector3(0, 0, 0), Quaternion.identity, transform)
            .GetComponent<AdvancedMesh_Floor>();
        theFloorTiles.CreateNewPanel(new Vector3(0,0,0), new Vector3(10,0,10), new Vector3(1,0,1), new Vector3(0,0,0));
        theFloorTiles.ApplyMaterial(simpleMaterial);
    }

    private void AddAFloorTile(Vector3 oldIndex, Vector3 newDirection)
    {
        var addPos = new Vector3(newDirection.x * MeshStatic.InnerWallThickness, 0, newDirection.z * MeshStatic.InnerWallThickness);
        theFloorTiles.AddFloorTile(new Vector3(5,0,5), newDirection, oldIndex, addPos);
    }

    private void AddWallsToFloor(Vector3 floorIndex, float wallH)
    {
        var room = 
            Instantiate(meshRoom, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<MeshRoom>();
 
        var floorValues = new FloorTileValues
        {
            pos = theFloorTiles.GetPositionFromTile(floorIndex)
        };

        room.floorTileValues.Add(floorIndex,floorValues);
        
        _rooms.Add(floorIndex, room);
    }
    
    private void AddRandomRoom(Vector3 startIndex)
    {
        Vector3 newSize = new Vector3(10, 4, 10);
       var aRoom = _rooms[startIndex];
       var random = Random.Range(1, aRoom.MeshTilesList.Count);
       var index = aRoom.MeshTilesList[random].panels[0].startTriangleIndex;
       var invertNormal = -aRoom.GetNormalAtVert(index);
       var aPos = aRoom.GetNewFloorPos((RoomDirections)random, newSize);
       var addVector = new Vector3(MeshStatic.InnerWallThickness * invertNormal.x, 0, MeshStatic.InnerWallThickness * invertNormal.z);
       
       MakeARoom(aPos + addVector, newSize, startIndex + invertNormal);
    }

    private void AddFloorTiles(Vector3 roomIndex, RoomDirections directions, Vector3 addVector = new Vector3())
    {
        _rooms[roomIndex].AddFloorTile(0, new Vector3(10,5,10), directions, roomIndex);
    }

    private void AddWallsToRoom(Vector3 index)
    {
        _rooms[index].BuildAllWalls();

        _rooms[index].MakeWallOpening(1,0, 1f);
    }
    
    private void MakeARoom(Vector3 start, Vector3 size, Vector3 startIndex, bool newOrExtend = true)
    {
        var room = 
            Instantiate(meshRoom, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<MeshRoom>();
        
      //  room.GetComponent<MeshRoom>().MakeNewFloor(0, new Vector3(1,0,1));
      //  room.GetComponent<MeshRoom>().AddDoorway2(new Vector3(0,0,0), new Vector2(1,2), new Vector3(1,0,0));
        
        
    }
}
