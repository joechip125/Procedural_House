using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class RoomSegments : MonoBehaviour
{
    private AdvancedMesh mesh;
    public float sizeX, sizeY, sizeZ;
    private Vector3 size = new Vector3(100,100,100);

    public int numberX, numberZ;
   
    public List<Segment> segments = new();

    public Vector3 currentPos;
    public Vector3 Max => currentPos + new Vector3(sizeX / 2, sizeY, sizeZ/ 2);
    public Vector3 Min => currentPos - new Vector3(sizeX / 2, 0, sizeZ / 2);
    public List<Vector3> tempPos = new();

    private readonly Vector3[] corners = new[]
        {   new Vector3(-1, 0, -1), 
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1), 
            new Vector3(1, 0, 0) };

    private Segment[] segArray;
    
    
    private void Start()
    {
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        currentPos = transform.position;
        HasNe();
        //AddFloorTile();
    }

    private void InitSegment()
    {
        var num = numberX * numberZ;
        segArray = new Segment[num];
        var pos = transform.position;

        for (int i = 0; i < num; i++)
        {
            segArray[i] = new Segment()
            {
                position = new Vector3()
            };
        }
    }


    private void HasNe()
    {
        var num = numberX * numberZ;
        segArray = new Segment[num];
        var pos = transform.position;
        var direction = new Vector3(-1, 0, -1);
        var axis = new Vector3(0, 1, 0);
        
        var total = 0;
        for (int i = 0; i < numberZ; i++)
        {
            currentPos = transform.position + new Vector3(0, 0, sizeZ * i);
            for (int j = 0; j < numberX; j++)
            {
                SetAPanel(direction, size, axis);
                segArray[total++] = new Segment()
                {
                    position = currentPos
                };
                
                if(j == 0) AddSomeWalls(new Vector3(-1,0,0));
                if(j == numberX - 1) AddSomeWalls(new Vector3(1,0,0));
                if(i == 0)AddSomeWalls(new Vector3(0,0,-1));
                if(i == numberZ - 1)AddSomeWalls(new Vector3(0,0,1));
                
                currentPos += new Vector3(sizeX, 0, 0);
            }    
        }
    }
   
    public void Find(int x, int z)
    {
        var found = segArray[z * numberX + x];
    }
    
    private void SetAPanel(Vector3 startDir, Vector3 theSize, Vector3 axis)
    {
        for (var i = 0; i < 4; i++)
        {
            corners[i] = currentPos+ Vector3.Scale(theSize / 2, 
                Quaternion.AngleAxis(90 * i, axis) * startDir);
        }
        mesh.AddQuad2(corners[0], corners[1], corners[2], corners[3]);
    }

    private void AddSomeWalls(Vector3 startDir)
    {
        var start = currentPos;
        currentPos = start + new Vector3((size.x / 2) * startDir.x, size.y / 2, (size.z / 2) * startDir.z);

        if (startDir.x != 0)
        {
            SetAPanel(new Vector3(0,-1,startDir.x), size, new Vector3(-startDir.x, 0,0));
        }
        else if (startDir.z != 0)
        {
            SetAPanel(new Vector3(startDir.z,-1,0), size, new Vector3(0, 0,-startDir.z));
        }

        currentPos = start;
    }
    

    private void MovePanel(int panelNumber, Vector2 mDir, float mAmount)
    {
        var first = 0;
        var second = 0;
        panelNumber *= 4;

        if (mDir.x != 0)
        {
            if (mDir.x > 0)
            {
                first = panelNumber;
                second = panelNumber + 1;
            }
            else if (mDir.x < 0)
            {
                first = panelNumber + 2;
                second = panelNumber + 3;
            }
        }
        else if (mDir.y != 0)
        {
            if (mDir.y > 0)
            {
                first = panelNumber + 1;
                second = panelNumber + 2;
            }
            else if (mDir.y < 0)
            {
                first = panelNumber;
                second = panelNumber + 3;
            }
        }
        
        mesh.MoveVertices(new Dictionary<int, Vector3>()
        {
            {first,new Vector3( mDir.x * mAmount,0,mDir.y *mAmount)},
            {second,new Vector3(mDir.x *mAmount,0,mDir.y *mAmount)},
        });
    }
    
    

    private void AddGrid()
    {
        var numX = 3;
        var numZ = 3;
        for (int i = 0; i < numZ; i++)
        {
            currentPos = transform.position + new Vector3(0, 0, sizeZ * i);
            for (int j = 0; j < numX; j++)
            {
                AddFloorTile();
                currentPos += new Vector3(sizeX, 0, 0);
            }
        }
    }
    
  

    private void AddDoorway()
    {
        AddSegment(20, 10, AddDirection.NorthSouth, 2);
        SetCeilingTile();
    }
    
   

    public void AddSegment(float xSize, float zSize, AddDirection wallChoice, int numberWalls)
    {
        sizeX = xSize;
        sizeZ = zSize;
        segments.Add(new Segment()
        {
            position = currentPos,
            size = new Vector3(sizeX, sizeY, sizeZ)
        });
        
        AddFloorTile();

        switch (numberWalls)
        {
            case 1:
                switch (wallChoice)
                {
                    case AddDirection.North:
                        AddWall(AddDirection.North);
                        break;
                    case AddDirection.East:
                        AddWall(AddDirection.East);
                        break;
                    case AddDirection.South:
                        AddWall(AddDirection.South);
                        break;
                    case AddDirection.West:
                        AddWall(AddDirection.West);
                        break;
                }
                break;
            
            case 2:
                switch (wallChoice)
                {
                    case AddDirection.North:
                        AddWall(AddDirection.North);
                        AddWall(AddDirection.East);
                        break;
                    case AddDirection.East:
                        AddWall(AddDirection.East);
                        AddWall(AddDirection.South);
                        break;
                    case AddDirection.South:
                        AddWall(AddDirection.South);
                        AddWall(AddDirection.West);
                        break;
                    case AddDirection.West:
                        AddWall(AddDirection.West);
                        AddWall(AddDirection.North);
                        break;
                    case AddDirection.NorthSouth:
                        AddWall(AddDirection.North);
                        AddWall(AddDirection.South);
                        break;
                    case AddDirection.EastWest:
                        AddWall(AddDirection.West);
                        AddWall(AddDirection.East);
                        break;
                }
                break;
            
            case 3:
                switch (wallChoice)
                {
                    case AddDirection.North:
                        AddWall(AddDirection.North);
                        AddWall(AddDirection.East);
                        AddWall(AddDirection.South);
                        break;
                    case AddDirection.East:
                        AddWall(AddDirection.East);
                        AddWall(AddDirection.South);
                        AddWall(AddDirection.West);
                        break;
                    case AddDirection.South:
                        AddWall(AddDirection.South);
                        AddWall(AddDirection.West);
                        AddWall(AddDirection.North);
                        break;
                    case AddDirection.West:
                        AddWall(AddDirection.West);
                        AddWall(AddDirection.North);
                        AddWall(AddDirection.East);
                        break;
                }
                break;
            
            case 4:
                AddWall(AddDirection.North);
                AddWall(AddDirection.East);
                AddWall(AddDirection.South);
                AddWall(AddDirection.West);
                break;
        }
    }
    
    private void AddFloorTile()
    {
        var v1 = new Vector3(Min.x, 0, Min.z);
        var v2 = v1 + new Vector3(0, 0, sizeZ);
        var v3 = v1 + new Vector3(sizeX, 0, sizeZ);
        var v4 = v1 + new Vector3(sizeX, 0, 0);
        
        mesh.AddQuad2(v1, v2, v3, v4);
    }

    private void SetCeilingTile()
    {
        var v1 = new Vector3(Min.x, Max.y, Min.z);
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

    private void OnDrawGizmos()
    {
        var center = transform.position;
        
        currentPos = center + new Vector3(0,50,0);

    }
}
