using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CylinderMesh : AdvancedMesh
{
    public void BuildCylinder(float radius, int numberSegments, float height)
    {

        var center = new Vector3(0, 0, 0);
        var deg = 0;
        var degInc = 360 / numberSegments;
        
        
        for (int i = 0; i < numberSegments; i++)
        {
            var pointOne = GetCircleEdge(deg, center, radius);
            var pointTwo = GetCircleEdge(deg - degInc, center, radius);
            
            AddQuad(pointOne, pointOne + new Vector3(0, height,0), pointTwo, pointTwo + new Vector3(0,height,0));

            deg += degInc;
        }
    }

    public void BuildCircle(float radius, int numberSegments, float height = 0)
    {
        var center = new Vector3(0, height, 0);
        var deg = 0;
        var degInc = 360 / numberSegments;
        
        
        for (int i = 0; i < numberSegments; i++)
        {
            var pointOne = GetCircleEdge(deg, center, radius);
            var pointTwo = GetCircleEdge(deg + degInc, center, radius);
          //  AddTriangle(center, pointOne, pointTwo);
            AddTriangle(center, pointTwo, pointOne);
           // AddQuad(pointOne, pointOne + new Vector3(0, 1,0), pointTwo, pointTwo + new Vector3(0,1,0));

            deg += degInc;
        }
    }

    public void BuildRing(float beginRadius, float ringExtent, int numberSegments)
    {
        var degInc = 360 / numberSegments;
        var center = new Vector3(0, 0, 0);
        var endHeight = new Vector3(0, -0.3f, 0);
        var startHeight = new Vector3(0, 0, 0);
        var deg = 0;

        for (int i = 0; i < numberSegments; i++)
        {
            var pointOne = GetCircleEdge(deg, center + startHeight, beginRadius);
            var pointTwo = GetCircleEdge(deg + degInc, center + startHeight, beginRadius);
            var pointThree = GetCircleEdge(deg, center + endHeight, beginRadius + ringExtent);
            var pointFour = GetCircleEdge(deg + degInc, center + endHeight, beginRadius + ringExtent);
        
            AddQuad(pointOne, pointThree, pointTwo, pointFour);
            deg += degInc;
        }
    }
    

    public Vector3 GetCircleEdge(float degree, Vector3 center, float extent)
    {
        var cos = Math.Cos(Mathf.PI / 180f * degree) * extent;
        var sin = Math.Sin(Mathf.PI / 180f * degree) * extent;
        
        return center + new Vector3((float)cos, 0, (float) sin);
    }
    
    public void MovePoints(float moveAmount)
    {
     
        
        for (int i = 0; i < vertices.Count; i+= 4)
        {
            if (i + 3 >= vertices.Count) break;
            
            vertices[i + 1] -= new Vector3(0, moveAmount, 0);
            vertices[i + 3] -= new Vector3(0, moveAmount, 0);
        }
        UpdateMesh();
    }

}
