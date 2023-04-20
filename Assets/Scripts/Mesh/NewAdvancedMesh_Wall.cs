using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{

    [SerializeField] private Vector3 direction;
    [SerializeField] private int length;
    [SerializeField] private int numberTiles;
    
    
    
    private void Awake()
    {
        InitMesh();
    }
    
    protected override void InitMesh()
    {
        base.InitMesh();
        
    }


    private void MakeWall()
    {
        
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var single = length / numberTiles;

        for (int i = 0; i < numberTiles; i++)
        {
            Gizmos.DrawSphere(pos, 6f);
            pos += direction * single;
        }


    }
}
