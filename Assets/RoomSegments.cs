using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomSegments : MonoBehaviour
{
    private AdvancedMesh mesh;
    public float sizeX;
    public float sizeZ;
    public float sizeY;

    public Vector3 currentPos;
    public Vector3 Max => currentPos + new Vector3(sizeX / 2, sizeY, sizeZ/ 2);
    public Vector3 Min => currentPos - new Vector3(sizeX / 2, 0, sizeZ / 2);
    
    private void Awake()
    {
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        currentPos = transform.position;
        SegTwo();
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
        AddWall(AddDirection.East);
        SetFloorTile();
        currentPos += new Vector3(0, 0, 60);
        sizeZ = 20;
        SetFloorTile();
        sizeZ = 100;
        currentPos += new Vector3(0, 0, 60);
        SetFloorTile();
        AddWall(AddDirection.East);
    }
    
    
    private void SetFloorTile()
    {
        var top =  new Vector3(Min.x, 0, Max.z);
        var left = new Vector3(Max.x, 0, Min.z);
        var other = new Vector3(Max.x, 0, Max.z);
        mesh.AddQuad(Min, left, top, other);
    }

    private void SetCeilingTile()
    {
        var first = new Vector3(Min.x, Max.y, Min.z);
        var top =  new Vector3(Max.x, Max.y, Min.z);
        var left = new Vector3(Min.x, Max.y, Max.z);
        var other = new Vector3(Max.x, Max.y, Max.z);
        mesh.AddQuad(first, left, top, other);
    }
    
    

    private void AddWall(AddDirection direction)
    {
        var min = new Vector3(Max.x, 0, Min.z);
        var top = new Vector3(Max.x, Max.y, Min.z);
        var left = new Vector3(Max.x, 0, Max.z);
        var other = new Vector3(Max.x, Max.y, Max.z);
        
        switch (direction)
        {
            case AddDirection.North:
                min = new Vector3(Max.x, 0, Min.z);
                top = new Vector3(Max.x, Max.y, Min.z);
                left = new Vector3(Max.x, 0, Max.z);
                other = new Vector3(Max.x, Max.y, Max.z);
                break;
            case AddDirection.East:
                min = new Vector3(Min.x, 0, Min.z);   
                left = new Vector3(Max.x, 0, Min.z);  
                top = new Vector3(Min.x, Max.y, Min.z);  
                other = new Vector3(Max.x, Max.y, Min.z);
                break;
            case AddDirection.South:
                break;
            case AddDirection.West:
                min = new Vector3(Min.x, 0, Max.z);   
                top = new Vector3(Max.x, 0, Max.z);  
                left = new Vector3(Min.x, Max.y, Max.z);  
                other = new Vector3(Max.x, Max.y, Max.z);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(direction), direction, null);
        }
        
       
        mesh.AddQuad(min, top, left, other);
    }

    
    
    
    private void AddAPanel()
    {
        
    }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
            
    }
}
