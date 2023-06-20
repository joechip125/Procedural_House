using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class RoomHolder
{
    public SingleRoom theRoom;
}

public class SimpleHouse : MonoBehaviour
{
    private List<NewAdvancedMesh> meshes = new();
    [SerializeField] private List<GameObject> roomTiles = new();
    private Vector3 startSize;
    private List<RoomHolder> rooms = new();
    

    void Start()
    {
        SpawnRoom();
    }
    
    private void SpawnRoom()
    {
        var temp = Instantiate(roomTiles[0], transform)
            .GetComponent<SingleRoom>();
        rooms.Add(new RoomHolder(){ theRoom = temp});
    }
}
