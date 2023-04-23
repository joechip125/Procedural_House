using System;
using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public class RoomSegments : MonoBehaviour
{
    private AdvancedMesh mesh;
    public float sizeX, sizeY;
    [SerializeField]private Vector3 size = new Vector3(100,100,100);
    [SerializeField] private Vector3 wallDirection;
    [SerializeField] private float startAxis;

    public int numberX, numberZ;

    private int lastVert;

    public Vector3 currentPos;
    
    private readonly Vector3[] corners = new[]
        {   new Vector3(-1, 0, -1), 
            new Vector3(0, 0, 1),
            new Vector3(1, 0, 1), 
            new Vector3(1, 0, 0) };

    private Segment[] segArray;

    public float aDegree;
    
    
    private void Start()
    {
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        currentPos = transform.position;
       
        DoPanel(currentPos, wallDirection, size);
        DoPanel(currentPos + new Vector3(100,0,0), wallDirection, size);
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
    
    private Vector3 PanelRotation(Vector3 direction, float angle)
    {
        var cross = Vector3.Cross(Vector3.up, direction);

        if (direction.y != 0)
        {
            cross = Vector3.up.y > 0 ? Vector3.Cross(Vector3.forward, Vector3.right) 
                : Vector3.Cross(Vector3.forward, -Vector3.right);
        }

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
            
            var betterAngle2 = Quaternion.AngleAxis(startAxis + -90 * i, wallDirection) *aCrossForward;
            var betterAngle3 = Quaternion.AngleAxis(startAxis + -90 * i, wallDirection) *aCrossUp;
            
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
        
        Vector3 aCrossOther = Vector3.Cross(normalDir, Vector3.forward).normalized;
        Vector3 aCrossOther2 = Vector3.Cross(aCrossOther, normalDir).normalized;
        
        if (wallDirection.y != 0 && wallDirection.x + wallDirection.z == 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
            aCrossUp = Quaternion.AngleAxis(-90, wallDirection) *aCrossForward;
        }
        
        Vector3 sumCross = (aCrossForward + aCrossUp).normalized;
        var betterAngle = Quaternion.AngleAxis(startAxis, wallDirection) *sumCross;
        
        
        for (int i = 0; i < 1; i++)
        {
            aCrossUp = Quaternion.AngleAxis(90 * i, wallDirection) *aCrossForward;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(pos, pos + aCrossUp * sizeX/ 2);
        
            aCrossUp = Quaternion.AngleAxis(0, wallDirection) *aCrossForward;
            Gizmos.color = Color.blue;
            Gizmos.DrawLine(pos, pos + aCrossUp * sizeY / 2);
        }
        
        Gizmos.color = Color.green;
        //Gizmos.DrawLine(pos, pos + aCrossUp * sizeX/ 2);
        //Gizmos.DrawLine(pos, pos + aCrossUp * sizeY/ 2);
        //
        //var thePos = pos + aCrossForward * sizeY/ 2 - new Vector3(0,0,1) * sizeX / 2;
        //Gizmos.DrawSphere(thePos, 0.5f);
        //
        //var thePos2 = pos + aCrossForward * -sizeY + aCrossUp * sizeX;
        //Gizmos.DrawSphere(thePos2, 1);
        //
        //var thePos3 = pos + aCrossForward * -sizeY + aCrossUp * -sizeX;
        //Gizmos.DrawSphere(thePos3, 1);
        //
        //var thePos4 = pos + aCrossForward * sizeY + aCrossUp * -sizeX;
        //Gizmos.DrawSphere(thePos4, 1);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos +(wallDirection.normalized * 50));

        for (int i = 0; i < 4; i++)
        {
            //betterAngle = Quaternion.AngleAxis(startAxis + -90 * i, wallDirection) *sumCross;
            //var betterAngle2 = Quaternion.AngleAxis(startAxis + -90 * i, wallDirection) *aCrossForward;
            //var betterAngle3 = Quaternion.AngleAxis(startAxis + -90 * i, wallDirection) *aCrossUp;
            //
            //var pos5 = betterAngle2.normalized * sizeX;
            //var pos6 = betterAngle3.normalized * sizeY;
            //
            //var pos4 = betterAngle2.normalized * sizeX + betterAngle3 * sizeY;
            //var posX = Mathf.Pow(betterAngle.normalized.x, 2) * sizeX;
            //var posY = Mathf.Pow(betterAngle.y, 2) * sizeY;
            //var posZ = Mathf.Pow(betterAngle.normalized.z, 2) * sizeX;
            //
            //
            //if (betterAngle.y < 0)
            //{
            //    posY = -posY;
            //}
            //if (betterAngle.x < 0)
            //{
            //    posX = -posX;
            //}
            //if (betterAngle.z < 0)
            //{
            //    posZ = -posZ;
            //}
            //
            //Gizmos.color = Color.black;
            //Gizmos.DrawLine(pos, pos + pos4 / 2);
            //
            //Gizmos.color = Color.yellow;
            //Gizmos.DrawLine(pos, pos + pos4 / 2);
            //
            //Gizmos.color = aColor;
            ////Gizmos.DrawSphere(pos + pos5 / 2, 2);
            //Gizmos.DrawSphere(pos + pos6 / 2 + pos5 / 2, 2);
            //
            //aColor += new Color(0.2f,0,0);
        }
    }
}