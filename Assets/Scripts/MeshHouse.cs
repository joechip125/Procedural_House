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
    public Vector3 origin;
    public GameObject meshFloor;

    public Material simpleMaterial;

    private void Start()
    {
        var start = new Vector3(0, 0, 0);
        var size = new Vector3(10, 4, 10);
        var index = new Vector3(0, 0, 0);
        MakeARoom(start, size, index);
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

    
    
    private void AddWallsToFloor(Vector3 floorIndex, float wallH = 4)
    {
        var room = 
            Instantiate(meshRoom, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<MeshRoom>();

        room.InstanceNewWall(0, floorIndex);
        room.InstanceNewWall(1, floorIndex);
        room.InstanceNewWall(2, floorIndex);
        room.InstanceNewWall(3, floorIndex);

        _rooms.Add(floorIndex, room);
    }
    
    private void AddRandomRoom(Vector3 startIndex)
    {
      
    }

    
    private void AddWallsToRoom(Vector3 index)
    {
      //  _rooms[index].BuildAllWalls();

      //  _rooms[index].MakeWallOpening(1,0, 1f);
    }
    
    private void MakeARoom(Vector3 start, Vector3 size, Vector3 startIndex, bool newOrExtend = true)
    {
        if (newOrExtend)
        {
            var room =
                Instantiate(meshRoom, new Vector3(0, 0, 0), Quaternion.identity, transform).GetComponent<MeshRoom>();

            room.InstanceTheFloor(start, startIndex, size);
            
        }
        else
        {
            
        }

        //  room.GetComponent<MeshRoom>().MakeNewFloor(0, new Vector3(1,0,1));
      //  room.GetComponent<MeshRoom>().AddDoorway2(new Vector3(0,0,0), new Vector2(1,2), new Vector3(1,0,0));
        
        
    }
}
