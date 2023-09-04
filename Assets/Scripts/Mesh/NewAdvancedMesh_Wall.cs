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
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = System.Numerics.Vector4;

[Serializable]
public class RoomSegment
{
    public Dictionary<Vector3,BaseWall> wallSegments = new();
    public Vector3 center;
    public Vector3 size;
}

[Serializable]
public class BaseWall
{
    public Vector3 center;
    public Vector3 size;
    public int firstVert;
}

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{
    [SerializeField] private float adjacent;

    private Dictionary<Vector3,BaseWall> cornerDict = new();
    private Dictionary<Vector3,RoomSegment> segmentDict = new();
    
    private int lastVert;
    
    public Material aMaterial;

    private Dictionary<Vector3, int> vertIndices = new ();
    private List<Vector3> testPos = new();
    private Dictionary<Vector3, Vector3> cornerPos = new();
    private List<Vector3> indices = new();
    private List<int> vList = new();
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
    }
    
    protected override void Activate()
    {
        base.Activate();
        ApplyMaterial(aMaterial);
        InitMesh();
    }

    private void TunnelVerts(Vector3 center, Vector3 aDir, Vector3 size)
    {
        SetSquare(aDir, center, new Vector2(size.x, size.y));
        var add = Vertices.Count;
        var first = add;

        foreach (var c in Corners)
        {
            Vertices.Add(c);
            Vertices.Add(c + aDir * size.z);
        }
        
        for (int i = 0; i < Corners.Count; i++)
        {
            if (i == Corners.Count - 1)
            {
                Triangles.Add(add);
                Triangles.Add(add + 1);
                Triangles.Add(first + 1);
                
                Triangles.Add(first + 1);
                Triangles.Add(first);
                Triangles.Add(add);
                continue;
            }
            Triangles.Add(add);
            Triangles.Add(add + 1);
            Triangles.Add(add + 2);
            
            Triangles.Add(add + 1);
            Triangles.Add(add + 3);
            Triangles.Add(add + 2);
            
            add+= 2;
        }
        UpdateMesh();
    }
    
    private void PlaneDirections(Vector3 normal, out Vector3 planeUp, out Vector3 planeRight)
    {
        planeUp = Vector3.ProjectOnPlane(normal.x + normal.z == 0 
            ? new Vector2(1,0) : new Vector2(0,1), normal);
        planeRight = Vector3.Cross(normal, planeUp);
    }

    private void VizPlane(Vector3 pos)
    {
        lastVert = 0;
        vertIndices.Clear();
        testPos.Clear();
        var myDir = Vector3.right;
        var pSize = new Vector2(200, 400);
        var corns = new Vector3[4];
        var counter = 0;
        PlaneDirections(myDir, out var pUp, out var pRight);
        var endY = pUp.normalized * Mathf.Sqrt(Mathf.Pow(pSize.y, 2))/ 2;
        var endX = pRight.normalized * Mathf.Sqrt(Mathf.Pow(pSize.x, 2))/ 2;
        var rez = 2;
        var xR = (int)(pSize.x / 100) * rez;
        var yR = (int)(pSize.y / 100) * rez;

        var test = Mathf.Sqrt(Mathf.Pow(pSize.y / 2, 2) + Mathf.Pow(pSize.x / 2, 2));
        var tan =Mathf.Atan(pSize.y / pSize.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(pSize.x / pSize.y) * (180 / Mathf.PI);
        
        for (int i = 0; i < corns.Length; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            
            var cAngle = Quaternion.AngleAxis(remain + 90 * i, myDir) *pRight;

            corns[i] = pos + cAngle.normalized * test;
        }
        
        var nextC = 0;
        var flip = true;
        var first = counter;
        var dotPos = new List<Vector3>();
        
        for (int i = 0; i < corns.Length; i++)
        {
            nextC = i == corns.Length - 1 ? 0 : nextC + 1;
            DrawLine(corns[i], corns[nextC],  Color.green, $"");
            var pos2 = corns[i];
            var dir = (corns[nextC] - pos2).normalized;
            var currNum = flip ? xR : yR;
            
            for (int j = 0; j < currNum; j++)
            {
                dotPos.Add(pos2);
                if (flip) pos2 += dir * pSize.x / currNum;
                else pos2 += dir * pSize.y / currNum;
            }
            flip = !flip;
        }

        nextC = 0;
        for (int i = 0; i < 4; i++)
        {
            nextC = i == corns.Length - 1 ? 0 : nextC + 1;
            var anAngle3 = Vector3.Angle(corns[i], corns[nextC]);
        }
        
        DrawLine(pos, pos + pRight * 100, Color.magenta);
        var aPoint = GetDots2(corns[0], pUp, pRight, Vector3.zero, 400);
        ExtendDots(new Vector3(1,0), 5, aPoint, pRight);
    }

    private void ExtendDots(Vector3 firstIndex,int numTiles, int startDot, Vector3 dir)
    {
        var sing = 40;
        var start = testPos[vertIndices[firstIndex]]+ dir * sing;
        var start2 = testPos[vertIndices[firstIndex + Vector3.up]] + dir * sing;
        
        for (int i = 0; i < numTiles; i++)
        {
            PlaceDot(Color.red, start, startDot++);
            PlaceDot(Color.red, start2, startDot++);
            start +=  dir * sing;
            start2 +=  dir * sing;
        }
    }
    private int GetDots2(Vector3 start, Vector3 planeU, Vector3 planeR, Vector3 firstIndex, float len)
    {
        var dotC = 0;
        var rez = 6;
        var size = new Vector2(30, 400);
        var incY = size.y / rez;
        
        for (int i = 0; i <= rez; i++)
        {
            testPos.Add(start);
            var next = start + planeR * size.x;
            PlaceDot(Color.red, start, dotC);
            vertIndices.Add(firstIndex, dotC++);
            testPos.Add(next);
            PlaceDot(Color.red, next, dotC);
            vertIndices.Add(firstIndex+ Vector3.right, dotC++);
            start += planeU * incY;
            firstIndex += Vector3.up;
        }

        return dotC - 1;
    }

    private void GetDots(Vector3 dir,float extent, float angle, List<Vector3> dotPos)
    {
        var test = dir * extent;
        var count = 0;
        var center = transform.position;
        var selection = dotPos
            .Where(x =>
            {
                var pex = (x - center).normalized;
                var dot = Vector3.Dot(dir, pex);
                var angle2 = Vector3.Angle(dir, pex);
                
                if (angle2 <= angle)
                {
                    return true;
                }
                
                return false;
            }).ToList();

        PlaceDot(Color.black, dir * extent, 0);
        foreach (var s in selection)
        {
            PlaceDot(Color.magenta,s + dir * 20, count++);
        }
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
    
    private void TangentWall(Vector3 aDir)
    {
        var pos = transform.position;
        var arcTan = 100f / adjacent;
        var newVal = arcTan * adjacent;
        var addDegrees = 0;
        var start = pos + aDir * adjacent;
        var nextDir = Vector3.Cross(Vector3.up, aDir);
        var hypo2 =Mathf.Sqrt( Mathf.Pow(adjacent, 2) + Mathf.Pow(100,2));
        var opposite2 = adjacent * (Mathf.Sin(Mathf.PI / 180 * addDegrees) / Mathf.Cos(Mathf.PI / 180 * addDegrees));
        var angle2= Quaternion.AngleAxis(23, Vector3.up) *aDir;
        Gizmos.DrawLine(pos, pos + aDir * adjacent);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, start + nextDir * -500);

        Gizmos.color = Color.yellow;
        for (int i = 0; i < 4; i++)
        {
            var opposite = adjacent * (Mathf.Sin(Mathf.PI / 180 * addDegrees) / Mathf.Cos(Mathf.PI / 180 * addDegrees));
            var hypo= Mathf.Sqrt( Mathf.Pow(adjacent, 2) + Mathf.Pow(opposite,2));
            var angle= Quaternion.AngleAxis(addDegrees, Vector3.up) *aDir;
            Gizmos.DrawLine(pos, pos + angle * hypo);
            addDegrees += 10;
        }
    }

    private void BaseWall()
    {
        testPos.Clear();
        cornerDict.Clear();
        segmentDict.Clear();
        lastVert = 0;
        
        var pos = transform.position;

        DrawCorner(pos);
    }

    private void DrawCorner(Vector3 pos)
    {
        var size = new Vector3(200, 1,200);
        var size3 = new Vector3(200, 200);
        var size2 = new Vector3(100, 100);
        var corner = new Vector3(15, 15);

        SquareSegment(pos, size3, corner, Vector3.right);
        DrawACube(pos, size, Color.green);

        var amount = ((size.x + corner.x) / 2) + ((size2.x + corner.x) / 2);
        var next = pos + Vector3.right * amount;

        foreach (var c in cornerPos)
        {
           // DrawACube(c.Value, new Vector3(corner.x,1, corner.y), Color.red);
        }
        var cAngle = Quaternion.AngleAxis(90, Vector3.up) * Vector3.right;
        var cAngle2 = Quaternion.AngleAxis(180, Vector3.up) * Vector3.right;
        var aScale = Vector3.Scale(size + corner, cAngle) / 2;
        
        Debug.Log($"angle {cAngle} angle2 {cAngle2} scale {aScale} size {size + corner}");
        DrawACube(pos + aScale, new Vector3(corner.x,1, 200), Color.blue);
        DrawCorners(pos);
    }

    private void DrawACube(Vector3 pos, Vector3 size, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawCube(pos, size);
    }
    
    private void AddSegment(Vector3 pos, Vector3 roomS, Vector3 cornerS, Vector3 currIndex)
    {
        var segment = new RoomSegment();

        FourCorners(pos, roomS + cornerS, currIndex);

        foreach (var cPos in cornerPos)
        {
            var seg = AddSquare(cPos.Value, cornerS, cPos.Key);
            segment.wallSegments.Add(cPos.Key, seg);
           // PlaceDot(Color.green, cPos.Value, lastVert++);
        }
        segmentDict.Add(currIndex, segment);
        
        foreach (var w in segment.wallSegments)
        {
            PlaceDot(Color.green, w.Value.center, lastVert++);
        }
    }

    private void SquareSegment(Vector3 pos, Vector3 roomS, Vector3 cornerS, Vector3 currIndex)
    {
        FourCorners(pos, roomS + cornerS, currIndex);

        foreach (var cPos in cornerPos)
        {
            AddSquare(cPos.Value, cornerS, cPos.Key);
        }
    }
    
    private void DrawCorners(Vector3 pos)
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
    
    private Vector3 GetIndexFromAngle(float angle)
    {
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        var cAngle = Quaternion.AngleAxis(angle, Vector3.up) *pRight;
        var index = new Vector3(Mathf.Round(cAngle.x), Mathf.Round(cAngle.y), Mathf.Round(cAngle.z));
        
        return index;
    }
    private void SetIndexFromAngle(float angle, Vector3 size)
    {
        indices.Clear();
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        
        var tan =Mathf.Atan(size.y / size.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(size.x / size.y) * (180 / Mathf.PI);
        var count = 360 / angle;
        
        for (int i = 0; i < count; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis(remain + angle * i, Vector3.up) *pRight;
            indices.Add(cAngle);
        }
    }
    
    private void FourCorners(Vector3 center, Vector3 size, Vector3 mainIndex)
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
            cornerPos.Add(cAngle + mainIndex,center + cAngle.normalized * mag);
        }
    }
    
    private BaseWall AddSquare(Vector3 center, Vector3 size, Vector3 newIndex)
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
        
        for (int i = 0; i < 4; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis(remain + 90 * i, Vector3.up) *pRight;
            testPos.Add(center + cAngle.normalized * mag);
        }
        cornerDict.Add(newIndex, bWall);
        return bWall;
    }

    private void AddSquare(Vector3 size, Vector3 newDir, Vector3 oldIndex, float extend)
    {
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        var start = cornerDict[oldIndex].center + newDir * extend;

        var bWall = new BaseWall()
        {
            center = start,
            size = size,
            firstVert = testPos.Count
        };
        
        var mag = Mathf.Sqrt(Mathf.Pow(size.y / 2, 2) + Mathf.Pow(size.x / 2, 2));
        var tan =Mathf.Atan(size.y / size.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(size.x / size.y) * (180 / Mathf.PI);
        
        for (int i = 0; i < 4; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis(remain + 90 * i, Vector3.up) *pRight;
            testPos.Add(start + cAngle.normalized * mag);
        }
        cornerDict.Add(oldIndex + newDir, bWall);
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
        if (Application.isPlaying) return;
        var pos = transform.position;
        BaseWall();
    }
}
