using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using Zenject.ReflectionBaking.Mono.Cecil;
using Quaternion = UnityEngine.Quaternion;
using Vector2 = UnityEngine.Vector2;
using Vector3 = UnityEngine.Vector3;

public enum WallTypes
{
    Blank,
    Door,
    Window
}

[System.Flags]
public enum PPlace
{
    None     = 0,
    LessX    = 1 << 0,
    LessY    = 1 << 1,
    MiddleX  = 1 << 2,
    MiddleY  = 1 << 3,
    GreaterX = 1 << 4,
    GreaterY = 1 << 5
}

[Serializable]
public class WallInfo
{
    public List<TileInfo> tileInfos = new();
    public Vector3 direction;
    public Vector3 start;
    public Vector3 size;

    public void AddTile(WallTypes type, Vector2 theSize)
    {
        tileInfos.Add(new TileInfo()
        {
            type = type,
            size = theSize
        });
    }
    
    public void ClearTiles()
    {
        tileInfos.Clear();
    }
}

[Serializable]
public class TileInfo
{
    public WallTypes type;
    public Vector2 size;
}
public class NewAdvancedMesh_Wall : NewAdvancedMesh
{

    [SerializeField] private Vector3 direction;
    [SerializeField] private float adjacent;
    [SerializeField] private int numberTiles;
    [SerializeField]private Vector3 wallSize;
    [SerializeField]private Vector3 wallNormal;
    
    [SerializeField]private float adjustDeg;
    
    [SerializeField, Range(1, 50)]private int resolution;
    
    public WallInfo wallInfo = new();

    private int lastVert;

    public Material aMaterial;
    private PPlace pPlace;
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        SetWall(direction, 500, new Vector3(1000,400,30));
        FillInfos(200);
        
        MakeWalls3();
    }
    
    private void AddDoor(Vector3 addPos, Vector3 size, Vector3 aDirection)
    {
        TunnelVerts(addPos, aDirection, size);
        var shift = aDirection * size.z;
        //SideVerts(addPos +shift, aDirection, size);
        //SideVerts(addPos, -aDirection, size);
    }
    
    private void AddDoor(Vector3 addPos,Vector3 aDirection, Vector3 innerSize, Vector3 outerSize, float adjustH = 0)
    {
        TunnelVerts(addPos + Vector3.up * adjustH, aDirection, innerSize);
        var shift = aDirection * innerSize.z;
        //SideVerts(addPos +shift, aDirection, outerSize);
        SideVerts(addPos+ Vector3.up * adjustH, -aDirection, innerSize);
    }

    private void SetWall(Vector3 startDir, float startAm, Vector3 totalWSize)
    {
        wallInfo.ClearTiles();
        wallInfo.direction = Vector3.Cross(Vector3.up, -startDir);
        wallInfo.size = totalWSize;
        wallInfo.start = startDir * startAm + -wallInfo.direction * totalWSize.x / 2;
    }

    private void SetTiles(int numTiles)
    {
        for (int i = 0; i < numTiles; i++)
        {
            wallInfo.AddTile(WallTypes.Blank, new Vector2(wallInfo.size.x / numTiles, wallInfo.size.y));
        }
    }
    
    
    public void AddDoor(int place)
    {
        BuildWall();
    }

    public void InitWall(Vector3 normal, Vector2 size, int numTiles)
    {
        wallNormal = normal;
        wallSize = size;
       
        BuildWall();
    }

    private void MakeWalls3()
    {
        var current = wallInfo.start;
        var aNormal = Vector3.Cross(Vector3.up, wallInfo.direction);
        var wallThick = 30;

        foreach (var t in wallInfo.tileInfos)
        {
            switch (t.type)
            {
                case WallTypes.Blank:
                    AWallTile(current, wallInfo.direction, t.size);
                    AWallTile(current + aNormal * wallInfo.size.z, wallInfo.direction, t.size, true);
                    break;
                case WallTypes.Door:
                    var outerSize = new Vector3(t.size.x, t.size.y, wallThick);
                    var doorSize = new Vector3(t.size.x, t.size.y, wallThick);
                    var innerSize = outerSize - new Vector3(50, 100, 0);
                    var adjust = (outerSize.y - innerSize.y) / 2 - 5;
                    AddDoor(current +(wallInfo.direction * t.size.x / 2) + Vector3.up * doorSize.y / 2, aNormal, innerSize, outerSize, -adjust);
                    break;
                case WallTypes.Window:
                    break;
            }
            
            current += wallInfo.direction * t.size.x;
        }
    }

    private void AWallTile(Vector3 start, Vector3 dir, Vector2 size, bool invert = false)
    {
        var vCount = Vertices.Count;
        Vertices.Add(start);
        Vertices.Add(start + Vector3.up * size.y);
        start += dir * size.x;
        Vertices.Add(start);
        Vertices.Add(start + Vector3.up * size.y);

        if (!invert)
        {
            Triangles.Add(vCount + 2);
            Triangles.Add(vCount + 1);
            Triangles.Add(vCount);
            
            Triangles.Add(vCount + 2);
            Triangles.Add(vCount + 3);
            Triangles.Add(vCount + 1);
        }

        else
        {
            Triangles.Add(vCount);
            Triangles.Add(vCount + 1);
            Triangles.Add(vCount + 3);
            
            Triangles.Add(vCount);
            Triangles.Add(vCount + 3);
            Triangles.Add(vCount + 2);
        }

        UpdateMesh();
    }
    
    private void MakeWalls()
    {
        var size = new Vector2(1000, 1000);
        SetSquare(Vector3.up, transform.position, size);
        var adjust = 90f;
        var point = 0;
        
        for (int i = 0; i < 4; i++)
        {
            var aNormal = Quaternion.AngleAxis(adjust + i * 90, Vector3.up) *Vector3.right;
            var wallRight = Vector3.Cross(Vector3.up, aNormal);
            
            var addVec = Vector3.zero;
            for (int j = 0; j < 4; j++)
            {
                Gizmos.DrawSphere(Corners[i] + addVec, 4);
                Handles.Label(Corners[i] + addVec, $"{point++}");
                
                Gizmos.DrawSphere(Corners[i] + addVec + Vector3.up * 100, 4);
                Handles.Label(Corners[i] + addVec+ Vector3.up * 100, $"{point++}");
                addVec += wallRight * size.x / 4;
            }
        }
    }
    private void MakeWalls2()
    {
        var size = new Vector2(1000, 1000);
        SetSquare(Vector3.up, transform.position, size);
        var adjust = 90f;

        for (int i = 0; i < 4; i++)
        {
            var aNormal = Quaternion.AngleAxis(adjust + i * 90, Vector3.up) *Vector3.right;
            var wallRight = Vector3.Cross(Vector3.up, aNormal);
            
            var addVec = Vector3.zero;
            for (int j = 0; j < 4; j++)
            {
                Vertices.Add(Corners[i] + addVec);
                Vertices.Add(Corners[i] + addVec+ Vector3.up * 100);
                addVec += wallRight * size.x / 4;
              
            }
        }

        var count = 0;
        for (int i = 0; i < 16; i++)
        {
            if (i == 15)
            {
                Triangles.Add(count);
                Triangles.Add(count + 1);
                Triangles.Add(1);  
                
                Triangles.Add(count);
                Triangles.Add(1);
                Triangles.Add(0);  
                continue;
            }
            
            Triangles.Add(count);
            Triangles.Add(count + 1);
            Triangles.Add(count + 3);
            
            Triangles.Add(count);
            Triangles.Add(count + 3);
            Triangles.Add(count + 2);
            count += 2;
        }
        UpdateMesh();
    }
    
    private void BuildWall()
    {
        ClearMesh();
        var wallRight = Vector3.Cross(Vector3.up, wallNormal);
        var numTiles = wallInfo.tileInfos.Count();

        var singlePanel = new Vector2(wallSize.x / numTiles, wallSize.y);
        var start = wallRight * singlePanel.x / 2 + (Vector3.up * wallSize.y) / 2;
        
        var panelSize2 = new Vector3(singlePanel.x, singlePanel.y, 10);
      
        for (int i = 0; i < numTiles; i++)
        {
            start += wallRight * singlePanel.x;
        }
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

    private void NewSideVerts(Vector3 pos, Vector3 dir, Vector2 innerSize, Vector2 outerSize)
    {
        var vAmount = 8;
        var vInc = outerSize.y / vAmount;
        for (int i = 0; i < vAmount; i++)
        {
            Vertices.Add(pos);
            pos += Vector3.up * vInc;
        }
    }

    private void DoorFront(Vector3 pos, Vector3 dir, Vector2 innerSize, Vector2 outerSize)
    {
        var vAmount = 6;
        var hAmount = 9;
        var lowest = 5;
        var vInc = outerSize.y / vAmount;
        var hInc = outerSize.x / hAmount;
        var counter = 0;
        var iStart = pos + (dir * (outerSize.x - innerSize.x) /2) + Vector3.up * lowest;
        var iEnd = iStart + dir * innerSize.x + Vector3.up * innerSize.y;
       
        var nextX = Vector3.zero;
        var nextY = Vector3.zero;
        var firstSet = false;

        var start = pos;
        var tempCount = 0;
        var skipNext = false;
        var vertC = Vertices.Count;
        

        for (int i = 0; i < hAmount; i++)
        {
            var cMin = pos;
            var cMax = cMin + dir * hInc + Vector3.up * vInc;
            if (skipNext)
            {
                skipNext = false;
                break;
            }
            if (IsPointInSquare(cMin, cMax, iStart))
            {
                skipNext = true;
                counter = StackEm(pos, counter, new Vector2(iStart.y, iEnd.y), outerSize.y, vAmount);
                continue;
            }
            for (int j = 0; j < vAmount; j++)
            {
                nextY = pos + Vector3.up * vInc;
                nextX = pos + dir * hInc;
                cMin = pos;
                cMax = (nextX + nextY ) / 2;
                Vector3 nextAll = pos + Vector3.up * vInc + dir * hInc;

                var currPlace = WhereIsPoint(iStart, iEnd, pos);
                var nextPlace = WhereIsPoint(iStart, iEnd, nextAll);
                
                Debug.Log($"v {j}, h {i} pos: {pos} next: {nextAll}  current: {currPlace.ToString()}");
                if (currPlace.HasFlag(PPlace.LessY))
                {
                    if (nextPlace.HasFlag(PPlace.MiddleY))
                    {
                        
                    }

                    if (nextPlace.HasFlag(PPlace.LessX))
                    {
                        PlaceDot(Color.magenta, pos + Vector3.forward *30, 0);
                    }
                }
                
                pos += Vector3.up * vInc;
            }

            start += dir * hInc;
            pos = start; 
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
                    skipNext = false;
                    break;
                }
                
                nextY = pos + Vector3.up * vInc;
                nextX = pos + dir * hInc;
                var nextAll = pos + Vector3.up * vInc + dir * hInc;

                var currPlace = WhereIsPoint(iStart, iEnd, pos);
                var nextPlace = WhereIsPoint(iStart, iEnd, nextAll);
                
                

                Debug.Log($"v {j}, h {i} pos: {pos} next: {nextAll}  current: {currPlace.ToString()}");
                if ((currPlace & (PPlace.LessY | PPlace.LessX)) == (PPlace.LessY | PPlace.LessX))
                {
                    
                }
                if (currPlace.HasFlag(PPlace.LessY))
                {
                    if (nextPlace.HasFlag(PPlace.MiddleY))
                    {
                        
                    }

                    if (nextPlace.HasFlag(PPlace.LessX))
                    {
                        //PlaceDot(Color.magenta, pos + Vector3.forward *30, 0);
                    }
                }
                else if (currPlace.HasFlag(PPlace.MiddleY))
                {
                    PlaceDot(Color.magenta, pos + Vector3.forward *30, 0);
                }
                else if (currPlace.HasFlag(PPlace.GreaterY))
                {
                    
                }
                
                var cMin = pos;
                var cMax = (nextX + nextY ) / 2;
                if (IsPointInSquare(cMin, cMax, iStart))
                {
                    skipNext = true;
                    counter = StackEm(pos, counter, new Vector2(iStart.y, iEnd.y), outerSize.y, vAmount);
                    continue;
                }

                if (IsPointInSquare(cMin, cMax, iStart + dir * innerSize.x))
                {
                    skipNext = true;
                    counter = StackEm(pos, counter, new Vector2(iStart.y, iEnd.y), outerSize.y, vAmount);
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

    private PPlace WhereIsPoint(Vector3 min, Vector3 max, Vector3 point)
    {
        var outP = PPlace.None;

        if (point.y < min.y) outP |= PPlace.LessY;
        
        else if (point.y >= min.y)
        { 
            if (point.y < max.y) outP |= PPlace.MiddleY;
            else if (point.y >= max.y) outP |= PPlace.GreaterY;
        }
        
        if (point.x < min.x || point.z < min.z) outP |= PPlace.LessX;
        
        else if (point.x >= min.x && point.z >= min.z)
        {
            if (point.x < max.x && point.z < max.z) outP |= PPlace.MiddleX;
            else if (point.x >= max.x && point.z >= max.z) outP |= PPlace.GreaterX;
        }
        return outP;
    }
    
    private void SVerts2(Vector3 normalDir)
    {
        var aRight = Vector3.Cross(normalDir, Vector3.up).normalized;

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


    private void AddInfo(float size, WallTypes type)
    {
        wallInfo.tileInfos.Add(new TileInfo()
        {
            size = new Vector2(size, wallInfo.size.y),
            type = type
        });
    }
    private void FillInfos(float doorSize)
    {
        wallInfo.ClearTiles();
        var sideSize = (wallInfo.size.x - doorSize) / 2;
        AddInfo(sideSize, WallTypes.Blank);
        AddInfo(doorSize, WallTypes.Door);
        AddInfo(sideSize, WallTypes.Blank);
    }
    
    private void TangentWall2(Vector3 aDir, float frwAmount, float wallLen)
    {
        var pos = transform.position;
        FillInfos(200);
        var next = wallInfo.start;
        var nextDir = wallInfo.direction;
        var count = 0;
        
        foreach (var t in wallInfo.tileInfos)
        {
            var start = next;
            next += nextDir * t.size.x;
            
            switch (t.type)
            {
                case WallTypes.Blank:
                    Gizmos.color = Color.white;
                    break;
                case WallTypes.Door:
                    Gizmos.color = Color.green;
                    break;
                case WallTypes.Window:
                    Gizmos.color = Color.yellow;
                    break;
            }
            //Gizmos.DrawLine(pos, start);
            //Gizmos.DrawLine(pos, next);
            if (t.type == WallTypes.Blank)
            {
                Gizmos.DrawSphere(start, 7);
                Handles.Label(start, $"{count++}");
                
                Gizmos.DrawSphere(start + Vector3.up * wallInfo.size.y, 7);
                Handles.Label(start+ Vector3.up * wallInfo.size.y, $"{count++}");
                
                Gizmos.DrawSphere(next, 7);
                Handles.Label(next, $"{count++}");
                
                Gizmos.DrawSphere(next+ Vector3.up * wallInfo.size.y, 7);
                Handles.Label(next+ Vector3.up * wallInfo.size.y, $"{count++}");
            }

            if (t.type == WallTypes.Door)
            {
                Gizmos.DrawLine(start, next);
            }
        }
    }

    private void WallTracer()
    {
        
    }
    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        GizmoSideVerts(Vector3.zero, direction, new Vector2(100,200), new Vector2(200, 300));
        //SetWall(direction, 500, new Vector2(1000, 300));
        //TangentWall2(direction, 500, 1000);
    }
}
