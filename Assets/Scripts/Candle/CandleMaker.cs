using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CandleMaker : MonoBehaviour
{
    private CylinderMesh cylinder;
    private bool shrink;
    
    
    void Start()
    {
        cylinder = gameObject.AddComponent<CylinderMesh>();
        //cylinder.BuildCylinder2();
        cylinder.BuildCylinder(0.3f, 12);
        shrink = true;
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
