using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{

    [SerializeField] private Vector3 direction;
    [SerializeField] private float length;
    [SerializeField] private float height;
    [SerializeField] private int numberTiles;
    
    private readonly Vector3[] corners = new[]
    {   new Vector3(-1,0,-1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0)};
    
    private void Awake()
    {
        InitMesh();
    }


    private void MakeWall()
    {
        
    }

    protected override void Activate()
    {
        base.Activate();
        var pos = transform.position;
        var single = length / numberTiles;
       
        for (int i = 0; i < numberTiles; i++)
        {
            pos += direction.normalized * single;
        }
        
    }


    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var single = length / numberTiles;
        Gizmos.DrawSphere(pos, 3f);
        Gizmos.DrawSphere(pos + new Vector3(0, height,0), 3f);
        pos += direction.normalized * single;

        for (int i = 0; i < numberTiles; i++)
        {
            Gizmos.DrawSphere(pos, 3f);
            Gizmos.DrawSphere(pos + new Vector3(0, height,0), 3f);
            pos += direction.normalized * single;
        }
    }
}
