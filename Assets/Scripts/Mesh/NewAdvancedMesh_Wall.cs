using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{

    [SerializeField] private Vector3 direction;
    [SerializeField] private float length;
    [SerializeField] private int numberTiles;
    
    private void Awake()
    {
        InitMesh();
    }


    private void MakeWall()
    {
        
    }
    
    
    
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var single = length / numberTiles;
        Gizmos.DrawSphere(pos, 3f);
        pos += direction.normalized * single;

        for (int i = 0; i < numberTiles; i++)
        {
            Gizmos.DrawSphere(pos, 3f);
            pos += direction.normalized * single;
        }
    }
}
