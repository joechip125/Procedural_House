using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class AdvancedMesh_Floor : AdvancedMesh
{
    public Dictionary<Vector3, MeshPanel> FloorTiles = new ();


    public List<Vector3> GetPositionFromTile(Vector3 tileIndex)
    {
        var theTile = FloorTiles[tileIndex];
        List<Vector3> pos = new List<Vector3>()
        {
            TheMesh.vertices[theTile.startTriangleIndex],
            TheMesh.vertices[theTile.startTriangleIndex + 1],
            TheMesh.vertices[theTile.startTriangleIndex + 2],
            TheMesh.vertices[theTile.startTriangleIndex + 3],
        };

        return pos;
    }
    
    
    public List<Vector3> GetPositionClockWise(Vector3 tileIndex)
    {
        var theTile = FloorTiles[tileIndex].startTriangleIndex;
        
        List<Vector3> positions = new List<Vector3>()
        {
            TheMesh.vertices[theTile],
            TheMesh.vertices[theTile + 1],
            TheMesh.vertices[theTile + 3],
            TheMesh.vertices[theTile + 2],
        };
        return positions;
        
    }

    public void CreateNewPanel(Vector3 theStart, Vector3 theSize, Vector3 theDirection, Vector3 wallIndex)
    {
        var points =
            MeshStatic.SetVertexPositions(theStart, theSize, false, theDirection);
        var vertIndex = AddQuadWithPointList(points);
        var meshPanel = new MeshPanel(vertIndex, theDirection);
        
        if(!FloorTiles.ContainsKey(wallIndex))
            FloorTiles.Add(wallIndex, new MeshPanel(vertIndex, theDirection));
    }
    
    
    private Vector3 GetNewFloorPos(Vector3 directions, Vector3 newSize, Vector3 floorIndex)
    {
        var min = TheMesh.bounds.min;
        var max = TheMesh.bounds.max;

        var ex = new Vector3(max.x, min.y, max.z);
        
        var panel = FloorTiles[floorIndex];
        var newStart = TheMesh.vertices[panel.startTriangleIndex];

        if (directions == Vector3.right)
        {
            newStart = TheMesh.vertices[panel.startTriangleIndex +  1];
        }
        
        else if (directions == Vector3.left)
        {
            newStart = TheMesh.vertices[panel.startTriangleIndex] - new Vector3(newSize.x,0,0);
        }
        else if (directions == Vector3.forward)
        {
            newStart = TheMesh.vertices[panel.startTriangleIndex + 2];
        }
        
        else if (directions == Vector3.back)
        {
            newStart = TheMesh.vertices[panel.startTriangleIndex ] - new Vector3(0,0, newSize.z);
        }
        
        return newStart;
    }
    
    
    public void AddFloorTile(Vector3 newSize, Vector3 directionFromTile, Vector3 oldIndex, Vector3 addPos)
    {
        if (FloorTiles.ContainsKey(oldIndex + directionFromTile)) return;

        var newDirection = new Vector3(1,0,1);
        var newStart = GetNewFloorPos(directionFromTile, newSize, oldIndex) + addPos;
        var newIndex = oldIndex + directionFromTile;

        var points =
            MeshStatic.SetVertexPositions(newStart, newSize, false, newDirection);

        var triIndex =AddQuadWithPointList(points);

        var newPanel = new MeshPanel(triIndex, newDirection);
        FloorTiles.Add(newIndex, newPanel);
    }
    
    public void AddFloorTile(Vector3 newSize, Vector3 directionFromTile, Vector3 oldIndex)
    {
        if (FloorTiles.ContainsKey(oldIndex + directionFromTile)) return;

        var newDirection = new Vector3(1,0,1);
        var newStart = GetNewFloorPos(directionFromTile, newSize, oldIndex);
        var newIndex = oldIndex + directionFromTile;

        var points =
            MeshStatic.SetVertexPositions(newStart, newSize, false, newDirection);

        var triIndex =AddQuadWithPointList(points);

        var newPanel = new MeshPanel(triIndex, newDirection);

        FloorTiles.Add(newIndex, newPanel);
    }
}
