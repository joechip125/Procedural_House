using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class AdvancedMesh_Wall : AdvancedMesh
{
    public Dictionary<int, MeshPanel> WallTiles = new ();

    public Vector3 GetPositionAtIndex(int panelIndex, int addIndex = 0) 
        => theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + addIndex];
    
    public Vector3 GetNormalAtIndex(int panelIndex, int addIndex = 0) 
        => theMesh.normals[WallTiles[panelIndex].startTriangleIndex + addIndex];


    public void MoveWallVerts(int firstIndex, Vector3 moveAmount, Vector3 sideToMove, int lastIndex = 0)
    {
        firstIndex = 2;

        moveAmount = new Vector3(0,0,-1.25f);
        var panelOne = WallTiles[firstIndex];
        
        
        
        if (lastIndex != 0)
        {
            
        }
        else
        {
            
        }
    }


    public void ShrinkWall(int tileIndex, float shrinkAmount)
    {
        var startTri = WallTiles[tileIndex].startTriangleIndex;
        var moveDir = Vector3.Cross(new Vector3(0, 1, 0), -theMesh.normals[startTri]);
        Vector3 shrink = new Vector3(shrinkAmount * moveDir.x, 0, shrinkAmount * moveDir.z);

        MoveTwoVerts(startTri + 1, startTri + 3, shrink);
    }

    public void CreateNewPanel(Vector3 theStart, Vector3 theSize, Vector3 theDirection, int wallIndex)
    {
        
        //   Debug.Log(theDirection);
        //   if (WallTiles.ContainsKey(wallIndex)) return;
        
        var points =
            MeshStatic.SetVertexPositions(theStart, theSize, true, theDirection);
        var vertIndex = AddQuadWithPointList(points);

     
        WallTiles.Add(wallIndex, new MeshPanel(vertIndex, theDirection));
    }

    public void AddPanel(Vector3 theSize, Vector3 theDirection, bool startEnd = false)
    {
        var theIndex = 0;
        Vector3 theStart;
        
        if (!startEnd)
        {
            var maxKey = WallTiles.Keys.Max();
            theStart = theMesh.vertices[WallTiles[maxKey].startTriangleIndex + 1];
            theIndex = maxKey + 1;
        }
        else
        {
            var minKey = WallTiles.Keys.Min();
            theStart = theMesh.vertices[WallTiles[minKey].startTriangleIndex];
            theStart -= new Vector3(theSize.x * theDirection.x, 0, theSize.z * theDirection.z);

            theIndex = minKey - 1;
        }
        

        var points =
            MeshStatic.SetVertexPositions(theStart, theSize, true, theDirection);
        var vertIndex = AddQuadWithPointList(points);

        if(!WallTiles.ContainsKey(theIndex))
            WallTiles.Add(theIndex, new MeshPanel(vertIndex, theDirection));
    }
    
    public void MakeWallOpening(int firstPanel, float openingSize)
    {
        var panelOne = WallTiles[firstPanel];
        var panelTwo = WallTiles[firstPanel + 1];

        var direction = panelOne.direction;
        var addVec = new Vector3(panelOne.direction.x * openingSize / 2, 0, direction.z * openingSize / 2);

        var theNormal = theMesh.normals[panelOne.startTriangleIndex + 1];


        theMesh.vertices[panelOne.startTriangleIndex + 1] -= addVec;
        theMesh.vertices[panelOne.startTriangleIndex + 3] -= addVec;

        theMesh.vertices[panelTwo.startTriangleIndex] += addVec;
        theMesh.vertices[panelTwo.startTriangleIndex + 2] += addVec;
     
        var theNormal2 = new Vector3(-theNormal.x,1, -theNormal.z);
        
    //    var newPanel = new Vector3(MeshStatic.OuterWallThickness, size.y, MeshStatic.OuterWallThickness);
        var doorSize = new Vector3(Mathf.Abs(openingSize * direction.x), 2, Mathf.Abs(openingSize * direction.z));
    //    AddDoorway(new Vector3(1,0,1), vertices[panelOne.startTriangleIndex + 1] + new Vector3(-0.1f,0,0), doorSize);
    }
    
    public void AddDoorway(int panelIndex, Vector2 size)
    {
        var wallThick = MeshStatic.InnerWallThickness;
        var wallDirection = WallTiles[panelIndex].direction;
        var wallNormal = theMesh.normals[WallTiles[panelIndex].startTriangleIndex];
        var aNewStart = theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + 1];
        
        var xzSize = new Vector3(size.x * wallDirection.x, 0, size.x * wallDirection.z) 
                     + new Vector3(wallThick * Mathf.Abs(wallNormal.x), 0, wallThick * Mathf.Abs(wallNormal.z));
        var xySize = new  Vector3(wallThick * Mathf.Abs(wallNormal.x), size.y, wallThick * Mathf.Abs(wallNormal.z));

        var totalH = theMesh.vertices[WallTiles[panelIndex].startTriangleIndex + 3].y - aNewStart.y;

        var topSize = new Vector3(size.x * wallDirection.x, totalH - size.y, size.x * wallDirection.z);
        
        var aDirection2 = new Vector3(-1,0,1);
        var aDirection3 = new Vector3(1,1,0);
        
        int wallIndex = panelIndex + 50;
        CreateNewPanel(aNewStart, xzSize, new Vector3(1,0,1), wallIndex++);
        var index = WallTiles[panelIndex + 50].startTriangleIndex;
        
        CreateNewPanel(theMesh.vertices[index + 1] + new Vector3(0,size.y,0), xzSize, aDirection2, wallIndex++);

        CreateNewPanel(theMesh.vertices[index], xySize, new Vector3(-wallNormal.x,1, -wallNormal.z), wallIndex++);
        CreateNewPanel(theMesh.vertices[index + 3], xySize, new Vector3(wallNormal.x,1, wallNormal.z), wallIndex++);
                       
        CreateNewPanel(theMesh.vertices[index] + new Vector3(0,size.y,0), topSize, aDirection3, wallIndex);
        
    }
}
