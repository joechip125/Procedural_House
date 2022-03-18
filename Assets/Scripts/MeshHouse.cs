using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshHouse : MonoBehaviour
{
    private Dictionary<Vector3, MeshRoom> _rooms = new();

    public GameObject meshRoom;


    private void Start()
    {
        MakeARoom(new Vector3(0,0,0), new Vector3(10,4,10), new Vector3(0,0,0));
    }

    private void MakeARoom(Vector3 start, Vector3 size, Vector3 index)
    {
        var room = 
            Instantiate(meshRoom, new Vector3(0, 0, 0), Quaternion.identity, transform);
        room.GetComponent<MeshRoom>().size = size;
        room.GetComponent<MeshRoom>().start = start;
        room.GetComponent<MeshRoom>().MakeNewFloor(0, new Vector3(1,0,1));
        
        _rooms.Add(index, room.GetComponent<MeshRoom>());
    }
}
