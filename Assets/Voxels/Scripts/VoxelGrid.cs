using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    public int resolution;
    
    public GameObject voxelPrefab;
    
    private float voxelSize;

    private bool[] voxels;
    
    private Material[] voxelMaterials;
    
    
    public void Initialize (int resolution, float size) 
    {
        this.resolution = resolution;
        voxelSize = size / resolution;
        voxels = new bool[resolution * resolution];
        
        voxelMaterials = new Material[voxels.Length];

        for (int i = 0, y = 0; y < resolution; y++) 
        {
            for (int x = 0; x < resolution; x++, i++) 
            {
                CreateVoxel(i, x, y);
            }
        }
        SetVoxelColors();
    }

    private void SetVoxelColors()
    {
        for (int i = 0; i < voxels.Length; i++) 
        {
            voxelMaterials[i].color = voxels[i] ? Color.black : Color.white;
        }
    }

    public void SetVoxel (int x, int y, bool state) 
    {
        voxels[y * resolution + x] = state;
        SetVoxelColors();
    }
    
    public void Apply (VoxelStencil stencil) 
    {
        int xStart = stencil.XStart;
        if (xStart < 0) 
        {
            xStart = 0;
        }
        int xEnd = stencil.XEnd;
        if (xEnd >= resolution) 
        {
            xEnd = resolution - 1;
        }
        int yStart = stencil.YStart;
        if (yStart < 0) 
        {
            yStart = 0;
        }
        int yEnd = stencil.YEnd;
        if (yEnd >= resolution) 
        {
            yEnd = resolution - 1;
        }

        for (int y = yStart; y <= yEnd; y++) 
        {
            int i = y * resolution + xStart;
            for (int x = xStart; x <= xEnd; x++, i++) 
            {
                voxels[i] = stencil.Apply(x, y);
            }
        }
        SetVoxelColors();
    }

    private void CreateVoxel (int i, int x, int y) 
    {
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;
        o.transform.localPosition = new Vector3((x + 0.5f) * voxelSize, (y + 0.5f) * voxelSize);
        o.transform.localScale = Vector3.one * voxelSize * 0.9f;
        
        voxelMaterials[i] = o.GetComponent<MeshRenderer>().material;
    }
}
