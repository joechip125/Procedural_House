using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public enum Edges
{
    MinusX,
    PlusX,
    MinusZ,
    PlusZ
}

public class NewAdvancedMesh_Floor : NewAdvancedMesh
{
    [SerializeField, Range(0, 30)] private int numberX;
    [SerializeField, Range(0, 30)] private int numberZ;
    [SerializeField, Range(1, 100)] private float tileSize;
    private List<Vector3> dots = new();
    [SerializeField] private Edges edgeChoice;
    [SerializeField] private Vector3 totalSize;

    private List<Vector3> edgeList = new();
    private List<Vector3> circleList = new();
    private Vector3 newStart;

    public int numberCircle;
    public int addCircle;

    [SerializeField] private float doorAdd;

    public Material aMaterial;
    
    private readonly Vector3[] corners = new[]
    {   new Vector3(-1, 0, -1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0) };

    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        MakeGrid();
        AddOpen(new Vector3(1,0,0), 0, 40, 10);
    }

    public override void Activate()
    {
        base.Activate();
        
        MakeGrid();
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
    
    private void SimplePanel(Vector3 addPos, Vector3 normalDir, Vector2 theSize, int addDegree = 0)
    {
        Vector3 aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        var flip = false;

        if (normalDir.y != 0 && normalDir.x + normalDir.z == 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
            flip = true;
        }

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp = Quaternion.AngleAxis((90 * i) + addDegree , normalDir) *aCrossForward;
            var aCrossUp2 = Quaternion.AngleAxis((90 * i) + 90 + addDegree, normalDir) *aCrossForward;
            
            var poss = new Vector3();
            var poss2 = new Vector3();

            if (!flip)
            {
                poss = (aCrossUp * (theSize.x / 2)) + addPos;
                poss2 = aCrossUp2 * (theSize.y / 2) + addPos;
            }

            else
            {
                poss = aCrossUp * theSize.y / 2 + addPos;
                poss2 = aCrossUp2 * theSize.x / 2 + addPos;
            }

            corners[i] = poss + poss2;
            
            flip = !flip;
        }
        
        AddQuad(corners[0], corners[1], corners[2], corners[3]);
    }

    private void AddOpen(Vector3 primeDir, float position, float length, float width)
    {
        var superStart = new Vector3((totalSize.x * primeDir.x), 0, (totalSize.y * primeDir.z)) / 2 
                         + (primeDir * width) / 2;
        var crossDir = Vector3.Cross(primeDir, Vector3.up);
        var max = Vector3.Magnitude(Vector3.Scale(crossDir, totalSize)) / 2;
        position = Mathf.Clamp(position, -max + length / 2, max - length / 2);
        var aStart = superStart + crossDir * position;
        
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
        if (Application.isPlaying) return;
        
        var pos = transform.position -(new Vector3(numberX - 1, 0, numberZ - 1) * tileSize)/ 2;
        
        dots.Clear();
        
        for (int i = 0; i < numberZ; i++)
        {
            for (int j = 0; j < numberX; j++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(pos + new Vector3(100 * j,0,0), 3);
                dots.Add(pos + new Vector3(100 * j,0,0));
            }
            pos += new Vector3(0, 0, 100);
        }

        var moveDirection = new Vector3(1,0,0);
        GetEdges(moveDirection);
        var aColor = new Color(0, 0, 0);
        
        for (int i = 0; i < numberX; i++)
        {
            var edgePos3 = dots[i];
            Gizmos.color = aColor;
            Gizmos.DrawLine(edgePos3, edgePos3 + new Vector3(0, 50,0));
            aColor.r += 0.25f;
        }
    }
}
