using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMesh_Wall : AdvancedMesh
{
    public Dictionary<int, MeshPanel> WallTiles = new ();

    public Vector3 GetPositionAtIndex(int panelIndex, int addIndex = 0) 
        => theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + addIndex];
    
    
    public void CreateNewPanel(Vector3 theStart, Vector3 theSize, Vector3 theDirection, int wallIndex)
    {
        if (wallIndex < 0)
        {
            
        }
        
        var wallOrFloor = theDirection.y != 0;
        
        var points =
            MeshStatic.SetVertexPositions(theStart, theSize, wallOrFloor, theDirection);
        var vertIndex = AddQuadWithPointList(points);
        var meshPanel = new MeshPanel(vertIndex, theDirection);
        
        if(!WallTiles.ContainsKey(wallIndex))
            WallTiles.Add(wallIndex, new MeshPanel(vertIndex, theDirection));
    }
    
    
    public void AddDoorway(Vector3 aStart, Vector2 openingSize, Vector3 direction, float totalHeight, int panelIndex)
    {
        var actualSize = new Vector3();
        var aDirection = new Vector3(0,1,1);
        var aDirection2 = new Vector3(-1,0,1);
        if (direction.x != 0)
        {
            actualSize = new Vector3(openingSize.x, openingSize.y, 0.1f);
        }
        
        if (direction.z != 0)
        {
            actualSize = new Vector3(0.1f, openingSize.y, openingSize.x);
            aDirection = new Vector3(1,1,0);
        }
        
        float remainingH = totalHeight - openingSize.y;
        int wallIndex = panelIndex + 50;
        CreateNewPanel(aStart, actualSize, new Vector3(1,0,1), wallIndex);
        var index = WallTiles[panelIndex + 50].startTriangleIndex;
        
        CreateNewPanel(theMesh.vertices[index], actualSize, aDirection, wallIndex);
        CreateNewPanel(theMesh.vertices[index + 3], actualSize, new Vector3(-aDirection.x,1,-aDirection.z), wallIndex);
                       
        CreateNewPanel(theMesh.vertices[index + 1] + new Vector3(0,openingSize.y,0), actualSize, aDirection2, wallIndex);
        
    }
}
