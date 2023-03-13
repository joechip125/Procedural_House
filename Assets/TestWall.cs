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


[Serializable]
public class Panel
{
    public float theStart;
    [Range(10, 500)]public float sizeX;
    [Range(10, 500)]public float sizeY;
    public Panel(float start)
    {
        theStart = start;
    }
}

public class TestWall : MonoBehaviour
{
    [Range(1, 50)]public float sizeX;
    [Range(1, 50)]public float sizeY;
    [Range(1, 50)] public float resolution;
    
    public List<Panel> panels = new ();
    
    
    
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
        var pos =  transform.position;

        for (int i = 0; i < sizeY; i++)
        {
            for (int y = 0; y < sizeX; y++)
            {
                Gizmos.DrawSphere(pos, 2);
                pos += new Vector3(resolution, 0, 0);
            }
            pos = transform.position + new Vector3(0, resolution * i, 0);
        }
        
    }
}
