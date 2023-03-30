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
       
        DoPanel(currentPos, wallDirection, size);
        DoPanel(currentPos + new Vector3(100,0,0), wallDirection, size);
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

    static Vector3 RotateVectorAroundAxis(Vector3 vector, Vector3 axis, float degrees)
    {
        return Quaternion.AngleAxis(degrees, axis) * vector;
    }
     
    static Vector3 RotatePointAroundLine(Vector3 pointToRotate, Vector3 pointOnLine0, Vector3 pointOnLine1, float degrees)
    {
        Vector3 localVector = pointToRotate - pointOnLine0;
        Vector3 axis = pointOnLine1 - pointOnLine0;
        Vector3 rotatedVector = RotateVectorAroundAxis(localVector, axis, degrees);
        return rotatedVector + pointOnLine0;
    }
    
    private void DoPanel(Vector3 position, Vector3 inverseNormal, Vector3 aSize)
    {
        Vector3 aCrossForward = Vector3.Cross(inverseNormal, Vector3.up).normalized;
        Vector3 aCrossUp = Vector3.Cross(aCrossForward, inverseNormal).normalized;
        
        var start = startAxis;

        if (wallDirection.y != 0 && wallDirection.x + wallDirection.z == 0)
        {
            aCrossForward = Vector3.Cross(inverseNormal, Vector3.forward).normalized;       
        }

        Vector3 sumCross = (aCrossForward + aCrossUp).normalized;
        
        for (var i = 0; i < 4; i++)
        {
            var amount = Mathf.Sqrt(Mathf.Pow(aSize.x, 2) + Mathf.Pow(aSize.z, 2)) / 2;
            var betterAngle = Quaternion.AngleAxis(start, inverseNormal) *sumCross;
            Vector3 newSpot = position + (betterAngle.normalized * amount);
            var total = position +  Vector3.Scale((aSize / 2), betterAngle);

            corners[i] = newSpot;
            start += -90f;
        }
        lastVert = mesh.AddQuad2(corners[0], corners[1], corners[2], corners[3]);
    }
    
    // clockwise
    Vector3 Rotate90CW(Vector3 aDir)
    {
        return new Vector3(aDir.z, 0, -aDir.x);
    }
    // counter clockwise
    Vector3 Rotate90CCW(Vector3 aDir)
    {
        return new Vector3(-aDir.z, 0, aDir.x);
    }

    private void OnDrawGizmos()
    {
        var aColor = new Color(0.2f, 0.2f, 0.2f);
        var pos = transform.position;
        var normalDir = wallDirection.normalized;

        Vector3 aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        Vector3 aCrossUp = Vector3.Cross(aCrossForward, normalDir).normalized;
        
        if (wallDirection.y != 0 && wallDirection.x + wallDirection.z == 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;       
        }
        
        Vector3 sumCross = (aCrossForward + aCrossUp).normalized;
        var betterAngle = Quaternion.AngleAxis(startAxis, wallDirection) *sumCross;
        

        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + aCrossUp * 50);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(pos, pos + aCrossForward * 50);
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(pos, pos +(wallDirection * 50));
        var tAmount = Mathf.Round(size.x * Mathf.Pow(betterAngle.x, 2));
        var rightPos = pos + Vector3.right * (0.7f * 50);
        var extraPos = rightPos + Vector3.back * 40;

        var angleChange = Vector3.Angle(Vector3.forward, aCrossForward);
        var aSin = Mathf.Sin(angleChange * (Mathf.PI * 2) / 360);
        Debug.Log($"angelChange {angleChange}, tAmount {tAmount}, angle {betterAngle}, aSin {aSin * size.x / 2}");

        
        Gizmos.DrawLine(pos, rightPos);
        Gizmos.DrawLine(rightPos, extraPos);
        //Vector3.Max();
        
       // Gizmos.DrawWireSphere(pos, 50);

        for (int i = 0; i < 4; i++)
        {
            Debug.Log($"tAmount {tAmount}, angle {betterAngle}");
            betterAngle = Quaternion.AngleAxis(startAxis + -90 * i, wallDirection) *sumCross;
            var xAmount = Mathf.Round(size.x * Mathf.Pow(betterAngle.x, 2));
            var zAmount = Mathf.Round(size.z *  Mathf.Pow(betterAngle.z, 2));
            var yAmount = Mathf.Round(size.y * Mathf.Pow(betterAngle.y, 2));
            var amount = Mathf.Sqrt(Mathf.Pow(xAmount, 2) + Mathf.Pow(yAmount, 2)) / 2;
            var amount2 = Mathf.Sqrt(Mathf.Pow(zAmount, 2) + Mathf.Pow(yAmount, 2)) / 2;
            var amount3 = Mathf.Sqrt(Mathf.Pow(zAmount, 2) + Mathf.Pow(yAmount, 2)) / 2;
            Vector3 newSpot = pos + (betterAngle.normalized * 70);
            Vector3 newSpot2 = pos + (betterAngle.normalized * amount);
            Vector3 newSpot4 = pos + (betterAngle.normalized * amount2);
            Vector3 newSpot3 = pos + Vector3.Scale(betterAngle, new Vector3(xAmount, yAmount, zAmount));
            

            if (betterAngle.x < 0)
            {
                amount = -amount;
                xAmount = -xAmount;
            }

            if (betterAngle.y < 0)
            {
                amount = -amount;
                yAmount = -yAmount;
            }

            if (betterAngle.z < 0)
            {
                amount2 = -amount2;
                zAmount = -zAmount;
            }

            var aTotal = new Vector3(xAmount, yAmount, zAmount);
            
            Gizmos.color = Color.yellow;
            var theAmount = new Vector3(amount, 0, amount2);
          //  var scale = Vector3.Scale();
            Gizmos.DrawLine(pos, newSpot);

            Gizmos.color = aColor;
            //Gizmos.DrawSphere(newSpot, 4);
            //Gizmos.DrawSphere(pos + aTotal, 2);

            //Debug.Log($"amount {theAmount}, angle {betterAngle} angleChange {angleChange}, sum {sumCross} " +
            //          $"xAmount {xAmount}, yAmount{yAmount}, " +
            //          $"zAmount {zAmount}, " +
            //          $"hypAmount {amount}, hyp2 {amount2} , sumTotal{aTotal}, newSpot2 {newSpot2}, newSpot4 {newSpot4}");
            aColor += new Color(0.2f,0,0);
        }
    }
}