using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdvancedMesh_Floor : AdvancedMesh
{
    public Dictionary<Vector3, MeshPanel> FloorTiles = new ();
    
    public void CreateNewPanel(Vector3 theStart, Vector3 theSize, Vector3 theDirection, Vector3 wallIndex)
    {
        var wallOrFloor = theDirection.y != 0;
        
        var points =
            MeshStatic.SetVertexPositions(theStart, theSize, wallOrFloor, theDirection);
        var vertIndex = AddQuadWithPointList(points);
        var meshPanel = new MeshPanel(vertIndex, theDirection);
        
        if(!FloorTiles.ContainsKey(wallIndex))
            FloorTiles.Add(wallIndex, new MeshPanel(vertIndex, theDirection));
    }
    
    
    public Vector3 GetNewFloorPos(RoomDirections directions, Vector3 newSize, Vector3 floorIndex)
    {
        var panel = FloorTiles[floorIndex];
        var newStart = theMesh.vertices[panel.startTriangleIndex];
        
        switch (directions)
        {
            case RoomDirections.XPlus:
                newStart = theMesh.vertices[panel.startTriangleIndex +  1];
                break;
            case RoomDirections.XMinus:
                newStart = theMesh.vertices[panel.startTriangleIndex] - new Vector3(newSize.x,0,0);
                break;
            case RoomDirections.ZPlus:
                newStart = theMesh.vertices[panel.startTriangleIndex + 2];
                break;
            case RoomDirections.ZMinus:
                newStart = theMesh.vertices[panel.startTriangleIndex ] - new Vector3(0,0, newSize.z);
                break;
        }

        return newStart;
    }
    
}
