using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class Cube : MonoBehaviour
{
    public int xSize, ySize, zSize;

    private Mesh mesh;
    private Vector3[] vertices;

    private void Awake () 
    {
        StartCoroutine(Generate());
    }
    
    private static int SetQuad (int[] triangles, int i, int v00, int v10, int v01, int v11) 
    {
        triangles[i] = v00;
        triangles[i + 1] = triangles[i + 4] = v01;
        triangles[i + 2] = triangles[i + 3] = v10;
        triangles[i + 5] = v11;
        return i + 6;
    }
    
    private void CreateVertices () 
    {
        var cornerVertices = 8;
        var edgeVertices = (xSize + ySize + zSize - 3) * 4;
        var faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        
        mesh.vertices = vertices;
    }

    
    
    private void CreateTriangles () 
    {
        int quads = (xSize * ySize + xSize * zSize + ySize * zSize) * 2;
        int[] triangles = new int[quads * 6];
        mesh.triangles = triangles;
    }
    

    private IEnumerator Generate () 
    {
        GetComponent<MeshFilter>().mesh = mesh = new Mesh();
        mesh.name = "Procedural Cube";
        WaitForSeconds wait = new WaitForSeconds(0.05f);
        
        CreateVertices();
        CreateTriangles();
        
        var cornerVertices = 8;
        var edgeVertices = (xSize + ySize + zSize - 3) * 4;
        var faceVertices = (
            (xSize - 1) * (ySize - 1) +
            (xSize - 1) * (zSize - 1) +
            (ySize - 1) * (zSize - 1)) * 2;
        vertices = new Vector3[cornerVertices + edgeVertices + faceVertices];
        
        int v = 0;
        for (int y = 0; y <= ySize; y++) 
        {
            for (int x = 0; x <= xSize; x++) 
            {
                vertices[v++] = new Vector3(x, y, 0);
                yield return wait;
            }
            for (int z = 1; z <= zSize; z++) 
            {
                vertices[v++] = new Vector3(xSize, y, z);
                yield return wait;
            }
            for (int x = xSize - 1; x >= 0; x--) 
            {
                vertices[v++] = new Vector3(x, y, zSize);
                yield return wait;
            }
            for (int z = zSize - 1; z > 0; z--) 
            {
                vertices[v++] = new Vector3(0, y, z);
                yield return wait;
            }
        }
        
        for (int z = 1; z < zSize; z++) 
        {
            for (int x = 1; x < xSize; x++) 
            {
                vertices[v++] = new Vector3(x, ySize, z);
                yield return wait;
            }
        }
        for (int z = 1; z < zSize; z++) 
        {
            for (int x = 1; x < xSize; x++) 
            {
                vertices[v++] = new Vector3(x, 0, z);
                yield return wait;
            }
        }

        yield return wait;
    }

    private void OnDrawGizmos () 
    {
        if (vertices == null) 
        {
            return;
        }
        Gizmos.color = Color.black;
        
        for (int i = 0; i < vertices.Length; i++) 
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}
