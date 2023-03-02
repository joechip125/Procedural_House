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
    public List<Vector3> points = new List<Vector3>();

    private void Awake()
    {
       mesh = gameObject.AddComponent<AdvancedMesh>();
       mesh.ApplyMaterial(aMaterial);
       AddSomePoints();
       AddSomePoints2();
    }
    
    private void SetPoints(Directions direction, Vector2 size)
    {
        points.Clear();
        var startPoint = transform.localPosition;
        points.Add(startPoint);
        if (direction == Directions.Up)
        {
            points.Add(startPoint + new Vector3(0, size.y));
            points.Add(startPoint + new Vector3(0, size.y, size.x));
            points.Add(startPoint + new Vector3(0, 0, size.x));
        }
        else
        {
            points.Add(startPoint + new Vector3(size.x,0,0));
            points.Add(startPoint + new Vector3(size.x, 0, size.y));
            points.Add(startPoint + new Vector3(0, 0, size.x));
        }
    }
    
    private void AddSomePoints()
    {
        SetPoints(Directions.Down, new Vector2(100,100));
        mesh.AddQuadWithPointList(points);
    }
    
    private void AddSomePoints2()
    {
        var aPoint = transform.localPosition;
        var aPoint2 = aPoint + new Vector3(0, 100, 0);
        var aPoint3 = aPoint + new Vector3(0, 100, 100);
        var aPoint4 = aPoint + new Vector3(0, 0, 100);
        mesh.AddQuad(aPoint, aPoint2, aPoint4, aPoint3);
    }
}
