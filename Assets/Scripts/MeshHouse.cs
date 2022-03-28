using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class MeshHouse : MonoBehaviour
{
    private Dictionary<Vector3, MeshRoom> _rooms = new();
    public GameObject meshRoom;
    
    private void Start()
    {
        MakeARoom(new Vector3(0,0,0), new Vector3(10,4,10), new Vector3(0,0,0), true);
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
        _rooms[index].AddOuterWalls();

        _rooms[index].MakeWallOpening(1,0, 1f);
    }
    
    private void MakeARoom(Vector3 start, Vector3 size, Vector3 startIndex, bool newOrExtend = true)
    {
        var room = 
            Instantiate(meshRoom, new Vector3(0, 0, 0), Quaternion.identity, transform);
        room.GetComponent<MeshRoom>().size = size;
        room.GetComponent<MeshRoom>().start = start;
    //    room.GetComponent<MeshRoom>().MakeNewFloor(0, new Vector3(1,0,1));
        
        _rooms.Add(startIndex, room.GetComponent<MeshRoom>());
    }
}
