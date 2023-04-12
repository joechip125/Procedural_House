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
    private List<Vector3> dots = new();
    [SerializeField] private Edges edgeChoice;

    private List<Vector3> edgeList = new();
    private List<Vector3> circleList = new();
    private Vector3 newStart;

    [SerializeField] private float doorAdd;

    public Material aMaterial;

    private void Awake()
    {
        InitMesh();
        MakeGrid();
        ApplyMaterial(aMaterial);
        GetEdges2(new Vector3(1,0,0));
        AddSomething();
    }

    private void MakeGrid()
    {
        var lineCount = Vertices.Count;
        
        var pos = transform.position;

        for (int i = 0; i < numberZ; i++)
        {
            for (int j = 0; j < numberX; j++)
            {
                Vertices.Add(pos + new Vector3(100 * j,0,0));
                
                if(j == numberX - 1 || i == numberZ - 1) continue;
                
                Triangles.Add(lineCount + j);
                Triangles.Add(lineCount + j + numberX);
                Triangles.Add(lineCount + j + numberX + 1);
                
                Triangles.Add(lineCount + j);
                Triangles.Add(lineCount + j + numberX + 1);
                Triangles.Add(lineCount + j + 1);
            }

            lineCount += numberX;
            pos += new Vector3(0, 0, 100);
        }
   
        UpdateMesh();
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
                Debug.Log($"index i{i} start {start} end {end}");
                Gizmos.DrawCube(pos + new Vector3(10 * j,0,0), Vector3.one * 10);
            }

            start++;
            end--;
            pos += new Vector3(0, 0, 10);
        }
    }

    private void Circle()
    {
        var pos = transform.position;
        circleList.Clear();

        for (int i = 0; i < 6; i++)
        {
            
        }
        
    }
    
    private void GetEdges2(Vector3 direction)
    {
        edgeList.Clear();

        var right = Vector3.Cross(direction, Vector3.up);

        if (direction.x != 0)
        {
            if(direction.x < 0)
                dots = Vertices.OrderBy(x => x.x).ToList();
            else if(direction.x > 0)
                dots = Vertices.OrderByDescending(x => x.x).ToList();
        }
        
        if (direction.z != 0)
        {
            if(direction.z < 0)
                dots = Vertices.OrderBy(x => x.z).ToList();
            else if(direction.z > 0)
                dots = Vertices.OrderByDescending(x => x.z).ToList();
        }
        
        newStart = dots[0];
    }

    private void OnDrawGizmos()
    {
        if (Application.isPlaying) return;
        
        var pos = transform.position;
        Gizmos.DrawSphere(pos, 3f);
       // Debug.Log($"length {Vector3.Magnitude(new Vector3(50, 0, 50))}");
       
        dots.Clear();
    
        var total = numberX * numberZ;
        
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
        
        Gizmos.color = Color.magenta;
        var aStart = newStart + moveDirection * doorAdd;
        Gizmos.DrawSphere(aStart, 5);
        Gizmos.DrawSphere(aStart + moveDirection * 50, 5);
        
        for (int i = 0; i < numberX; i++)
        {
            var edgePos3 = dots[i];
            Gizmos.color = aColor;
            Gizmos.DrawLine(edgePos3, edgePos3 + new Vector3(0, 50,0));
            aColor.r += 0.25f;
        }
        
        Circle();

        foreach (var c in circleList)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(c, 3);
        }
    }
}
