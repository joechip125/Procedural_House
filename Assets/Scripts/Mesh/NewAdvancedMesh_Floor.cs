using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class NewAdvancedMesh_Floor : NewAdvancedMesh
{
    [SerializeField, Range(0, 30)] private int numberX;
    [SerializeField, Range(0, 30)] private int numberZ;
    private List<Vector3> dots = new();
    
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
        Vertices.Clear();
        Triangles.Clear();
        
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

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.DrawSphere(pos, 3f);
        
       
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
        
        dots = dots.OrderBy(x => x.x).ThenBy(x => x.z).ToList();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(dots[0], dots[0] + new Vector3(0, 100,0));
     
        Gizmos.color = Color.red;
        Gizmos.DrawLine(dots[^1], dots[^1] + new Vector3(0, 100,0));
    }
}
