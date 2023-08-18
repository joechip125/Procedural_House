using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.ExceptionServices;
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
public class DotInfo
{
    public int vertIndex;
    public Vector3 vertPos;
}

[Serializable]
public class SquareInfo
{
    public Vector3 pos;
    public Vector3 size;
    public Vector2 numDots;
    public Vector2 tileSize;
}

public class NewAdvancedMesh_Floor : NewAdvancedMesh
{
    [SerializeField, Range(0, 30)] private int circleResolution;
    [SerializeField, Range(0, 30)] private int numberZ;
    [SerializeField, Range(1, 100)] private float tileSize;
    private List<Vector3> dots = new();
    [SerializeField] private Edges edgeChoice;
    
    private List<TileInfo> info = new();
    
    private List<Vector3> edgeList = new();

    public int numberCircle;
    public int addCircle;

    public Material aMaterial;
    
    private int lastVert;
    private Dictionary<Vector3, int> vertIndices = new ();
    
    private Dictionary<Vector3, DotInfo> dotInfos = new ();
    private List<Vector3> testPos = new();
    private List<SquareInfo> squares = new();

    private readonly Vector3[] corners = new[]
    {   new Vector3(-1, 0, -1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0) };
    
    public void SetValuesAndActivate(float sizeX, float sizeY)
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        FloorVerts();
    }


    
    public void SetValuesAndActivate(Vector3 size, int tilesX, int tilesZ)
    {
        
        Activate();
    }
    
    protected override void Register()
    {
        
    }

    private void CylinderTest()
    {
        var pos = transform.position;
        CircleFloor(pos, Vector3.forward, 360, circleResolution);
        Sides(1, Vertices.Count);
    }

    private void Sides(int first, int last)
    {
        var depth = 20;
        var add = 0;
        
        for (int i = first; i < last; i++)
        {
            Vertices.Add(Vertices[i] + Vector3.down * depth);

            if (i < last - 1)
            {
                Triangles.Add(i + 1);
                Triangles.Add(i);
                Triangles.Add(last + add);
                
                Triangles.Add(last + add);
                Triangles.Add(last + add + 1);
                Triangles.Add(i + 1);
            }
            
            add++;
        }
        UpdateMesh();
    }
    
    private void MakeGrid(Vector3 totalSize, Vector2Int numberTiles)
    {
        var lineCount = Vertices.Count;

        var pos = -new Vector3(totalSize.x, 0, totalSize.z) / 2;
        var singleS = new Vector3(totalSize.x / (numberTiles.x - 1), totalSize.y, totalSize.z / (numberTiles.y - 1));

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
    }

    private void CircleFloor(Vector3 pos, Vector3 dir, float numDeg, int resolution, float start = 0)
    {
        var radius = 100;
        
        Vertices.Add(pos);
        var adder = Vertices.Count;
        var vertIndex = Vertices.Count - 1;
        var size = new Vector2(200, 200);
        
        var aCrossForward2 = Vector3.Cross(dir,  Vector3.up).normalized;
        var singDeg = numDeg / resolution;

        for (int i = 0; i <= resolution; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(start + singDeg * i, Vector3.up) *aCrossForward2;
            var newPos = new Vector3(size.x * aCrossUp2.x, 0, size.y * aCrossUp2.z) + pos;

            Vertices.Add(newPos);

            if(i > resolution - 1) continue;
            Triangles.Add(vertIndex);
            Triangles.Add(adder + i);
            Triangles.Add(adder + i + 1);
        }
        UpdateMesh();
    }
    
    private void CircleTest(Vector3 pos, Vector3 dir, float numDeg, int resolution)
    {
        Gizmos.DrawSphere(pos, 3);
        var size = new Vector2(200, 200);

        var aCrossForward2 = Vector3.Cross(dir,  Vector3.up).normalized;
        var singDeg = numDeg / resolution;

        for (int i = 0; i <= resolution; i++)
        {
            var cos = (Mathf.Cos((Mathf.PI / 180) * (singDeg * i)) * dir) * size.y;
            var cos2 = (Mathf.Cos((Mathf.PI / 180) * (singDeg * i)) * aCrossForward2) * size.x;
            
            var aCrossUp2 = Quaternion.AngleAxis(singDeg * i, Vector3.up) *aCrossForward2;
            var aCrossUp3 = Quaternion.AngleAxis(singDeg * i, Vector3.up) * dir;
            var x = aCrossUp2 * size.x;
            var y = aCrossUp2 * size.y;
            var newPos = new Vector3(size.x * aCrossUp2.x, 0, size.y * aCrossUp2.z) + pos;
            var newPos2 = pos + cos2;
            Gizmos.DrawSphere(newPos, 3);
            Handles.Label(newPos + Vector3.up * 10, $"{i}");
        }
    }
    private void PlaceDot(Color color, Vector3 pos, int count)
    {
        Gizmos.color = color;
        Handles.Label(pos,$"{count}");
        Gizmos.DrawSphere(pos, 3);
    }

    private void FloorVerts()
    {
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        lastVert = Vertices.Count;
    }
    
    private void FloorVerts(Vector3 min, Vector3 max)
    {
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        lastVert = Vertices.Count;
    }
    
    private void FloorTest(Vector3 pos)
    {
        MathHelpers.PlaneDirections(Vector3.up, out var pUp, out var pRight);
        lastVert = 0;
        vertIndices.Clear();
        testPos.Clear();
        dotInfos.Clear();
        squares.Clear();
        var square = new Vector3(6, 6);
        AddSquare(Vector3.zero, square);
        AddSquare(new Vector3(square.x,0), new Vector3(3,3));
        
        var pSize = new Vector2(1000, 1000);
        var corns = new Vector3[4];
        var counter = 0;
        var rez = 4;
        var numTiles = new Vector2(5, 5);
        var xInc = pSize.x / numTiles.x;
        var yInc = pSize.y / numTiles.y;
        
        var test = Mathf.Sqrt(Mathf.Pow(pSize.y / 2, 2) + Mathf.Pow(pSize.x / 2, 2));
        var tan =Mathf.Atan(pSize.y / pSize.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(pSize.x / pSize.y) * (180 / Mathf.PI); 
        
        for (int i = 0; i < corns.Length; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis(remain + 90 * i, Vector3.up) *pRight;
            corns[i] = pos + cAngle.normalized * test;
        }
        
        var aNewPos = corns[1];
        for (int i = 0; i < dotInfos.Count(x => x.Key.x == 0); i++)
        {
            for (int j = 0; j < dotInfos.Count(x => x.Key.y == i); j++)
            {
                var index3 = new Vector3(j,i);
                dotInfos[index3].vertIndex = lastVert++;
                dotInfos[index3].vertPos = aNewPos + pRight * (xInc * j);
            }
            aNewPos += pUp * yInc;
        }
        
        foreach (var d in dotInfos)
        {
            PlaceDot(Color.red, d.Value.vertPos, d.Value.vertIndex);
        }
        
        var nextC = 0;
        for (int i = 0; i < corns.Length; i++)
        {
            nextC = i == corns.Length - 1 ? 0 : nextC + 1;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(corns[i], corns[nextC]);
        }
    }

    private void AddSquare(Vector3 minIndex, Vector3 size)
    {
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                dotInfos.Add(minIndex + Vector3.right * j, new DotInfo());
            }
            minIndex += Vector3.up;
        }
    }
    
    private void AddSquare(Vector3 pos, Vector3 size, Vector2 normal)
    {
        MathHelpers.PlaneDirections(normal, out var pUp, out var pRight);
        
        squares.Add(new SquareInfo()
        {
            
        });
        for (int i = 0; i < size.y; i++)
        {
            for (int j = 0; j < size.x; j++)
            {
                dotInfos.Add(pos + Vector3.right * j, new DotInfo());
            }
            pos += Vector3.up;
        }
    }

    private void ExtendFloor(Vector3 extendDir)
    {
        var edge = vertIndices
            .Where(x => x.Key.x >= 5)
            .Select(x => x.Value)
            .ToList();
        var pos = transform.position;
        var add = 200;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 4; j++)
            {
                PlaceDot(Color.blue, testPos[edge[i]] + extendDir * add, lastVert++);
                add += 200;
            }

            add = 200;
        }
    }
    
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        if (circleResolution <= 0) return;
        var aPos = transform.position;
        FloorTest(aPos);
        
    }
}
