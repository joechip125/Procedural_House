using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TilesType
{
    Floor, Wall
}



[Serializable]
public class MeshTiles
{
    public TilesType TilesType;
    public List<MeshPanel> panels = new ();
    
}

[Serializable]
public class FloorTileValues
{
    public Vector3 min;
    public Vector3 max;
    public List<Vector3> pos = new ();
}


[Serializable]
public class WallTiles
{
    public Dictionary<int, MeshPanel> panels = new ();
}

[Serializable]
public class RoomTiles
{
    public List<MeshPanel> panels = new ();
    public Vector3 coordinate;
    public Dictionary<int, MeshPanel> floorPanels;
}

[Serializable]
public class FloorTile
{
    public Vector3 coordinate;
    public int startTriangleIndex;
    public string roomName;
}

