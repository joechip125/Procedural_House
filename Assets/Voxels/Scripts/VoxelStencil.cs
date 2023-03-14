using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VoxelStencil 
{
    protected bool fillType;
    protected int centerX, centerY, radius;

    public virtual void Initialize (bool fillType, int radius) 
    {
        this.fillType = fillType;
        this.radius = radius;
    }

    public int XStart => centerX - radius;

    public int XEnd => centerX + radius;

    public int YStart => centerY - radius;

    public int YEnd => centerY + radius;


    public virtual void SetCenter (int x, int y) 
    {
        centerX = x;
        centerY = y;
    }
    public virtual bool Apply (int x, int y, bool voxel) 
    {
        return fillType;
    }
}
