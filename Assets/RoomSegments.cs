using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class RoomSegments : MonoBehaviour
{
    private AdvancedMesh mesh;
    public float sizeX, sizeY, sizeZ;
   
    public List<Segment> segments = new();

    public Vector3 currentPos;
    public Vector3 Max => currentPos + new Vector3(sizeX / 2, sizeY, sizeZ/ 2);
    public Vector3 Min => currentPos - new Vector3(sizeX / 2, 0, sizeZ / 2);
    


    private void Start()
    {
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        currentPos = transform.position;
        SetFloorTile();
        AddWall(AddDirection.East);
        var mid = 20;
        var edge = 100;
        currentPos += new Vector3(sizeX / 2 + (mid / 2), 0, 0);
        sizeX = mid;
        SetFloorTile();
        sizeX = edge;
        currentPos += new Vector3(sizeX / 2 + (mid / 2), 0, 0);
        SetFloorTile();
        AddWall(AddDirection.East);
        //SegOne();
        //SegTwo();
    }

    public void AddSegment(float xSize, float zSize)
    {
        segments.Add(new Segment(3)
        {
            
        });
    }
    

    private void SegTwo()
    {
        sizeX = 10;
        sizeZ = 40;
        SetFloorTile();
        AddWall(AddDirection.East);
        AddWall(AddDirection.West);
        SetCeilingTile();
    }

    private void SegOne()
    {
        Debug.Log("started");
        int mid = 40;
        int sides = 100;
        AddWall(AddDirection.North);
        SetFloorTile();
        currentPos += new Vector3(0, 0, (mid / 2) + sizeZ / 2);
        sizeZ = mid;
        SetFloorTile();
        var hold = currentPos;
        sizeZ = sides;
        currentPos += new Vector3(0, 0, (mid / 2) + sizeZ / 2);
        SetFloorTile();
        AddWall(AddDirection.North);
        currentPos = hold + new Vector3(55,0,0);
        SegTwo();
    }
    
    
    private void SetFloorTile()
    {
        var v1 = new Vector3(Min.x, 0, Min.z);
        var v2 = v1 + new Vector3(0, 0, sizeZ);
        var v3 = v1 + new Vector3(sizeX, 0, sizeZ);
        var v4 = v1 + new Vector3(sizeX, 0, 0);
        
        mesh.AddQuad2(v1, v2, v3, v4);
    }

    private void SetCeilingTile()
    {
        var v1 = new Vector3(Min.x, currentPos.y, Min.z);
        var v2 = v1 + new Vector3(sizeX, 0, 0);
        var v3 = v1 + new Vector3(sizeX, 0, sizeZ);
        var v4 = v1 + new Vector3(0, 0, sizeZ);
        
        mesh.AddQuad2(v1, v2, v3, v4);
    }
    
    

    private void AddWall(AddDirection direction)
    {
        var v1 = new Vector3(Min.x, 0, Min.z);
        var max = new Vector3(0, sizeY, sizeZ);
        
        switch (direction)
        {
            case AddDirection.North:
                v1 = new Vector3(Min.x, 0, Min.z);
                max = new Vector3(0, sizeY, sizeZ);
                break;
            case AddDirection.East:
                v1 = new Vector3(Min.x, 0, Max.z);
                max = new Vector3(sizeX, sizeY, 0);
                break;
            case AddDirection.South:
                v1 = new Vector3(Max.x, 0, Max.z);
                max = new Vector3(0, sizeY, -sizeZ);
                break;
            case AddDirection.West:
                v1 = new Vector3(Max.x, 0, Min.z);
                max = new Vector3(-sizeX, sizeY, 0);
                break;
        }
        var v2 = v1 + new Vector3(0, sizeY, 0);
        var v4 = v1 + new Vector3(max.x, 0, max.z);
        var v3 = v1 + max;
        
        mesh.AddQuad2(v1, v2, v3, v4);
    }

}
