using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AddDirection
{
    North,
    East,
    South,
    West
}

public class RoomSegment : MonoBehaviour
{
    private AdvancedMesh mesh;
    public float sizeX;
    public float sizeZ;

    public Vector3 Max => transform.position + new Vector3(sizeX, 0, sizeZ) / 2;
    public Vector3 Min => transform.position - new Vector3(sizeX, 0, sizeZ) / 2;
    
    private void Awake()
    {
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        SetFloorTile();
        AddWall(AddDirection.East);
    }

    private void SetFloorTile()
    {
        var pos = transform.position;
        var top = pos + new Vector3(0, 0, sizeZ);
        var left = pos + new Vector3(sizeX, 0, 0) ;
        var other = pos + new Vector3(sizeX, 0, sizeZ) ;
        mesh.AddQuad(pos, left, top, other);
    }


    private void AddWall(AddDirection direction)
    {
        var min = new Vector3(Max.x, 0, Min.z);
        var top = new Vector3(Max.x, 40, Min.z);
        var left = new Vector3(Max.x, 0, Max.z);
        var other = new Vector3(Max.x, 40, Max.z);
        mesh.AddQuad(min, top, left, other);
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
