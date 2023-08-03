using System;
using System.Collections.Generic;
using TMPro.EditorUtilities;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public abstract class NewAdvancedMesh : MonoBehaviour
{
    protected Mesh TheMesh;
    MeshCollider meshCollider;
    [NonSerialized] protected List<Vector3> Vertices = new ();
    [NonSerialized] protected List<int> Triangles = new ();
    protected Vector3[] Directions = new Vector3[4];
    
    protected Vector3[] TempVectors = new Vector3[4];
    
    private readonly Vector3[] panelCorners = new[]
    {   new Vector3(-1,0,-1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0)};

    protected List<Vector3> Corners = new();

    protected virtual void InitMesh()
    {
        GetComponent<MeshFilter>().mesh = TheMesh = new Mesh();
        TheMesh.name = "TheMesh";
    }

    protected virtual void Activate()
    {
        
    }
    
    protected virtual void Register()
    {
        
    }
    
    protected virtual void GetMinMax(out Vector3 min, out Vector3 max)
    {
        min = Vector3.zero;
        max = Vector3.zero;
    }

    protected void ClearMesh()
    {
        Vertices.Clear();
        Triangles.Clear();
        UpdateMesh();
    }
    
    protected void UpdateMesh()
    {
        TheMesh.Clear();
        TheMesh.SetVertices(Vertices);
        TheMesh.SetTriangles(Triangles, 0);
        TheMesh.RecalculateNormals();
    }

    public void RotateAroundAxis(Vector3 normalDir, int numberDir, float degreeInc, float startDeg = 0)
    {
        if (Directions.Length != numberDir)
        {
            Directions = new Vector3[numberDir];
        }
        var aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        if (normalDir.y != 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
        }

        for (int i = 0; i < numberDir; i++)
        {
            var aCrossUp = Quaternion.AngleAxis(startDeg +(degreeInc * i), normalDir) *aCrossForward;
            Directions[i] = aCrossUp;
        }
    }
    
    
    protected void SetSquare(Vector3 normalDir, Vector3 pos, Vector2 size, float degreeAdjust = 0)
    {
        var aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        if (normalDir.y != 0) aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
        
        if (TempVectors.Length != 4) TempVectors = new Vector3[4];
        
        for (int i = 0; i < TempVectors.Length; i++)
        {
            TempVectors[i]  = Quaternion.AngleAxis(degreeAdjust + 90 * i, normalDir) *aCrossForward;
        }
        
        Corners.Clear();
        var tempPos = new Vector3();

        size /= 2;
        for (int i = 0; i < TempVectors.Length; i++)
        {
            if (i == TempVectors.Length - 1) tempPos = pos + TempVectors[0] * size.x + TempVectors[^1] * size.y;
            else
            {
                if (i % 2 == 0) tempPos = pos + TempVectors[i] * size.x + TempVectors[i + 1] * size.y;
                else tempPos = pos + TempVectors[i + 1] * size.x + TempVectors[i] * size.y;
            }
            Corners.Add(tempPos);
        }
    }
    
    public Vector3 RotateAroundAxisReturn(Vector3 normalDir,float degreeInc, float startDeg = 0)
    {
        var aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        if (normalDir.y != 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
        }
        var aCrossUp = Quaternion.AngleAxis(startDeg + degreeInc, normalDir) *aCrossForward;

        return aCrossUp;
    }
    
    protected void SimplePanel(Vector3 addPos, Vector3 normalDir, Vector2 theSize, int addDegree = 0)
    {
        Vector3 aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        var flip = false;

        if (normalDir.y != 0)
        {
            aCrossForward = Vector3.Cross(normalDir, Vector3.forward).normalized;
            flip = true;
        }

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp = Quaternion.AngleAxis((90 * i) + addDegree , normalDir) *aCrossForward;
            var aCrossUp2 = Quaternion.AngleAxis((90 * i) + 90 + addDegree, normalDir) *aCrossForward;
            
            var poss = new Vector3();
            if (!flip) poss = (aCrossUp * (theSize.x / 2)) + (aCrossUp2 * (theSize.y / 2));
            else poss = aCrossUp * theSize.y / 2 + (aCrossUp2 * theSize.x / 2);
            
            panelCorners[i] = poss + addPos;
            
            flip = !flip;
        }
        
        AddQuad(panelCorners[0], panelCorners[1], panelCorners[2], panelCorners[3]);
    }
    
    protected void RemoveTriangle(int start, int count)
    {
        for (int i = start; i < count; i++)
        {
            Vertices.RemoveAt(i);
        }    
    }
    
    protected void AddTriangle(Vector3 v1,Vector3 v2, Vector3 v3)
    {
        var vertexIndex = Vertices.Count;
        Vertices.Add(v1);
        Vertices.Add(v2);
        Vertices.Add(v3);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        
        UpdateMesh();
    }
    protected int AddQuad(Vector3 v1, Vector3 v2, Vector3 v3, Vector3 v4) 
    {
        var vertexIndex = Vertices.Count;
        Vertices.Add(v1);
        Vertices.Add(v2);
        Vertices.Add(v3);
        Vertices.Add(v4);
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 1);
        Triangles.Add(vertexIndex + 2);
        
        Triangles.Add(vertexIndex);
        Triangles.Add(vertexIndex + 2);
        Triangles.Add(vertexIndex + 3);
        
        UpdateMesh();
        return vertexIndex;
    }
    
    public void AddCollider()
    {
        meshCollider = gameObject.AddComponent<MeshCollider>();
    }
    
    protected void ApplyMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
        UpdateMesh();
    }
}
