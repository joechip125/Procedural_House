using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;
using Vector4 = System.Numerics.Vector4;

[Serializable]
public class CornerInfo
{
    public Vector3 center;
    public Vector3 size;
    public List<BaseWall> wallSegments = new();
}

[Serializable]
public class BaseWall
{
    public List<int> cornerVerts = new();
}

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{
    [SerializeField] private Vector3 direction;
    [SerializeField] private float adjacent;
    [SerializeField] private int numberTiles;
    [SerializeField]private Vector3 wallSize;
    [SerializeField]private Vector3 wallNormal;
    private List<CornerInfo> corners = new();
    
    [SerializeField, Range(0, 180)]private float adjustDeg;
    
    private int lastVert;

    public Material aMaterial;

    private Dictionary<Vector3, int> vertIndices = new ();
    private List<Vector3> testPos = new();

    [Header("Direction")]
    [SerializeField, Range(-1,1)] private float xDir;
    [SerializeField, Range(-1,1)] private float yDir;
    [SerializeField, Range(-1,1)] private float zDir;
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        
    }
    
    public void InitWall(Vector3 normal, Vector2 size, int numTiles)
    {
        wallNormal = normal;
        wallSize = size;
    }

    protected override void Activate()
    {
        base.Activate();
        ApplyMaterial(aMaterial);
        
        var pos = transform.position;
        var single = adjacent / numberTiles;
       
        for (int i = 0; i < numberTiles; i++)
        {
            pos += direction.normalized * single;
        }
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
        var myDir = new Vector3(xDir, yDir, zDir);
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
        var anAngle = Vector3.Angle(corns[1], corns[2]);
        var anAngle2 = Vector3.Angle(corns[0], corns[1]);
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

    private bool GetPosAtIndex(Vector3 index, out Vector3 newPos)
    {
        newPos = Vector3.zero;
        if(!vertIndices.ContainsKey(index)) return false;

        newPos = testPos[vertIndices[index]];
        return true;
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
    
    private void GizmoSideVerts2(Vector3 pos, Vector3 normal, Vector2 innerSize, Vector2 outerSize)
    {
        var counter = 0;
        var vAmount = 8;
        var hAmount = 8;
        var lowest = 5;
        var vInc = outerSize.y / vAmount;
        var hInc = outerSize.x / hAmount;

        var iStart = new Vector2((outerSize.x - innerSize.x) /2, lowest);
        var iEnd = iStart + innerSize;
        var pPos = Vector2.zero;
        var pDot = true;

        PlaneDirections(normal, out var pUp, out var pRight);
        VizPlane(pos);
        for (int i = 0; i < vAmount; i++)
        {
            pPos.y = vInc * i;
            
            if (pos.y < iStart.y)
            {
                //if (pPos.y > iStart.y) pPos.y  = iStart.y;
            }
            else if (pos.y > iStart.y && pPos.y + vInc < iEnd.y)
            {
                //if (pPos.y > iEnd.y) pPos.y = iEnd.y;
            }
            pos = pUp * pPos.y;
            
            for (int j = 0; j < hAmount; j++)
            {
                pPos.x = hInc * j;
                var addVec = pRight * hInc;

                if (pPos.x < iStart.x)
                {
                    if (pPos.x + hInc >= iStart.x)
                    {
                       // addVec = pRight * iStart.x;
                    }    
                }
                else if (pPos.x < iEnd.x)
                {
                    if (pPos.x + hInc >= iEnd.x)
                    {
                      //  addVec = pRight * iEnd.x;
                    }    
                }


                if (pPos.x >= iStart.x && pPos.x < iEnd.x)
                {
                    if (pPos.y >= iStart.y && pPos.y < iEnd.y)
                    {
                        pDot = false;
                    }
                }
                
                else
                {
                    pDot = true;
                }

                if(pDot) PlaceDot(Color.red, pos, counter++);
                else  PlaceDot(Color.green, pos, counter++);
                pos += addVec;
            }
        }
    }
    
    private void GizmoSideVerts(Vector3 pos, Vector3 dir, Vector2 innerSize, Vector2 outerSize)
    {
        var vAmount = 6;
        var hAmount = 8;
        var lowest = 5;
        var vInc = outerSize.y / vAmount;
        var hInc = outerSize.x / hAmount;
        var counter = 0;
        var iStart = pos + (dir * (outerSize.x - innerSize.x) /2) + Vector3.up * lowest;
        var iEnd = iStart + dir * innerSize.x + Vector3.up * innerSize.y;
        var side = dir * (outerSize - innerSize).x / 2;
        var center = dir * innerSize.x;
        var nextX = Vector3.zero;
        var nextY = Vector3.zero;
        var firstSet = false;

        var start = pos;
        var tempCount = 0;
        var skipNext = false;
        
        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < vAmount; j++)
            {
                if (skipNext)
                {
                    counter = StackEm(pos, counter, new Vector2(iStart.y, iEnd.y), outerSize.y, vAmount);
                    skipNext = false;
                    break;
                }
                
                nextY = pos + Vector3.up * vInc;
                nextX = pos + dir * hInc;
                var nextAll = pos + (Vector3.up * vInc) + (dir * hInc);

                var currPlace = WhereIsPoint(iStart, iEnd, pos);
                var nextPlace = WhereIsPoint(iStart, iEnd, nextAll);
                Debug.Log($"current {currPlace}, next {nextPlace}");


                if (currPlace.x < 0 && currPlace.y < 0)
                {
                    if (nextPlace.x < 0)
                    {
                        
                    }
                    
                    if (currPlace.x < 0 && currPlace.y < 0)
                    {
                    
                    }    
                }
                
                var cMin = pos;
                var cMax = (nextX + nextY ) / 2;
                if (IsPointInSquare(cMin, cMax, iStart))
                {
                    skipNext = true;
                    continue;
                }

                if (IsPointInSquare(cMin, cMax, iStart + dir * innerSize.x))
                {
                    skipNext = true;
                    continue;
                }
                
                if (!IsPointInSquare(iStart, iEnd, pos))
                {
                    PlaceDot(Color.red, pos, counter++);
                    
                    if (IsPointInSquare(iStart, iEnd, nextX))
                    {
                        //PlaceDot(Color.green, new Vector3(iStart.x, pos.y, iStart.z), counter++);
                    }

                    if (nextY.y > iStart.y && nextY.y < iEnd.y && pos.y < 10)
                    {
                        PlaceDot(Color.green, new Vector3(pos.x, iStart.y + (Vector3.up * 10).y, pos.z), counter++);
                    }
                    
                    else if (nextY.y > iEnd.y && nextY.y < outerSize.y)
                    {
                       PlaceDot(Color.green, new Vector3(pos.x, iEnd.y + (Vector3.up * 10).y, pos.z), counter++);
                    }
                    
                }
                else
                {
                    if (nextY.y > iEnd.y && nextY.y < outerSize.y)
                    {
                        PlaceDot(Color.green, new Vector3(pos.x, iEnd.y + (Vector3.up * 10).y, pos.z), counter++);
                    }
                    
                    if (nextX.x > iEnd.x || nextX.z > iEnd.z)
                    {
                        //PlaceDot(Color.green, new Vector3(iEnd.x, pos.y, iEnd.z), counter++);
                    }
                }
                
                pos += Vector3.up * vInc;
            }

            start += dir * hInc;
            pos = start; 
        }
    }

    private int LineEm(Vector3 pos, int count, Vector3 dir, float length, int numDots)
    {
        var outVal = count;
        
        for (int i = 0; i < numDots; i++)
        {
            var next = pos + dir * (length / numDots);
            PlaceDot(Color.green, pos, outVal++);
            pos += dir * (length / numDots);
        }

        return outVal;
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
    
    private void SVerts2(Vector3 normalDir)
    {
        var aRight = Vector3.Cross(normalDir, Vector3.up).normalized;
        
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
    
    private void SetPoints2(Color pointColor,int num)
    {
        Gizmos.color = pointColor;
        
        for (int i = 0; i < Corners.Count; i++)
        {
            Gizmos.DrawSphere(Corners[i], 4);
            Handles.Label(Corners[i], $"{num++}");
            Gizmos.DrawSphere(Corners[i] + direction * wallSize.z, 4);
            Handles.Label(Corners[i]+ direction * wallSize.z, $"{num++}");
        }
    }

    private void TangentWall(Vector3 aDir)
    {
        var pos = transform.position;
        var arcTan = 100f / adjacent;
        var newVal = arcTan * adjacent;
        var addDegrees = adjustDeg;
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
        corners.Clear();
        testPos.Clear();
        lastVert = 0;
        var pos = transform.position;
        var size = new Vector3(20, 20);
        
        AddSquare(pos, size, true);
        AddSquare(pos + Vector3.left * 200, size);
        var cCount = corners[^1].wallSegments.Count;
        var nextI = 1;
        
        for (int i = 0; i < cCount; i++)
        {
            var current = corners[^1].wallSegments[i];

            for (int j = 0; j < current.cornerVerts.Count; j++)
            {
                nextI = j >= 3 ? 0 : nextI + 1;
                var cCorner = current.cornerVerts[j];
                var nCorner = current.cornerVerts[nextI];
                
            }
            
            nextI = 0;
        }
    }

    private void ExtendCorner(int cIndex, int vertI)
    {
        var startDir = Quaternion.AngleAxis(vertI * 90, Vector3.up) *Vector3.left;
        var verts = corners[cIndex].wallSegments[0].cornerVerts;
        var firstV = verts[vertI];
        var secondV = firstV >= 3 ? 0 : firstV + 1;
        var bWall = new BaseWall();
        var moveA = startDir * 200;
        bWall.cornerVerts = new List<int>
        {
            firstV, secondV, lastVert++, lastVert++
        };
        testPos.Add(testPos[firstV] + moveA);
        testPos.Add(testPos[secondV] + moveA);

        for (int i = 0; i < 4; i++)
        {
            var cV = bWall.cornerVerts[i];
            if(i > 1) PlaceDot(Color.red, testPos[cV], cV);
        }

        corners[cIndex].wallSegments.Add(bWall);
    }

    
    
    private void AddSquare(Vector3 center, Vector3 size, bool addNew = false)
    {
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        var bWall = new BaseWall();
        if(addNew)
        { 
            corners.Add(new CornerInfo()
            {
            center = center,
            size = size, 
            });
        }
        
        var mag = Mathf.Sqrt(Mathf.Pow(size.y / 2, 2) + Mathf.Pow(size.x / 2, 2));
        var tan =Mathf.Atan(size.y / size.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(size.x / size.y) * (180 / Mathf.PI);
        
        for (int i = 0; i < 4; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis(remain + 90 * i, Vector3.up) *pRight;
            testPos.Add(center + cAngle.normalized * mag);
            bWall.cornerVerts.Add(lastVert);
            PlaceDot(Color.red, center + cAngle.normalized * mag, lastVert++);
        }
        
        corners[^1].wallSegments.Add(bWall);
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        var pos = transform.position;
        BaseWall();
    }
}
