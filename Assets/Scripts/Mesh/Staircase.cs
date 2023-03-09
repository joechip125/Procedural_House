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
    private void Awake()
    {
        if (!Application.isEditor) return;
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        mesh.ApplyMaterial(mat);
        Try();
    }

    private void Try()
    {
        var add = new Vector3(0, 30, 0);
        var t1 = transform.position;
        var t2 = t1 + add;
        var t3 = t1 + new Vector3(100,0,0);
        var t4 = t3 + add;
        mesh.AddQuad(t1, t2, t3, t4);
        
        var add2 = new Vector3(0, 30, 0);
        var t5 = transform.position;
        var t6 = t1 + add;
        var t7 = t1 + new Vector3(100,0,0);
        var t8 = t3 + add;
        mesh.AddQuad(t1, t2, t3, t4);
    }
}
