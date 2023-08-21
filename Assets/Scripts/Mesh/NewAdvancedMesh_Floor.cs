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
    public Vector3 center;
    public Vector3 size;
    public Vector2 numTiles;
    public Vector2 tileSize;
    public Vector3 normal;
    public int firstVert;
    public List<Vector3> corners = new();
    public List<int> vertIndices = new();
    public Dictionary<Vector3,int> VertDict = new();

    public void SetVertIndices()
    {
        
    }
}

public class NewAdvancedMesh_Floor : NewAdvancedMesh
{
    public Material aMaterial;
    private int lastVert;
    private Dictionary<Vector3, int> vertIndices = new ();
    
    private Dictionary<Vector3, DotInfo> dotInfos = new ();
    private List<Vector3> testPos = new();
    private List<SquareInfo> squares = new();

    private List<Vector3> sidePos = new();

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
        AddSquare(pos, new Vector3(1000,1000), Vector3.up, lastVert, new Vector2(5,5));
       
        var aSquare = squares[^1];
        
        var nextC = 0;
        var theCount = aSquare.corners.Count;
        for (int i = 0; i < theCount; i++)
        {
            nextC = i == theCount - 1 ? 0 : nextC + 1;
            Gizmos.color = Color.green;
            Gizmos.DrawLine(aSquare.corners[i], aSquare.corners[nextC]);
        }
    }

    private void SetSidePos(Vector2 side)
    {
        var square = squares[^1];
        sidePos.Clear();
        var temp = square.VertDict
            .Where(x =>
            {
                if (x.Key.x >= 5)
                {
                    
                }

                return false;
            });
    }
    
    private void AddSquare(Vector3 center, Vector3 size, Vector3 normal, int firstVert, Vector2 numTiles)
    {
        MathHelpers.PlaneDirections(normal, out var pUp, out var pRight);
        squares.Add(new SquareInfo()
        {
            normal = normal,
            center = center,
            size = size,
            firstVert = firstVert
        });
        
        var mag = Mathf.Sqrt(Mathf.Pow(size.y / 2, 2) + Mathf.Pow(size.x / 2, 2));
        var tan =Mathf.Atan(size.y / size.x) * (180 / Mathf.PI);
        var tan2 =Mathf.Atan(size.x / size.y) * (180 / Mathf.PI);
        
        for (int i = 0; i < 4; i++)
        {
            var remain = i % 2 == 0 ? tan : tan2;
            var cAngle = Quaternion.AngleAxis(remain + 90 * i, Vector3.up) *pRight;
            squares[^1].corners.Add(center + cAngle.normalized * mag);
        }

        var newIndex = Vector3.zero;
        var newPos = squares[^1].corners[1];
        var incX = size.x / numTiles.x;
        
        for (int i = 0; i <= numTiles.y; i++)
        {
            for (int j = 0; j <= numTiles.x; j++)
            {
                var current = newPos + pUp * (incX * j);
                PlaceDot(Color.red, current, firstVert);
                squares[^1].VertDict.Add(newIndex + Vector3.right * j, firstVert++);
                testPos.Add(current);
            }
            newIndex += Vector3.up;
            newPos += pRight * (size.y / numTiles.y);
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
        var aPos = transform.position;
        FloorTest(aPos);
    }
}
