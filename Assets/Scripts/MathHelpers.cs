using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MathHelpers 
{
    public static bool IsPointInSquare(Vector3 min, Vector3 max, Vector3 point)
    {
        if (point.x < min.x || point.y < min.y || point.z < min.z) return false;
        if (point.x > max.x || point.y > max.y || point.z > max.z) return false;
        
        return true;
    }

    public static Vector2 WhereIsPoint(Vector3 min, Vector3 max, Vector3 point)
    {
        var outVec = new Vector2();
        
        var calc = point.x + point.z;
        var calc2 = min.x + min.z;
        var calc3 = max.x + max.z;
        
        if (point.y < min.y) outVec.y = -1;
        else if (point.y >= min.y) outVec.x = point.y < max.y ? 0 : 1;
        
        if (calc < calc2) outVec.x = -1;
        else if (calc >= calc2) outVec.x = calc < calc3 ? 0 : 1;
        return outVec;
    }
    
    public static void PlaneDirections(Vector3 normal, out Vector3 planeUp, out Vector3 planeRight)
    {
        planeUp = Vector3.ProjectOnPlane(normal.x + normal.z == 0 
            ? new Vector2(1,0) : new Vector2(0,1), normal);
        planeRight = Vector3.Cross(normal, planeUp);
    }
}
