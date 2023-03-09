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
    
    
    private void Awake()
    {
        if (!Application.isEditor) return;
        mesh = GetComponent<AdvancedMesh>();
        mesh.InstanceMesh();
        mesh.ApplyMaterial(mat);
        Try();
    }

    private void AddPanel()
    {
        
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
        }

        var add = new Vector3(0, height, 0);
        var t1 = transform.position;
        var t2 = t1 + addH;
        var t3 = t1 + addW;
        var t4 = t3 + addH;
        mesh.AddQuad(t1, t2, t3, t4);

        var secS = t1 + add;
        
        var add2 = new Vector3(0, 0, -20);
        var t5 =secS;
        var t6 = t5 + add2;
        var t7 = t5 + addL;
        var t8 = t7 + add2;
        mesh.AddQuad(t5, t6, t7, t8);
    }
}
