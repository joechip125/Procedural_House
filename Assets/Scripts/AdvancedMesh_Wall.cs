using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AdvancedMesh_Wall : AdvancedMesh
{
    public Dictionary<int, MeshPanel> WallTiles = new ();

    public Vector3 GetPositionAtIndex(int panelIndex, int addIndex = 0) 
        => theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + addIndex];
    
    public Vector3 GetNormalAtIndex(int panelIndex, int addIndex = 0) 
        => theMesh.normals[WallTiles[panelIndex].startTriangleIndex + addIndex];

    
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
    
    public void AddDoorway2(int panelIndex, Vector2 size)
    {
        var wallThick = 0.1f;
        var wallDirection = WallTiles[panelIndex].direction;
        var wallNormal = theMesh.normals[WallTiles[panelIndex].startTriangleIndex];
        var aNewStart = theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + 1];
        
        var xzSize = new Vector3(size.x * wallDirection.x, 0, size.x * wallDirection.z) 
                     + new Vector3(wallThick * Mathf.Abs(wallNormal.x), 0, wallThick * Mathf.Abs(wallNormal.z));
        var xySize = new  Vector3(wallThick * Mathf.Abs(wallNormal.x), size.y, wallThick * Mathf.Abs(wallNormal.z));

        var totalH = theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + 3].y - aNewStart.y;

        var topSize = new Vector3(size.x * wallDirection.x, totalH - size.y, size.x * wallDirection.z);

        var floorSize = new Vector3();
        var actualSize = new Vector3(1,0,1);
        var aDirection = new Vector3(0,1,1);
        var aDirection2 = new Vector3(-1,0,1);
        
        int wallIndex = panelIndex + 50;
        CreateNewPanel(aNewStart, xzSize, new Vector3(1,0,1), wallIndex++);
        var theIndex = theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + 1];
        var index = WallTiles[panelIndex + 50].startTriangleIndex;
        
        CreateNewPanel(theMesh.vertices[index + 1] + new Vector3(0,size.y,0), xzSize, aDirection2, wallIndex);

        CreateNewPanel(theMesh.vertices[index], xySize, new Vector3(-wallNormal.x,1, -wallNormal.z), wallIndex++);
        CreateNewPanel(theMesh.vertices[index + 3], xySize, new Vector3(wallNormal.x,1, wallNormal.z), wallIndex++);
                       
        CreateNewPanel(theMesh.vertices[index + 1] + new Vector3(0,size.y,0), topSize, aDirection2, wallIndex);
        
    }
    
    public void AddDoorway(Vector3 aStart, Vector2 openingSize, Vector3 direction, float totalHeight, int panelIndex, Vector3 wallNormal)
    {
        var wallDirection = new Vector3(-1,1,0);
        var wallNormal2 = new Vector3(1,0,0);
        var sidePiece = new Vector3(wallNormal.x, 1, wallNormal.z);
        
        var floorSize = new Vector3();
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
        CreateNewPanel(aStart, actualSize, new Vector3(1,0,1), wallIndex++);
        var index = WallTiles[panelIndex + 50].startTriangleIndex;
        
        CreateNewPanel(theMesh.vertices[index + 1], actualSize, new Vector3(-wallNormal.x,1, -wallNormal.z), wallIndex++);
        CreateNewPanel(theMesh.vertices[index + 2], actualSize, sidePiece, wallIndex++);
                       
        CreateNewPanel(theMesh.vertices[index + 1] + new Vector3(0,openingSize.y,0), actualSize, aDirection2, wallIndex);
        
    }
}
