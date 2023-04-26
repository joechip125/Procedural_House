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
    public float sizeX, sizeY, sizeZ;
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

        var crossDir = Vector3.Cross(wallDirection, Vector3.up);
        SimplePanel(crossDir * -sizeX / 4, crossDir, new Vector2(sizeZ,sizeY));
        
        SimplePanel(crossDir * sizeX / 4, -crossDir,new Vector2(sizeZ, sizeY));
       
        SimplePanel(new Vector3(0,-sizeY / 4,0), new Vector3(0,1,0), new Vector2(sizeX, sizeZ));
        //SimplePanel(new Vector3(0,10,0), new Vector3(0,-1,0));
        //SimplePanel(new Vector3(0,10,-2.5f), new Vector3(0,0,-1));
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
    
    
    private void SimplePanel(Vector3 addPos, Vector3 normalDir, Vector2 theSize)
    {
        Vector3 aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        var flip = false;

        if (normalDir.y != 0 && normalDir.x + normalDir.z == 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
            flip = true;
        }

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp = Quaternion.AngleAxis(90 * i, normalDir) *aCrossForward;
            var aCrossUp2 = Quaternion.AngleAxis((90 * i) + 90, normalDir) *aCrossForward;
            
            var poss = new Vector3();
            var poss2 = new Vector3();

            if (!flip)
            {
                poss = (aCrossUp * (theSize.x / 2)) + addPos;
                poss2 = aCrossUp2 * (theSize.y / 2) + addPos;
            }

            else
            {
                poss = aCrossUp * theSize.y / 2 + addPos;
                poss2 = aCrossUp2 * theSize.x / 2 + addPos;
            }

            corners[i] = poss + poss2;
            
            flip = !flip;
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

        var flip = false;
        
        if (wallDirection.y != 0 && wallDirection.x + wallDirection.z == 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
            flip = true;
        }
        
        var theList = new List<Vector3>();
        
        for (int i = 0; i < 4; i++)
        {
            aCrossUp = Quaternion.AngleAxis(90 * i, wallDirection) *aCrossForward;
            var poss = aCrossUp * sizeX / 2;

            Gizmos.color = Color.green;
            
            if (!flip)
            {
                Gizmos.DrawLine(pos, pos + aCrossUp * sizeX/ 2);
                poss = aCrossUp * sizeX / 2;
            }
            else
            {
                Gizmos.DrawLine(pos, pos + aCrossUp * sizeY / 2);
                poss = aCrossUp * sizeY / 2;
            }
            
            theList.Add(poss);
            
            flip = !flip;
        }

        for (int i = 0; i < 4; i++)
        {
            Gizmos.color = Color.green;
            
            if (i > 2)
            {
                Gizmos.DrawSphere(pos +theList[i] + theList[0], 0.5f);
            }
            else
            {
                Gizmos.DrawSphere(pos +theList[i] + theList[i + 1], 0.5f);
            }
        }
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos +(wallDirection.normalized * 20));
    }
}