using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum AddDirection
{
    North,
    East,
    South,
    West,
    NorthSouth,
    EastWest
    
}

public enum SegmentTypes
{
    
}


[Serializable]
public class Segment
{
    public Vector3[] positions;
    public Vector3 position;
    public Vector3 size;
    public SegmentTypes type;

    public Segment(int numberPos)
    {
        
        positions = new Vector3[numberPos];
    }
}
