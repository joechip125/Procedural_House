using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshStatic : MonoBehaviour
{
    public static float WallThickness = 0.3f;
    
    public static Vector3[] DirectionVectors()
    {
        return new[] {new Vector3(1,0,0),};
    }

    public static Vector3 GetCenterOfFloorSquare(Vector3 size, Vector3 start)
    {
        return new Vector3(start.x + size.x / 2, start.y, start.z + size.z/ 2);
    }

    public static Vector3 GetSizeOfTile(Vector3 min, Vector3 max)
    {
        return new Vector3(max.x - min.x, max.y - min.y, max.z - min.z);
    }
    
    
    public static List<Vector3> SetVertexPositions(Vector3 start, Vector3 size, bool wallFloor, Vector3 direction)
    {
        var vertexPositions = new List<Vector3> {start};

        if (wallFloor)
        {
            vertexPositions.Add(start + new Vector3(size.x * direction.x, 0, size.z * direction.z));
            vertexPositions.Add(start + new Vector3(0, size.y * direction.y, 0));
            vertexPositions.Add(start + new Vector3(size.x * direction.x, size.y * direction.y, size.z * direction.z));
        }
        else
        {
            vertexPositions.Add(start + new Vector3(size.x * direction.x, 0,0));
            vertexPositions.Add(start + new Vector3(0, 0, size.z * direction.z));
            vertexPositions.Add(start + new Vector3(size.x * direction.x, 0, size.z * direction.z));
        }

        return vertexPositions;
    }
}
