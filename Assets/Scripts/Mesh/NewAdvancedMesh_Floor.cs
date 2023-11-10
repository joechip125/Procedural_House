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
    private List<int> vertSelect = new();
    private Vector3 panelR;
    private Vector3 panelU;

    private readonly Vector3[] corners = new[]
    {   new Vector3(-1, 0, -1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0) };
    
    public void SetValuesAndActivate(float sizeX, float sizeY)
    {
        InitMesh();
        ApplyMaterial(aMaterial);
    }

    private void SetValuesAndActivate()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
    }

    private void Awake()
    {
        var start = transform.position;
        var size = new Vector3(1000,1000);
        SetValuesAndActivate();
        lastVert = FloorVerts(start, size, Vector3.up, 0, new Vector2(5,5));
    }

    public void SetValuesAndActivate(Vector3 size, int tilesX, int tilesZ)
    {
        
        Activate();
    }
    
    protected override void Register()
    {
        
    }
    

    private int FloorVerts(Vector3 start, Vector3 size, Vector3 normal, int firstVert, Vector2 numTiles)
    {
        MathHelpers.PlaneDirections(normal, out var pUp, out var pRight);
        squares.Add(new SquareInfo()
        {
            normal = normal,
            center = start,
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
            squares[^1].corners.Add(start + cAngle.normalized * mag);
        }

        var newIndex = Vector3.zero;
        var newPos = squares[^1].corners[0];
        var incX = size.x / numTiles.x;
        
        for (int i = 0; i <= numTiles.y; i++)
        {
            for (int j = 0; j <= numTiles.x; j++)
            {
                var current = newPos + pUp * (incX * j);
                squares[^1].VertDict.Add(newIndex + Vector3.right * j, firstVert);
                Vertices.Add(current);

                if (i < numTiles.y && j < numTiles.x)
                {
                    Triangles.Add(firstVert);
                    Triangles.Add(firstVert + (int)numTiles.x + 1);
                    Triangles.Add(firstVert + (int)numTiles.x + 2);
                    
                    Triangles.Add(firstVert);
                    Triangles.Add(firstVert + (int)numTiles.x + 2);
                    Triangles.Add(firstVert + 1);
                }
                else
                {
                    
                }

                firstVert++;
            }
            newIndex += Vector3.up;
            newPos += -pRight * (size.y / numTiles.y);
        }
     
        UpdateMesh();
        return firstVert;
    }
    private void FloorTest(Vector3 pos)
    {
        lastVert = 0;
        vertIndices.Clear();
        testPos.Clear();
        dotInfos.Clear();
        squares.Clear();
        var numTiles = new Vector2(5, 5);
        var size = new Vector3(1000, 1000);
        lastVert = AddSquare(pos, size, Vector3.up, lastVert, numTiles);
        AddSquare2(Vector3.up, Vector3.right);
       
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
    
    
    private int AddSquare(Vector3 center, Vector3 size, Vector3 normal, int firstVert, Vector2 numTiles)
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
        var newPos = squares[^1].corners[0];
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
            newPos += -pRight * (size.y / numTiles.y);
        }

        return firstVert;
    }

    private void SetRotatedVector(Vector3 axis, Vector3 rVec, float rAngle)
    {
        panelR = Quaternion.AngleAxis(rAngle, axis) *rVec;
        panelU = Quaternion.AngleAxis(rAngle + 90, axis) *rVec;
    }
    
    private void AddSquare2(Vector3 normal, Vector3 exDir)
    {
        var maxX = squares[^1].VertDict.Keys.Max(x => x.x);
        var side = squares[^1].VertDict
            .Where(x => x.Key.x == maxX)
            .Select(x => x.Value)
            .ToList();

        var nEx = 200;
        
        foreach (var t in side)
        {
            var current = testPos[t] + exDir * nEx;
            for (int j = 0; j < 4; j++)
            {
                PlaceDot(Color.black, current, lastVert++);
                current += exDir * nEx;
            }
        }
        
        MathHelpers.PlaneDirections(normal, out var pUp, out var pRight);
        SetRotatedVector(normal, pRight, 90);
    }
    private void OnDrawGizmos()
    {
        var aPos = transform.position;
        FloorTest(aPos);
    }
}
