using System.Collections;
using System.Collections.Generic;
using System.Numerics;
using Vector3 = UnityEngine.Vector3;

public class AdvancedMesh_Floor : AdvancedMesh
{
    public Dictionary<Vector3, MeshPanel> FloorTiles = new ();


    public List<Vector3> GetPositionFromTile(Vector3 tileIndex)
    {
        var theTile = FloorTiles[tileIndex];
        List<Vector3> pos = new List<Vector3>()
        {
            theMesh.vertices[theTile.startTriangleIndex],
            theMesh.vertices[theTile.startTriangleIndex + 1],
            theMesh.vertices[theTile.startTriangleIndex + 2],
            theMesh.vertices[theTile.startTriangleIndex + 3],
        };

        return pos;
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
        var panel = FloorTiles[floorIndex];
        var newStart = theMesh.vertices[panel.startTriangleIndex];

        if (directions == Vector3.right)
        {
            newStart = theMesh.vertices[panel.startTriangleIndex +  1];
        }
        
        else if (directions == Vector3.left)
        {
            newStart = theMesh.vertices[panel.startTriangleIndex] - new Vector3(newSize.x,0,0);
        }
        else if (directions == Vector3.forward)
        {
            newStart = theMesh.vertices[panel.startTriangleIndex + 2];
        }
        
        else if (directions == Vector3.back)
        {
            newStart = theMesh.vertices[panel.startTriangleIndex ] - new Vector3(0,0, newSize.z);
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
}
