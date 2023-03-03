using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Matrix4x4 = System.Numerics.Matrix4x4;

public enum Directions
{
    Up,
    Down
}
[ExecuteInEditMode]
public class NewMeshTest : MonoBehaviour
{
    private AdvancedMesh mesh;
    public Material aMaterial;
    public List<Vector3> points = new ();
    private Matrix4x4 aMatrix;

    private Vector3[] arrayPoints;
    private Vector3[] vertices;

    private void Awake()
    {
        if (!Application.isEditor) return;
        arrayPoints = new Vector3[4];
        mesh = gameObject.GetComponent<AdvancedMesh>() == null ? 
            gameObject.AddComponent<AdvancedMesh>() : gameObject.GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        mesh.ApplyMaterial(aMaterial);
        AddSomePoints2();
    }
    
    private void SetPoints(Vector3 direction, Vector3 size, Vector3 startPoint, bool backFront = false)
    {
        points.Clear();

        if(backFront) points.Add(startPoint);
        else points.Add(startPoint + Vector3.Scale(direction, size));

        if (direction.z != 0)
        {
            points.Add(startPoint + new Vector3(0, 0, size.z * direction.z));
        }
        
        if (direction.x != 0)
        {
            points.Add(startPoint + new Vector3(size.x * direction.x, 0,0));
        }

        if (direction.y != 0)
        {
            points.Add(startPoint + new Vector3(0, size.y,0));
        }

        if(!backFront) points.Add(startPoint);
        else points.Add(startPoint + Vector3.Scale(direction, size));
    }

    private void SetFloorCeil(Vector2 size, Vector3 startPoint)
    {
        //points.Add(startPoint);
        //points.Add(startPoint + new Vector3(0, 0, size.x));
        //points.Add(startPoint + new Vector3(size.x,0,0));
        //points.Add(startPoint + new Vector3(size.x, 0, size.y));

        points.Add(startPoint + new Vector3(size.x, 0, size.y));
        points.Add(startPoint + new Vector3(0, 0, size.x));
        points.Add(startPoint + new Vector3(size.x,0,0));
        points.Add(startPoint);
    }
    
    private void AddPoints()
    {
        points.Clear();
    }

    private void AddSomePoints2()
    {
        var pos = transform.localPosition;
        
        SetPoints(new Vector3(0,1,1), new Vector3(100,100, 50), pos, true);
        mesh.AddQuadWithPointList(points);
        
        SetPoints(new Vector3(0,1,1), new Vector3(100,100, 50), pos + new Vector3(-10,0,0));
        mesh.AddQuadWithPointList(points);
        
        SetPoints(new Vector3(1,1,0), new Vector3(10,80, 50), pos + new Vector3(-10,0,50));
        mesh.AddQuadWithPointList(points);
        
        SetPoints(new Vector3(1,0,1), new Vector3(10,10, 30), pos + new Vector3(-10,80,50), true);
        mesh.AddQuadWithPointList(points);
        
        SetPoints(new Vector3(1,0,1), new Vector3(10,10, 30), pos + new Vector3(-10,0,50), false);
        mesh.AddQuadWithPointList(points);
        
        SetPoints(new Vector3(0,1,1), new Vector3(10,20, 30), pos + new Vector3(-10,80,50), false);
        mesh.AddQuadWithPointList(points);
        
        SetPoints(new Vector3(0,1,1), new Vector3(10,20, 30), pos + new Vector3(0,80,50), true);
        mesh.AddQuadWithPointList(points);
    }
    
    
    private void OnDrawGizmos () 
    {
        if (vertices == null) return;
        
        Gizmos.color = Color.black;
        
        for (int i = 0; i < vertices.Length; i++) 
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(vertices[i], 0.1f);
            if (i > 0)
            {
                Gizmos.color = Color.red;
                var prev = vertices[i - 1];
                Gizmos.DrawLine(prev, vertices[i]);
            }
        }
    }
}
