using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{
    
    private void Awake()
    {
        InitMesh();
    }
    
    protected override void InitMesh()
    {
        base.InitMesh();
        
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.DrawSphere(pos, 6f);
    }
}
