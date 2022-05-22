using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMesh : AdvancedMesh
{
    public void BuildCylinder(float radius, int numberSegments)
    {

        var center = new Vector3(0, 0, 0);
        var deg = 0;
        var degInc = 360 / numberSegments;
        
        
        for (int i = 0; i < numberSegments; i++)
        {
            var pointOne = GetCircleEdge(deg, center, radius);
            var pointTwo = GetCircleEdge(deg - degInc, center, radius);
            
            AddQuad(pointOne, pointOne + new Vector3(0, 1,0), pointTwo, pointTwo + new Vector3(0,1,0));

            deg += degInc;
        }
    }
    

    public Vector3 GetCircleEdge(float degree, Vector3 center, float extent)
    {
        var cos = Math.Cos(Mathf.PI / 180f * degree) * extent;
        var sin = Math.Sin(Mathf.PI / 180f * degree) * extent;
        
        return center + new Vector3((float)cos, 0, (float) sin);
    }

  
    void Update()
    {
        
    }
}
