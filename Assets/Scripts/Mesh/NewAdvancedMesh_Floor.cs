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
}

public class NewAdvancedMesh_Floor : NewAdvancedMesh
{
    [SerializeField, Range(0, 30)] private int numberX;
    [SerializeField, Range(0, 30)] private int numberZ;
    [SerializeField, Range(1, 100)] private float tileSize;
    private List<Vector3> dots = new();
    [SerializeField] private Edges edgeChoice;
    [SerializeField] private Vector3 totalSize;

    public Vector3 TotalSize => totalSize;

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
        tileSize = size;
        numberX = tilesX;
        numberZ = tilesZ;
        Activate();
    }
    
    public void SetValuesAndActivate(Vector3 size, int tilesX, int tilesZ)
    {
        totalSize = size;
        numberX = tilesX;
        numberZ = tilesZ;
        Activate();
    }
    
    protected override void Activate()
    {
        base.Activate();
        
        MakeGrid();
        ApplyMaterial(aMaterial);
    }

    protected override void Register()
    {
        
    }

    private void MakeGrid()
    {
        var lineCount = Vertices.Count;
        
        var pos = -(new Vector3(numberX - 1, 0, numberZ - 1) * tileSize)/ 2;
        totalSize = new Vector3(numberX - 1,0, numberZ - 1) * tileSize;

        for (int i = 0; i < numberZ; i++)
        {
            for (int j = 0; j < numberX; j++)
            {
                Vertices.Add(pos + new Vector3(tileSize * j,0,0));
                
                if(j == numberX - 1 || i == numberZ - 1) continue;
                
                Triangles.Add(lineCount + j);
                Triangles.Add(lineCount + j + numberX);
                Triangles.Add(lineCount + j + numberX + 1);
                
                Triangles.Add(lineCount + j);
                Triangles.Add(lineCount + j + numberX + 1);
                Triangles.Add(lineCount + j + 1);
            }

            lineCount += numberX;
            pos += new Vector3(0, 0, tileSize);
        }
   
        UpdateMesh();
    }
    
    
    private void MakeGridTest()
    {
        var pos = -(new Vector3(numberX - 1, 0, numberZ - 1) * tileSize)/ 2;
        totalSize = new Vector3(numberX - 1,0, numberZ - 1) * tileSize;
        var count = 0;

        for (int i = 0; i < numberZ; i++)
        {
            for (int j = 0; j < numberX; j++)
            {
                var pos2 = pos + new Vector3(tileSize * j, 0, 0);
                Gizmos.DrawSphere(pos2, 3);
                Handles.Label(pos2 + Vector3.up * 10, $"{count}");
                count++;
            }
            
            pos += new Vector3(0, 0, tileSize);
        }
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

    private void GetTilePos(Vector3 direction)
    {
        
    }
    
    private void AddOpen(Vector3 primeDir, float position, float length, float width)
    {
        var superStart = new Vector3((totalSize.x * primeDir.x), 0, (totalSize.y * primeDir.z)) / 4 
                         + (primeDir * width) / 4;
        var crossDir = Vector3.Cross(primeDir, Vector3.up);
        var max = Vector3.Magnitude(Vector3.Scale(crossDir, totalSize)) / 2;
        position = Mathf.Clamp(position, -max + length / 2, max - length / 2);
        var aStart = superStart + crossDir * position;
        doors.Add(aStart);
        Callback?.Invoke(aStart * 2, primeDir);
        
      
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

    private void CircleWall()
    {
        var pos = transform.position;
        var start = 0;
        var radius = 100;
        var adder = 0;
        
        for (int i = 0; i < numberCircle; i++)
        {
            var sin =Mathf.Cos((Mathf.PI / 180) * start);
            var cos = Mathf.Sin((Mathf.PI / 180) * start);
            var newPos = new Vector3(radius * sin, 0, radius * cos) + pos;
            
            Vertices.Add(newPos);
            Vertices.Add(newPos+ new Vector3(0, 50,0));
            
            start += addCircle;
            
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
        MakeGridTest();
        GetGridPos(Vector3.forward);
    }
}
