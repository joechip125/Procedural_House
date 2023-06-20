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
        var center = new Vector3();
        SpawnRoom();
    }


    private void SpawnRoom()
    {
        var temp = Instantiate(roomTiles[0], transform)
            .GetComponent<SingleRoom>();
        rooms.Add(new RoomHolder(){ theRoom = temp});
    }

    private void AddWall()
    {
        var place = new Vector3(-200, 0,200);
        meshes.Add(Instantiate(roomTiles[1],place, Quaternion.identity, transform)
            .GetComponent<NewAdvancedMesh>());
        var temp = (NewAdvancedMesh_Wall)meshes[^1];
        temp.wallInfos.Add(new WallInfo()
        {
            type = WallTypes.Blank
        });
        temp.wallInfos.Add(new WallInfo()
        {
            type = WallTypes.Blank
        });
        temp.wallInfos.Add(new WallInfo()
        {
            type = WallTypes.Door
        });
        temp.wallInfos.Add(new WallInfo()
        {
            type = WallTypes.Blank
        });
        temp.BuildWall();
    }
    
    private void AddFloor()
    {
        meshes.Add(Instantiate(roomTiles[0], transform).GetComponent<NewAdvancedMesh>());
        var temp = (NewAdvancedMesh_Floor)meshes[^1];
        temp.SetValuesAndActivate(100, 5,5);
        startSize = new Vector3(500,100,500);
    }
}
