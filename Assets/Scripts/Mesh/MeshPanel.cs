using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshPanel
{
    public int startTriangleIndex;
    public Vector3 direction;

    public MeshPanel(int startTriangleIndex, Vector3 direction)
    {
        this.direction = direction;
        this.startTriangleIndex = startTriangleIndex;
    }
}
