using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Directions
{
    Up,
    Down
}

public class NewMeshTest : MonoBehaviour
{
    private AdvancedMesh mesh;
    public Material aMaterial;
    public List<Vector3> points = new ();

    private Vector3[] arrayPoints;

    private void Awake()
    {
        arrayPoints = new Vector3[4];
        mesh = gameObject.AddComponent<AdvancedMesh>();
        mesh.ApplyMaterial(aMaterial);
        AddSomePoints();
        AddSomePoints2();
    }
    
    private void SetPoints(Directions direction, Vector2 size, Vector3 startPoint)
    {
        points.Clear();

        if (direction == Directions.Up)
        {
            arrayPoints[0] = startPoint;
            arrayPoints[1] = startPoint + new Vector3(0, 0, size.x);
            arrayPoints[2] = startPoint + new Vector3(0, size.y,0);
            arrayPoints[3] = startPoint + new Vector3(0, size.y, size.x);
            points.Add(startPoint);
            points.Add(startPoint + new Vector3(0, 0, size.x));
            points.Add(startPoint + new Vector3(0, size.y,0));
            points.Add(startPoint + new Vector3(0, size.y, size.x));
        }
        else
        {
            points.Add(startPoint + new Vector3(size.x, 0, size.y));
            points.Add(startPoint + new Vector3(0, 0, size.x));
            points.Add(startPoint + new Vector3(size.x,0,0));
            points.Add(startPoint);
        }
    }

    private void AddPoints()
    {
        points.Clear();
        
        
    }
    
    private void AddSomePoints()
    {
        SetPoints(Directions.Down, new Vector2(100,100), transform.localPosition);
        mesh.AddQuadWithPointList(points);
    }
    
    private void AddSomePoints2()
    {
        SetPoints(Directions.Up, new Vector2(25,50),transform.localPosition);
        mesh.AddQuadWithPointList(points);
        
        SetPoints(Directions.Up, new Vector2(25,50),transform.localPosition + new Vector3(0,0, 75));
        mesh.AddQuadWithPointList(points);
    }
}
