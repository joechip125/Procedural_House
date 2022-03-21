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
public class RoomTiles
{
    public List<MeshPanel> panels = new ();
    public Vector3 coordinate;
    public Dictionary<int, MeshPanel> floorPanels;
}

