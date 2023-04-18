using System;
using UnityEngine;

public class NewAdvancedMesh_Door : NewAdvancedMesh
{
    public Vector3 direction;

    [Range(-1, 1)]public float directionX;
    [Range(-1, 1)]public float directionZ;

    private void Awake()
    {
        InitMesh();
    }


    private void OnDrawGizmos()
    {
        var dir = new Vector3(directionX, 0, directionZ);
        var pos = transform.position + new Vector3(0, 50,0);
        var aCross = Vector3.Cross(dir, Vector3.up);
        var aCross2 = Vector3.Cross(dir, Vector3.down);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(pos, pos + dir * 50);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + aCross * 50);
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(pos, pos + aCross2 * 50);
    }
}
