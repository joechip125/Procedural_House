using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NewMeshTest : MonoBehaviour
{
    private AdvancedMesh mesh;
    public Material aMaterial;

    private void Awake()
    {
       mesh = gameObject.AddComponent<AdvancedMesh>();
       AddSomePoints();
    }
    
    private void AddSomePoints()
    {
        var aPoint = transform.localPosition;
        var aPoint2 = aPoint + new Vector3(100, 0, 0);
        var aPoint3 = aPoint + new Vector3(100, 0, 100);
        var aPoint4 = aPoint + new Vector3(0, 0, 100);
        mesh.AddQuad(aPoint, aPoint2, aPoint4, aPoint3);
        mesh.ApplyMaterial(aMaterial);
    }
}
