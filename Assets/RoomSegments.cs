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
    [SerializeField]private Vector3 size = new Vector3(100,100,100);
    [SerializeField] private Vector3 wallDirection;
    [SerializeField] private float startAxis;

    public int numberX, numberZ;

    private int lastVert;

    public Vector3 currentPos;
    public Vector3 Max => currentPos + new Vector3(sizeX / 2, sizeY, sizeZ/ 2);
    public Vector3 Min => currentPos - new Vector3(sizeX / 2, 0, sizeZ / 2);

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
        //InitSegment();
        //MoveStuff(new Vector3(-1,0,0));
        ////AddFloorTile();
        //
        //var ang =Quaternion.AngleAxis(90, new Vector3(1,0,0)) * new Vector3(0,1,0);
        DoPanel(currentPos + new Vector3(50,0,0), Vector3.right, currentPos);
    }
    

    private void MoveStuff(Vector3 startDir)
    {
        var z = 0;
        var x = numberX;
        var first =  Find(3, 1).wallStarts[0];
        var second =  Find(3, 2).wallStarts[0];
        if (startDir.x != 0)
        {
          first =  Find(0, 1).wallStarts[0];
          second =  Find(0, 2).wallStarts[0];
        }

        var newAx = Quaternion.AngleAxis(90, new Vector3(0,1,0)) * startDir;
        
        var placePos = mesh.GetPositionAtVert(first + 3);
        AddDoor(placePos, startDir);
        
        mesh.MoveVertices(new Dictionary<int, Vector3>()
        {
            {first + 2,-newAx * 50},
            {first + 3,-newAx * 50},
            {second , newAx * 50},
            {second + 1,newAx * 50}
        });
    }

    private void AddDoor(Vector3 start, Vector3 direction)
    {
        var doorLength = 10;
        var newAx = Quaternion.AngleAxis(90, new Vector3(0,1,0)) * direction;
        var currHold = currentPos;
        currentPos = start + direction * doorLength / 2;
        SetAPanel(new Vector3(-1, 0, -1), new Vector3(10,0, 100), new Vector3(0, 1, 0));

        currentPos += new Vector3(0, 50, 0);
        SetAPanel(new Vector3(-1,-1,0), 
            new Vector3(10,100,0), 
            new Vector3(0, 0,-1),
            currentPos + newAx * 50);
        
        SetAPanel(new Vector3(-1,-1,0), 
            new Vector3(10,100,0), 
            new Vector3(0, 0,1),
            currentPos + -newAx * 50);
        currentPos = currHold;
    }


    private Segment Find(int x, int z)
    {
        return segArray[z * numberX + x];
    }

    private void InitSegment()
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
                segArray[total] = new Segment()
                {
                    position = currentPos
                };

                if (j == 0)
                {
                    AddSomeWalls(new Vector3(-1,0,0));
                    segArray[total].wallStarts.Add(lastVert);
                }

                if (j == numberX - 1)
                {
                    AddSomeWalls(new Vector3(1,0,0));
                    segArray[total].wallStarts.Add(lastVert);
                }

                if (i == 0)
                {
                    AddSomeWalls(new Vector3(0,0,-1));
                    segArray[total].wallStarts.Add(lastVert);
                }

                if (i == numberZ - 1)
                {
                    AddSomeWalls(new Vector3(0,0,1));
                    segArray[total].wallStarts.Add(lastVert);
                }

                currentPos += new Vector3(sizeX, 0, 0);
                total++;
            }    
        }
        mesh.AddCollider();
    }

    private void RotateDirection()
    {
        
    }
    
    private void SetAPanel(Vector3 startDir, Vector3 theSize, Vector3 axis)
    {
        for (var i = 0; i < 4; i++)
        {
            corners[i] = currentPos+ Vector3.Scale(theSize / 2, 
                Quaternion.AngleAxis(90 * i, axis) * startDir);
        }
        lastVert = mesh.AddQuad2(corners[0], corners[1], corners[2], corners[3]);
    }
    
    private void SetAPanel(Vector3 startDir, Vector3 theSize, Vector3 axis, Vector3 position)
    {
        for (var i = 0; i < 4; i++)
        {
            corners[i] = position+ Vector3.Scale(theSize / 2, 
                Quaternion.AngleAxis(90 * i, axis) * startDir);
        }
        lastVert = mesh.AddQuad2(corners[0], corners[1], corners[2], corners[3]);
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
            SetAPanel(new Vector3(-startDir.z,-1,0), size, new Vector3(0, 0,-startDir.z));
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
    
    private void SetCeilingTile()
    {
        var v1 = new Vector3(Min.x, Max.y, Min.z);
        var v2 = v1 + new Vector3(sizeX, 0, 0);
        var v3 = v1 + new Vector3(sizeX, 0, sizeZ);
        var v4 = v1 + new Vector3(0, 0, sizeZ);
        
        mesh.AddQuad2(v1, v2, v3, v4);
    }

    private Vector3 PanelRotation(Vector3 direction, float angle)
    {
        var cross = Vector3.Cross(Vector3.up, direction);
        
      //  Quaternion.AngleAxis(angle, direction)* cross;

        if (direction.y != 0)
        {
            cross = Vector3.up.y > 0 ? Vector3.Cross(Vector3.forward, Vector3.right) 
                : Vector3.Cross(Vector3.forward, -Vector3.right);
        }
//        Debug.Log($"Panel Rotation: cross {cross}, angle {Quaternion.AngleAxis(angle, direction)* cross}");
        return Quaternion.AngleAxis(angle, direction)* cross;
    }

    private void DoPanel(Vector3 position, Vector3 inverseNormal, Vector3 originalPos)
    {
        var aSize = new Vector3(100, 100, 100);
        var start = 45f;
        
        Vector3 dir = position-originalPos;
        
        Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
        var anAngle = Quaternion.AngleAxis(start, wallDirection) * left;
        
        for (var i = 0; i < 4; i++)
        {
            anAngle = Quaternion.AngleAxis(start +(-90* i), wallDirection) * left;
            var amount = Vector3.Scale((aSize / 2), anAngle);
            Debug.Log($"runtime angle {anAngle}, index {i}, amount {amount}");
            
            var startDir =  PanelRotation(inverseNormal, start +(-90* i));
            corners[i] = position+ Vector3.Scale(aSize / 2, 
                Quaternion.Euler(PanelRotation(inverseNormal, start + (-90 * i))) * startDir);
        }
        lastVert = mesh.AddQuad2(corners[0], corners[1], corners[2], corners[3]);
        
        //corners[i] = position+ Vector3.Scale(theSize / 2, 
        //    Quaternion.AngleAxis(90 * i, axis) * startDir);
    }

    private void OnDrawGizmos()
    {
        var aColor = new Color(0.2f, 0.2f, 0.2f);
        var pos = transform.position;
        var anAngle = transform.eulerAngles;
        Vector3 forward = Quaternion.Euler(anAngle) * Vector3.forward;
        Vector3 up = Quaternion.Euler(anAngle) * Vector3.up;
        Vector3 right = Quaternion.Euler(anAngle) * Vector3.right;

        var startDirection = Vector3.up;
        var sAngle = 180f;

        var newAxFor = Quaternion.AngleAxis(sAngle, startDirection) * Vector3.forward;
        var newAxRight = Quaternion.AngleAxis(sAngle, startDirection) * Vector3.right;
        var use = up;
        var sumAxis = newAxFor + newAxRight;

        var cross = Vector3.Cross(newAxFor, newAxRight);
        var cross3 = Vector3.Cross(Vector3.forward, -Vector3.right);
        var cross2 = Vector3.Cross(Vector3.forward, up);

        var start = -135f;
        var angg2 = Quaternion.AngleAxis(-135f - 90, Vector3.up) * cross;
        //Debug.Log($"plus {cross} minus{cross3} cross angle {angg}, angle two {angg2}");
        var first = pos + use * 40;
        var second = pos + newAxFor * 40;
        var third = pos + newAxRight * 40;
        var fourth = pos + sumAxis * 40;
        var fifth = pos + wallDirection * 40;


        Vector3 dir = fifth - pos;
        Vector3 left = Vector3.Cross(dir, Vector3.up).normalized;
        var angg = Quaternion.AngleAxis(startAxis, wallDirection) * left;
        var sixth = fifth + angg * 40;

        //Vector3 right = -left;
        //Vector3 right = Vector3.Cross(Vector3.up, dir).normalized;
        //Vector3 right = Vector3.Cross(-dir, Vector3.up).normalized;
        //Vector3 right = Vector3.Cross(dir, -Vector3.up).normalized;

       // Debug.Log(
       //     $"up angle {up}, cross {cross}, newAxFor {newAxFor}, newAxRight{newAxRight}, angg {angg} , left {left}");

        //Gizmos.DrawLine(pos, first);

        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(pos, fifth);
        Gizmos.DrawLine(fifth, sixth);

        // Gizmos.color = Color.green;
        // Gizmos.DrawLine(pos, third);
        // 
        // Gizmos.color = Color.red;
        // Gizmos.DrawLine(pos, fourth);

        var deg = startAxis;

        for (int i = 0; i < 4; i++)
        {
            angg = Quaternion.AngleAxis(deg, wallDirection) * left;
            sixth = fifth + angg * 40;
           // Debug.Log($"current {angg}");
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(fifth, sixth);

            deg -= 90f;
        }
    }
}