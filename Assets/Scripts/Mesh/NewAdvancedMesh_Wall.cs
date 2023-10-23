using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;
using Color = UnityEngine.Color;
using Matrix4x4 = UnityEngine.Matrix4x4;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = System.Numerics.Vector4;

[Serializable]
public class BaseWall
{
    public Vector3 center;
    public Vector3 size;
    public int firstVert;
    public List<Vector3> points = new();
}

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{
    private Dictionary<Vector3,BaseWall> cornerDict = new();

    private int lastVert;
    
    public Material aMaterial;

    private Dictionary<Vector3, int> vertIndices = new ();
    private List<Vector3> testPos = new();
    private List<Vector3> cornerPos = new();
    private List<int> vList = new();
    
    private void Awake()
    {
        Activate();
    }
    
    protected override void Activate()
    {
        base.Activate();
        InitMesh();
        ApplyMaterial(aMaterial);
    }

    private void ManyCorners()
    {
        cornerDict.Clear();
        var pos = transform.position;
        var size = new Vector3(300,1,600);
        AddCornersToDict(pos,size, Vector3.up, Vector3.zero);
        MoreCornersToDict(0, Vector3.zero);
        var count = 0;
        
        foreach (var cD in cornerDict)
        {
            foreach (var p in cD.Value.points)
            {
                PlaceDot(Color.red,  p, count++);    
            }

            count = 0;
        }
    }

    private void AddCorners(Vector3 pos, Vector3 size, Vector3 normal)
    {
        MathHelpers.PlaneDirections(normal, out var pUp, out var pRight);
       
        for (int i = 0; i < 4; i++)
        {
            var cAngle = Quaternion.AngleAxis(90 * i, normal) *pRight;
            var cAngle2 = Quaternion.AngleAxis(90 * i, normal) *pUp;
            var aPlace = pos + cAngle.normalized * (i % 2 != 0 ? size.x : size.z) / 2;
            var bPlace = pos + cAngle2.normalized * (i % 2 != 0 ? size.z : size.x) / 2;
          
            cornerPos.Add(bPlace + aPlace);
        }
    }
    
    private void AddCornersToDict(Vector3 pos, Vector3 size, Vector3 normal, Vector3 index)
    {
        MathHelpers.PlaneDirections(normal, out var pUp, out var pRight);
        var theBase = new BaseWall()
        {
            center = pos,
            firstVert = 0,
            size = size
        };

        for (int i = 0; i < 4; i++)
        {
            var cAngle = Quaternion.AngleAxis(90 * i, normal) *pRight;
            var cAngle2 = Quaternion.AngleAxis(90 * i, normal) *pUp;
            var aPlace = pos + cAngle.normalized * (i % 2 != 0 ? size.x : size.z) / 2;
            var bPlace = pos + cAngle2.normalized * (i % 2 != 0 ? size.z : size.x) / 2;
            
            theBase.points.Add(bPlace + aPlace);
        }
        
        cornerDict.Add(index, theBase);
    }
    
    private bool MoreCornersToDict(int cornNum, Vector3 index)
    {
        if (!cornerDict.ContainsKey(index)) return false;
        var aCorner = cornerDict[index];

        GetPositionFromCorner(0, Vector3.zero, 0.5f, out var aPos);
        
        var newSize = new Vector3(200, 0, 200);
        var time = 1f;
        var nextNum = cornNum == 3 ? 0 : cornNum + 1;
        var dir = aCorner.points[nextNum] - aCorner.points[cornNum];
        var norm = Vector3.Cross(Vector3.up, dir.normalized);
        var aStart = Vector3.Lerp(aCorner.points[cornNum], aCorner.points[nextNum], time);

        PlaceDot(Color.green,  aPos, 1);
        PlaceDot(Color.green, aStart  + Vector3.Scale(-norm, newSize / 2), 2);
        return true;
    }

    private bool GetPositionFromCorner(int cornNum, Vector3 index, float time, out Vector3 newPos)
    {
        newPos = Vector3.zero;
        if (!cornerDict.ContainsKey(index)) return false;
        var nextNum = cornNum == 3 ? 0 : cornNum + 1;
        newPos = Vector3.Lerp(cornerDict[index].points[cornNum], cornerDict[index].points[nextNum], time);
        
        return true;
    }
    
    private void MoreCorners(int cornNum)
    {
        var newSize = new Vector3(200, 0, 200);
        var interp = 1f;
        var nextNum = cornNum == 3 ? 0 : cornNum + 1;
        var dir = cornerPos[nextNum] - cornerPos[cornNum];
        var norm = Vector3.Cross(Vector3.up, dir.normalized);
        var aStart = Vector3.Lerp(cornerPos[cornNum], cornerPos[nextNum], interp);

        PlaceDot(Color.green,  aStart, 1);
        PlaceDot(Color.green, aStart  + Vector3.Scale(-norm, newSize / 2), 2);
    }

    private int StackEm(Vector3 pos, int count, Vector2 startEnd, float height, int numDots)
    {
        var outVal = count;
        
        for (int i = 0; i < numDots; i++)
        {
            var next = pos + Vector3.up * (height / numDots);

            if (pos.y > startEnd.x && pos.y < startEnd.y)
            { 
                PlaceDot(Color.green, pos, outVal++);
                if (next.y > startEnd.y)
                {
                    PlaceDot(Color.green, new Vector3(pos.x, startEnd.y + 10, pos.z), outVal++);
                }
            }
            else if(pos.y < startEnd.x || pos.y > startEnd.y)
            {
                PlaceDot(Color.red, pos, outVal++);
                if (next.y > startEnd.x && next.y < startEnd.y)
                {
                    PlaceDot(Color.green, new Vector3(pos.x, startEnd.x + 10, pos.z), outVal++);
                }
            }
            pos += Vector3.up * (height / numDots);
        }

        return outVal;
    }

    private bool IsPointInSquare(Vector3 min, Vector3 max, Vector3 point)
    {
        if (point.x < min.x || point.y < min.y || point.z < min.z) return false;
        if (point.x > max.x || point.y > max.y || point.z > max.z) return false;
        
        return true;
    }

    private Vector2 WhereIsPoint(Vector3 min, Vector3 max, Vector3 point)
    {
        var outVec = new Vector2();
        
        var calc = point.x + point.z;
        var calc2 = min.x + min.z;
        var calc3 = max.x + max.z;
        
        if (point.y < min.y) outVec.y = -1;
        else if (point.y >= min.y) outVec.x = point.y < max.y ? 0 : 1;
        
        if (calc < calc2) outVec.x = -1;
        else if (calc >= calc2) outVec.x = calc < calc3 ? 0 : 1;
        return outVec;
    }
    
    private void DrawLine(Vector3 origin, Vector3 end, Color color, string label = "")
    {
        Gizmos.color = color;
        var place = origin + (end - origin) / 2;
        Handles.Label(place, label);
        Gizmos.DrawLine(origin, end);
    }
    
    private void PlaceDot(Color color, Vector3 pos, int count)
    {
        Gizmos.color = color;
        Handles.Label(pos,$"{count}");
        Gizmos.DrawSphere(pos, 3);
    }
    
    private void SideVerts(Vector3 pos, Vector3 dir, Vector2 innerSize)
    {
        var start = Vertices.Count;
        var outer = new Vector2(200, 400) - innerSize;
        var adjustIn = new Vector3(0,(innerSize.y / 2) - 5);
        
        for (int i = 0; i < 2; i++)
        {
            if(i == 1) SetSquare(dir, pos+ adjustIn, innerSize);
            else SetSquare(dir, pos, innerSize);
            
            foreach (var t in Corners)
            {
                Vertices.Add(t);
            }
            innerSize += outer;
        }

        for (int i = 0; i < Corners.Count; i++)
        {
            var current = start + i;

            if (i == Corners.Count - 1)
            {
                Triangles.Add(current);
                Triangles.Add(current + 4);
                Triangles.Add(start + 4); 
                
                Triangles.Add(current);
                Triangles.Add(start + 4);
                Triangles.Add(start);  
                continue;
            }
            Triangles.Add(current);
            Triangles.Add(current + 4);
            Triangles.Add(current + 5);  
            
            Triangles.Add(current + 1);
            Triangles.Add(current);
            Triangles.Add(current + 5);  
        }
        UpdateMesh();
    }
    
    private void SetPoints(Color pointColor,int num)
    {
        Gizmos.color = pointColor;
        foreach (var c in Corners)
        {
            Gizmos.DrawSphere(c, 4);
            Handles.Label(c, $"{num++}");
        }
    }
    
    private void BaseWall()
    {
        testPos.Clear();
        cornerDict.Clear();
        lastVert = 0;
        var pos = transform.position;

        DrawCorner(pos);
    }

    private void DrawCorner(Vector3 pos)
    {
        var size = new Vector3(200, 1,200);
        var size3 = new Vector3(200, 200);
        var corner = new Vector3(15, 15);
        var corner2 = new Vector3(15, 1, 15);
        
        FourCorners(pos, size + corner);
        DrawACube(pos, size3, Color.green);
        
        AddSquare(pos, size + corner, Vector3.zero);
        
        DrawCorners();
    }

    private void DrawACube(Vector3 pos, Vector2 size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawCube(pos, new Vector3(size.x, 1, size.y));
    }

    private void DrawCorners()
    {
        foreach (var t in testPos)
        {
            PlaceDot(Color.red, t, lastVert++);
        }
        
        foreach (var c in cornerDict)
        {
            var first = c.Value.firstVert;
            for (int i = 0; i < 4; i++)
            {
                DrawLine(testPos[first+i],testPos[i >= 3 ? first : first+i+1], Color.green);
            }
        }
    }
    
    private void FourCorners(Vector3 center, Vector3 size)
    {
        cornerPos.Clear();
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);

        var mag = Mathf.Sqrt(Mathf.Pow(size.y / 2, 2) + Mathf.Pow(size.x / 2, 2));
        var tan =Mathf.Atan(size.y / size.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(size.x / size.y) * (180 / Mathf.PI);
        
        for (int i = 0; i < 4; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis(remain + 90 * i, Vector3.up) *pRight;
            cornerPos.Add(center + cAngle.normalized * mag);
        }
    }
    
    private void AddSquare(Vector3 center, Vector3 size, Vector3 newIndex)
    {
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        var bWall = new BaseWall()
        {
            center = center,
            size = size,
            firstVert = testPos.Count
        };
        
        var mag = Mathf.Sqrt(Mathf.Pow(size.y / 2, 2) + Mathf.Pow(size.x / 2, 2));
        var tan =Mathf.Atan(size.y / size.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(size.x / size.y) * (180 / Mathf.PI);
        var add = 45;

        for (int i = 0; i < 4; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis( add +90 * i, Vector3.up) *pRight;
            testPos.Add(center + cAngle.normalized * mag);
        }
        cornerDict.Add(newIndex, bWall);
    }
    
    private int FindClosestPoint(Vector3 wIndex, Vector3 point)
    {
        var first = cornerDict[wIndex].firstVert;
        var minDist = Vector3.Distance(testPos[first], point);
        var rPoint = first;

        for (int i = first; i < first + 4; i++)
        {
            var dist  = Vector3.Distance(testPos[i], point);

            if (!(dist < minDist)) continue;
            minDist = dist;
            rPoint = i;
        }
        return rPoint;
    }
    
    private void FindClosestPoints(Vector3 point)
    {
        vList.Clear();

        foreach (var c in cornerDict)
        {
            var f = c.Value.firstVert;
            var minDist = Single.PositiveInfinity;
            var rPoint = f;
            for (int i = f; i < f + 4; i++)
            {
                var dist  = Vector3.Distance(testPos[i], point);

                if (!(dist < minDist)) continue;
                minDist = dist;
                rPoint = i;
            }
            vList.Add(rPoint);
        }
    }
    private void FindDistantPoints(Vector3 point)
    {
        vList.Clear();

        foreach (var c in cornerDict)
        {
            var f = c.Value.firstVert;
            var max = Single.NegativeInfinity;
            var rPoint = f;
            for (int i = f; i < f + 4; i++)
            {
                var dist  = Vector3.Distance(testPos[i], point);

                if (dist < max) continue;
                max = dist;
                rPoint = i;
            }
            vList.Add(rPoint);
        }
    }
    
    private void ConnectDots(Vector3 wIndex, Vector3 wNext, Vector3 testPoint)
    {
        DrawLine(testPos[FindClosestPoint(wIndex, testPoint)],testPos[FindClosestPoint(wNext, testPoint)], Color.blue);
    }
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        if (Application.isPlaying)
        {
            ManyCorners();
            return;
        }
        BaseWall();
    }
}
