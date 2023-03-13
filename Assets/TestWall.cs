using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

[Serializable]
public class Doorway
{
    public float theStart;
    [Range(10, 500)]public float sizeX;
    [Range(10, 500)]public float sizeY;
    public Doorway(float start)
    {
        theStart = start;
    }
}

public class TestWall : MonoBehaviour
{
    [Range(10, 500)]public float sizeX;
    [Range(10, 500)]public float sizeY;
    public List<Doorway> doors = new ();

    public void AddDoor()
    {
        
    }
    
    void Start()
    {
        
    }
    
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var max = pos + new Vector3(sizeX, sizeY, 0);
        
        Gizmos.color = Color.red;
        Gizmos.DrawLine(pos, pos + new Vector3(0, sizeY,0));
        Gizmos.DrawLine(pos, pos + new Vector3(sizeX, 0,0));
        
        Gizmos.DrawLine(pos+ new Vector3(0, sizeY,0), pos+ new Vector3(0, sizeY,0) + new Vector3(sizeX, 0,0));
        
        Gizmos.DrawLine(pos+ new Vector3(sizeX, 0,0), pos + new Vector3(sizeX, 0,0) + new Vector3(0, sizeY,0));

        for (int i = 0; i < doors.Count; i++)
        {
            var start = pos + new Vector3(doors[i].theStart, 0, 0);
            var theX = doors[i].sizeX;
            var theY = doors[i].sizeY;
            Gizmos.DrawLine(start, start + new Vector3(0, theY, 0));
        }
        
    }
}
