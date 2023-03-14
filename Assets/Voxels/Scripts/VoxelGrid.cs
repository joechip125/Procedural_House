using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[SelectionBase]
public class VoxelGrid : MonoBehaviour
{
    private Mesh mesh;

    private List<Vector3> vertices;
    private List<int> triangles;
    
    public int resolution;
    
    public GameObject voxelPrefab;
    
    private float voxelSize;

    private Voxel[] voxels;
    //private bool[] voxels;
    
    private Material[] voxelMaterials;
    
    
    public void Initialize (int resolution, float size) 
    {
        this.resolution = resolution;
        voxelSize = size / resolution;
        voxels = new Voxel[resolution * resolution];
        
        voxelMaterials = new Material[voxels.Length];

        for (int i = 0, y = 0; y < resolution; y++) 
        {
            for (int x = 0; x < resolution; x++, i++) 
            {
                CreateVoxel(i, x, y);
            }
        }

        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "VoxelGrid Mesh";
        vertices = new List<Vector3>();
        triangles = new List<int>();
        Refresh();
    }

    private void Refresh()
    {
        SetVoxelColors();
        Triangulate();
    }

    private void Triangulate()
    {
        vertices.Clear();
        triangles.Clear();
        mesh.Clear();

        TriangulateCellRows();

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
    }

    private void TriangulateCellRows () 
    {
        int cells = resolution - 1;
        for (int i = 0, y = 0; y < cells; y++, i++) 
        {
            for (int x = 0; x < cells; x++, i++) 
            {
                TriangulateCell(voxels[i], voxels[i + 1], voxels[i + resolution], voxels[i + resolution + 1]);
            }
        }
    }

    private void TriangulateCell (Voxel a, Voxel b, Voxel c, Voxel d)
    {
        int cellType = 0;
        if (a.state) cellType |= 1;
        
        if (b.state) cellType |= 2;
        
        if (c.state) cellType |= 4;
        
        if (d.state) cellType |= 8;
        
        
        switch (cellType) 
        {
            case 0:
                return;
            case 1:
                AddTriangle(a.position, a.yEdgePosition, a.xEdgePosition);
                break;
        }
    }
   
    private void AddTriangle (Vector3 a, Vector3 b, Vector3 c) 
    {
        int vertexIndex = vertices.Count;
        vertices.Add(a);
        vertices.Add(b);
        vertices.Add(c);
        triangles.Add(vertexIndex);
        triangles.Add(vertexIndex + 1);
        triangles.Add(vertexIndex + 2);
    }

    private void SetVoxelColors()
    {
        for (int i = 0; i < voxels.Length; i++) 
        {
            voxelMaterials[i].color = voxels[i].state ? Color.black : Color.white;
        }
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
                voxels[i].state = stencil.Apply(x, y, voxels[i].state);
            }
        }
        
        Refresh();
    }

    private void CreateVoxel (int i, int x, int y) 
    {
        voxels[i] = new Voxel(x, y, voxelSize);
        
        GameObject o = Instantiate(voxelPrefab) as GameObject;
        o.transform.parent = transform;
        o.transform.localPosition = new Vector3((x + 0.5f) * voxelSize, (y + 0.5f) * voxelSize, -0.01f);
        o.transform.localScale = Vector3.one * voxelSize * 0.1f;
        
        voxelMaterials[i] = o.GetComponent<MeshRenderer>().material;
    }
}
