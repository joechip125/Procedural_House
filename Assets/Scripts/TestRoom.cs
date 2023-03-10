using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AdvancedMesh))]
[ExecuteInEditMode]
public class TestRoom : MonoBehaviour
{
    private AdvancedMesh mesh;
    [SerializeField, Range(10, 1000)] private float sizeX;
    [SerializeField, Range(10, 1000)] private float sizeZ;
    [SerializeField, Range(10, 300)] private float sizeY;
    
    private void Awake()
    {
        if (!Application.isEditor) return;
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
    }

    private void Start()
    {
        if (!Application.isPlaying) return;
        Debug.Log("something");
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var add = new Vector3(0, sizeY, 0);
        Gizmos.DrawCube(pos, new Vector3(sizeX, 1, sizeZ));
        var p = new Vector3(pos.x + sizeX / 2, pos.y, pos.z + sizeZ / 2);
        var p2 = new Vector3(pos.x - sizeX / 2, pos.y, pos.z - sizeZ / 2);
        var p3 = new Vector3(pos.x + sizeX / 2, pos.y, pos.z - sizeZ / 2);
        var p4 = new Vector3(pos.x - sizeX / 2, pos.y, pos.z + sizeZ / 2);
        
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(p, 4);
        Gizmos.DrawSphere(p2, 4);
        Gizmos.DrawSphere(p3, 4);
        Gizmos.DrawSphere(p4, 4);
        
        Gizmos.DrawSphere(p + add, 4);
        Gizmos.DrawSphere(p2 + add, 4);
        Gizmos.DrawSphere(p3 + add, 4);
        Gizmos.DrawSphere(p4 + add, 4);
        
        Gizmos.color = Color.red;
        
        Gizmos.DrawLine(p, p + add);
        Gizmos.DrawLine(p2, p2 + add);
        Gizmos.DrawLine(p3, p3 + add);
        Gizmos.DrawLine(p4, p4 + add);
        
        
        Gizmos.DrawLine(p3 + add, p + add);
        Gizmos.DrawLine(p4 + add, p + add);
        Gizmos.DrawLine(p4 + add, p2 + add);
        Gizmos.DrawLine(p2 + add, p3 + add);

        for (int i = 0; i < 4; i++)
        {
            
        }
        
    }
}
