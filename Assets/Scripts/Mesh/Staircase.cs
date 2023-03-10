using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AdvancedMesh))]
[ExecuteInEditMode]
public class Staircase : MonoBehaviour
{
    private AdvancedMesh mesh;
    [SerializeField] private Material mat;
    [Range(2, 40)]public float height;
    [Range(2, 100)]public float width;
    [Range(2, 20)] public int numberStairs;

    private void Awake()
    {
        if (!Application.isEditor) return;
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        mesh.ApplyMaterial(mat);
    }


    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var w = new Vector3(width, 0, 0);
        Gizmos.DrawSphere(pos, 2);
        Gizmos.DrawSphere(pos + new Vector3(width,0,0), 2);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + w);
        
        for (int i = 0; i < numberStairs - 1; i++)
        {   
            Gizmos.color = Color.gray;
            var first = pos + new Vector3(0, height, 0);
            var second = pos + new Vector3(0, 0, -20);
            pos += new Vector3(0, height, 0);
            Gizmos.DrawSphere(pos, 2);
            Gizmos.DrawSphere(pos + new Vector3(width,0,0), 2);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + w);
            
            Gizmos.color = Color.gray;
            pos += new Vector3(0, 0, -20);
            Gizmos.DrawSphere(pos, 2);
            Gizmos.DrawSphere(pos+ new Vector3(width,0,0), 2);
            
            Gizmos.color = Color.red;
            Gizmos.DrawLine(pos, pos + w);
        }
    }


    private void Try()
    {
        var start = transform.position;
        var addH =new Vector3(0, height, 0);
        var addW = new Vector3(width,0,0);
        var addL = new Vector3(0,0,-20);

        for (int i = 0; i < 8; i++)
        {
            var v1 = start;
            var v2 = v1 + addH;
            var v3 = v1 + addW;
            var v4 = v3 + addH;
            start += addH;
            mesh.AddQuad(v1, v2, v3, v4);

            var v5 = start;
            var v6 = v5 + addL;
            var v7 = v5 + addW;
            var v8 = v7 + addL;
            start += addL;
            mesh.AddQuad(v5, v6, v7, v8);
        }
    }
}
