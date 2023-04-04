using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeshGridTest : MonoBehaviour
{
    [SerializeField, Range(0, 30)] private int numberX;
    [SerializeField, Range(0, 30)] private int numberZ;
    private List<Vector3> dots = new();
    [Range(0, 12)] public int testNum;

    protected Mesh TheMesh;
    MeshCollider meshCollider;
    [NonSerialized] protected List<Vector3> Vertices = new ();
    [NonSerialized] protected List<int> Triangles = new ();

    public Material aMaterial;

    private void Awake()
    {
        GetComponent<MeshFilter>().mesh = TheMesh = new Mesh();
        TheMesh.name = "TheMesh";
        ApplyMaterial(aMaterial);
        MakeGrid();
    }

    void Start()
    {
        
    }
    
    public void ApplyMaterial(Material material)
    {
        GetComponent<MeshRenderer>().material = material;
        UpdateMesh();
    }
    
    protected void UpdateMesh()
    {
        TheMesh.Clear();
        TheMesh.SetVertices(Vertices);
        TheMesh.SetTriangles(Triangles, 0);
        TheMesh.RecalculateNormals();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    private Vector3 Find(int x, int z)
    {
        return Vertices[z * numberX + x];
    }


    private void MakeGrid()
    {
        var vertexIndex = Vertices.Count;
        var lineCount = Vertices.Count;
        
        var pos = transform.position;
        Vertices.Clear();
        Triangles.Clear();
        
        for (int i = 0; i < numberZ; i++)
        {
            for (int j = 0; j < numberX; j++)
            {
                Vertices.Add(pos + new Vector3(100 * j,0,0));

                Debug.Log($"index {i} other {lineCount}");
                if(j == numberX - 1) continue;
                Triangles.Add(vertexIndex + j);
                Triangles.Add(vertexIndex + j + numberX);
                Triangles.Add(vertexIndex + j + numberX + 1);
            }

            lineCount += numberX;
            pos += new Vector3(0, 0, 100);
        }
        //Triangles.Add(0);
        //Triangles.Add( 4);
        //Triangles.Add( 5);
        //
        //Triangles.Add(4);
        //Triangles.Add( 8);
        //Triangles.Add( 9);
        //
        //Triangles.Add(8);
        //Triangles.Add( 12);
        //Triangles.Add( 13);
        //
        //Triangles.Add(10);
        //Triangles.Add( 14);
        //Triangles.Add( 15);

        Debug.Log($"index {vertexIndex}, num vertex {Vertices.Count}");

        UpdateMesh();
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        dots.Clear();
    
        var total = numberX * numberZ;
        
        for (int i = 0; i < numberZ; i++)
        {
            for (int j = 0; j < numberX; j++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(pos + new Vector3(100 * j,0,0), 3);
                dots.Add(pos + new Vector3(100 * j,0,0));
            }
            pos += new Vector3(0, 0, 100);
        }
        
        dots = dots.OrderBy(x => x.x).ThenBy(x => x.z).ToList();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(dots[0], dots[0] + new Vector3(0, 100,0));
     
        Gizmos.color = Color.red;
        Gizmos.DrawLine(dots[^1], dots[^1] + new Vector3(0, 100,0));

    }
}
