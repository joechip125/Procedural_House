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
    private Vector3 newStart;

    [SerializeField] private float doorAdd;

    public Material aMaterial;

    private void Awake()
    {
        InitMesh();
        MakeGrid();
        ApplyMaterial(aMaterial);
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

    private void GetEdges(Edges edge, Vector3 direction)
    {
        edgeList.Clear();
        var numEdge = 0;
        switch (edge)
        {
            case Edges.PlusX:
                dots = dots.OrderByDescending(x => x.x).ToList();
                break;
            case Edges.PlusZ:
                dots = dots.OrderByDescending(x => x.z).ToList();
                break;
            case Edges.MinusX:
                dots = dots.OrderBy(x => x.x).ToList();
                break;
            case Edges.MinusZ:
                dots = dots.OrderBy(x => x.z).ToList();
                break;
        }

        newStart = dots[0];
    }

    private void OnDrawGizmos()
    {
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

        var moveDirection = new Vector3(0,0,1);
        GetEdges(edgeChoice, moveDirection);
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
    }
}
