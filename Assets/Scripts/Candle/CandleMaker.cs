using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleMaker : MonoBehaviour
{
    private CylinderMesh cylinder;
    private bool shrink;
    public Material aMaterial;
    
    
    void Start()
    {
        cylinder = gameObject.AddComponent<CylinderMesh>();
       // cylinder.BuildCylinder(1, 12, 1);
        cylinder.BuildCircle(0.5f, 24);
        cylinder.BuildRing(0.5f, 0.5f,24);
    //    shrink = true;
        cylinder.ApplyMaterial(aMaterial);
    
    }


    public void ShrinkCylinder()
    {
        shrink = true;
    }
    
    
    void Update()
    {
        if (!shrink) return;
        cylinder.MovePoints(Time.deltaTime * 0.01f);
        
        
    }
}
