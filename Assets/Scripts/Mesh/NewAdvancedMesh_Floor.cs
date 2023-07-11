using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public enum Edges
{
    MinusX,
    PlusX,
    MinusZ,
    PlusZ
}

public enum TileType
{
    Square,
    Triangle,
    HalfCircle
}

[Serializable]
public class TileInfo
{
    public TileType type;
    public Vector2Int index;
    public Vector3 center;
    public Vector3 size;
}

public class NewAdvancedMesh_Floor : NewAdvancedMesh
{
    [SerializeField, Range(0, 30)] private int numberX;
    [SerializeField, Range(0, 30)] private int numberZ;
    [SerializeField, Range(1, 100)] private float tileSize;
    private List<Vector3> dots = new();
    [SerializeField] private Edges edgeChoice;
    
    private List<TileInfo> info = new();
    

    private List<Vector3> edgeList = new();
    private List<Vector3> circleList = new();

    public List<Vector3> doors = new();
    private Vector3 newStart;

    public int numberCircle;
    public int addCircle;

    [SerializeField] private float doorAdd;

    public Material aMaterial;

    public Action<Vector3, Vector3> Callback;

    private readonly Vector3[] corners = new[]
    {   new Vector3(-1, 0, -1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0) };

    private void Awake()
    {
        InitMesh();
    }

    public void SetValuesAndActivate(float size, int tilesX, int tilesZ)
    {
        ApplyMaterial(aMaterial);
        CircleFloor(transform.position, new Vector3(1,0,0));
        //MakeGrid(new Vector3(400,100,400), new Vector2Int(5,5));
    }
    
    public void SetValuesAndActivate(Vector3 size, int tilesX, int tilesZ)
    {
        
        numberX = tilesX;
        numberZ = tilesZ;
        Activate();
    }
    
    protected override void Activate()
    {
        base.Activate();
        
        MakeGrid(new Vector3(400,100,400), new Vector2Int(5,5));
        ApplyMaterial(aMaterial);
    }

    protected override void Register()
    {
        
    }

    private void MakeGrid(Vector3 totalSize, Vector2Int numberTiles)
    {
        var lineCount = Vertices.Count;

        var pos = -new Vector3(totalSize.x, 0, totalSize.z) / 2;
        var singleS = new Vector3(totalSize.x / (numberTiles.x - 1), totalSize.y, totalSize.z / (numberTiles.y - 1));
        
        info.Add(new TileInfo()
        {
            index = Vector2Int.zero,
            type = TileType.Square,
            center = pos + totalSize / 2, 
            size = totalSize
        });

        for (int i = 0; i < numberTiles.y; i++)
        {
            for (int j = 0; j < numberTiles.x; j++)
            {
                Vertices.Add(pos + new Vector3(singleS.x * j,0,0));
                
                if(j == numberTiles.x - 1 || i == numberTiles.y - 1) continue;
                
                Triangles.Add(lineCount + j);
                Triangles.Add(lineCount + j + numberTiles.x);
                Triangles.Add(lineCount + j + numberTiles.x + 1);
                
                Triangles.Add(lineCount + j);
                Triangles.Add(lineCount + j + numberTiles.x + 1);
                Triangles.Add(lineCount + j + 1);
            }

            lineCount += numberTiles.x;
            pos += new Vector3(0, 0, singleS.z);
        }
   
        UpdateMesh();
    }
    
    private void MakeGridTest(Vector3 totalSize, Vector2Int numberTiles)
    {
        var pos = -new Vector3(totalSize.x, 0, totalSize.z) / 2;
        var singleS = new Vector3(totalSize.x / (numberTiles.x - 1), totalSize.y, totalSize.z / (numberTiles.y - 1));
        var count = 0;

        for (int i = 0; i < numberTiles.y; i++)
        {
            for (int j = 0; j < numberTiles.x; j++)
            {
                var pos2 = pos + new Vector3(singleS.x * j, 0, 0);
                Gizmos.DrawSphere(pos2, 3);
                Handles.Label(pos2 + Vector3.up * 10, $"{count}");
                count++;
                
            }
            pos += new Vector3(0, 0, singleS.z);
        }
        UpdateMesh();
    }
    
    private void GetGridPos(Vector3 direction)
    {
        var all = Vertices
            .Where(x => x.x > 0)
            .OrderByDescending(x => x.x)
            .ThenByDescending(x => x.z)
            .ToList();
        Gizmos.color = Color.yellow;
        Debug.Log($"amount: {all.Count}");
        Gizmos.DrawSphere(all[0], 6);
    }

    private bool GetTilePos(Vector3 direction, Vector2Int index, out Vector3 pos)
    {
        var some = info.SingleOrDefault(x => x.index == index);
        pos = Vector3.zero;
        if (some == default) return false;

        var center = some.center;
        
        if (some.type == TileType.Square)
        {
            pos = center + Vector3.Scale(direction.normalized, some.size) / 2;
        }
        
        return true;
    }

    public void AddNewTile(TileType newType)
    {
        if (newType == TileType.HalfCircle)
        {
            
        }
    }
    
    private void AddSomething()
    {
        var v1 = newStart + new Vector3(0, 0, doorAdd);
        var v2 = v1 + new Vector3(0,0,50);
        var v3 = v1 + new Vector3(10,0,50);
        var v4 = v1 + new Vector3(10,0,0);
        
        AddQuad(v1, v2, v3, v4);
    }

    private void GetEdges(Vector3 direction)
    {
        edgeList.Clear();
        var numEdge = 0;

        if (direction.x != 0)
        {
            dots = direction.x > 0 ? 
                dots.OrderByDescending(x => x.x).ToList() 
                : dots.OrderBy(x => x.x).ToList();
        }
        
        if (direction.z != 0)
        {
            dots = direction.z > 0 ? 
                dots.OrderByDescending(x => x.z).ToList() 
                : dots.OrderBy(x => x.z).ToList();
        }
        
        newStart = dots[0];
    }
    
    private void Pyramid()
    {
        var start = 0;
        var amountX = 12;
        var amountZ = 12;
        var end = amountX;
        var pos = transform.position;

        for (int i = 0; i < amountZ; i++)
        {
            for (int j = start; j < end; j++)
            {
                Gizmos.DrawCube(pos + new Vector3(10 * j,0,0), Vector3.one * 10);
            }

            start++;
            end--;
            pos += new Vector3(0, 0, 10);
        }
    }

    private void CircleFloor(Vector3 pos, Vector3 dir)
    {
        var start = 0;
        var radius = 100;
        var adder = Vertices.Count  + 1;
        var vertexIndex = Vertices.Count;
        Vertices.Add(pos);

        for (int i = 0; i < numberCircle; i++)
        {
            var sin =Mathf.Cos((Mathf.PI / 180) * start);
            var cos = Mathf.Sin((Mathf.PI / 180) * start);
            var newPos = new Vector3(radius * sin, 0, radius * cos) + pos;
            
            Vertices.Add(newPos);
            
            start += addCircle;
            
            if(i > numberCircle - 2) continue;
            var current = adder + i;
            Triangles.Add(vertexIndex);
            Triangles.Add(current + 1);
            Triangles.Add(current);
        }
    }
    
    private void CircleTest(Vector3 pos, Vector3 dir, float numDeg, int resolution)
    {
        var radius = 100;
        
        Gizmos.DrawSphere(pos, 3);
        
        var aCrossForward2 = Vector3.Cross(dir,  Vector3.up).normalized;
        var singDeg = numDeg / resolution;

        for (int i = 0; i <= resolution; i++) 
        {
            var aCrossUp2 = Quaternion.AngleAxis(singDeg * i, Vector3.up) *aCrossForward2;
            var newPos = new Vector3(radius * aCrossUp2.x, 0, radius * aCrossUp2.z) + pos;
            Gizmos.DrawSphere(newPos, 3);
            Debug.Log($"degrees: {singDeg * i}");
        }
    }
    
    private void CircleWall(int startDegree, int radius)
    {
        var pos = transform.position;
        var adder = 0;
        
        for (int i = 0; i < numberCircle; i++)
        {
            var sin =Mathf.Cos((Mathf.PI / 180) * startDegree);
            var cos = Mathf.Sin((Mathf.PI / 180) * startDegree);
            var newPos = new Vector3(radius * sin, 0, radius * cos) + pos;
            
            Vertices.Add(newPos);
            Vertices.Add(newPos+ new Vector3(0, 50,0));
            
            startDegree += addCircle;
            
            if(i > numberCircle - 2) continue;
            Triangles.Add(adder);
            Triangles.Add(adder + 1);
            Triangles.Add(adder + 2);
            
            Triangles.Add(adder + 1);
            Triangles.Add(adder + 3);
            Triangles.Add(adder + 2);
            adder += 2;
        }
    }
    
    private void Circle(float radius)
    {
        var pos = transform.position;
        circleList.Clear();
        var aDir = new Vector3(1, 0, 0);
        var start = 0;

        for (int i = 0; i < numberCircle; i++)
        {
            var sin =Mathf.Cos((Mathf.PI / 180) * start);
            var cos = Mathf.Sin((Mathf.PI / 180) * start);

            var newPos = new Vector3(radius * sin, 0, radius * cos) + pos;
            circleList.Add(newPos);
            circleList.Add(newPos+ new Vector3(0, 50,0));

            start += addCircle;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;
        var pos = transform.position;
        //MakeGridTest(new Vector3(400,100,400), new Vector2Int(5,5));
        CircleTest(pos, Vector3.forward, 90, 10);
    }
}
