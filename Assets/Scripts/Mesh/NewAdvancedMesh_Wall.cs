using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public enum WallTypes
{
    Blank,
    Door,
    Window
}

[Serializable]
public class WallInfo
{
    public List<TileInfo> tileInfos = new();
    public Vector3 direction;
    public Vector3 start;
    public Vector2 size;

    public void ClearTiles()
    {
        tileInfos.Clear();
    }
}

[Serializable]
public class TileInfo
{
    public WallTypes type;
    public Vector2 pStartEnd;
    public Vector2 size;
}
public class NewAdvancedMesh_Wall : NewAdvancedMesh
{

    [SerializeField] private Vector3 direction;
    [SerializeField] private float adjacent;
    [SerializeField] private float height;
    [SerializeField] private int numberTiles;
    [SerializeField]private Vector3 wallSize;
    [SerializeField]private Vector3 wallNormal;
    
    [SerializeField]private float adjustDeg;
    
    [SerializeField, Range(1, 50)]private int resolution;
    
    public WallInfo wallInfo = new();

    private int lastVert;

    public Material aMaterial;
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        
        //AddDoor(Vector3.zero, new Vector3(100,200,30), Vector3.right);
        MakeWalls2();
    }
    
    private void AddDoor(Vector3 addPos, Vector3 size, Vector3 aDirection)
    {
        TunnelVerts(addPos, aDirection, size);
        var shift = aDirection * size.z;
        SideVerts(addPos +shift, aDirection, size);
        SideVerts(addPos, -aDirection, size);
    }

    private void SetWall()
    {
        
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
                Triangles.Add(1);
                
                Triangles.Add(1);
                Triangles.Add(0);
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
            size = new Vector2(size, 200),
            type = type
        });
    }
    private void FillInfos(float placePos, float size, float totalSize)
    {
        wallInfo.ClearTiles();
        wallInfo.size = new Vector2(totalSize, 200);
        var sideSize = (totalSize - size) / 2;
        AddInfo(sideSize, WallTypes.Blank);
        AddInfo(size, WallTypes.Door);
        AddInfo(sideSize, WallTypes.Blank);
    }
    
    private void TangentWall2(Vector3 aDir, float frwAmount, float wallLen)
    {
        var pos = transform.position;
        var start4 = -wallLen / 2;
        var rAmount = 1000f;
        FillInfos(-100, 200, wallLen);
        var addPos = 0f;
        var start = pos + aDir * frwAmount;
        var nextDir = Vector3.Cross(Vector3.up, aDir);
        var count = 0;
        
        foreach (var t in wallInfo.tileInfos)
        {
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
            Gizmos.DrawLine(pos, start);
            start += nextDir * t.size.x;
            Gizmos.DrawLine(pos, start);
            if (t.type == WallTypes.Blank)
            {
                Gizmos.DrawSphere(start + nextDir * t.pStartEnd.x, 7);
                Handles.Label(start + nextDir * t.pStartEnd.x, $"{count++}");
                Gizmos.DrawSphere(start + nextDir * t.pStartEnd.y, 7);
                Handles.Label(start + nextDir * t.pStartEnd.y, $"{count++}");
            }
        }
    }

    private void WallTracer()
    {
        
    }
    private void OnDrawGizmos()
    {
       // TangentWall2(direction, 500, 1000);
    }
}
