﻿using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class ProceduralMesh : MonoBehaviour 
{
    
    static MeshJobScheduleDelegate[] jobs = 
    {
        MeshJob<SquareGrid, SingleStream>.ScheduleParallel,
        MeshJob<SharedSquareGrid, SingleStream>.ScheduleParallel
    };

    public enum MeshType 
    {
        SquareGrid, SharedSquareGrid
    };

    [SerializeField]
    MeshType meshType;
    [SerializeField, Range(1, 10)]
    int resolution = 1;

    Mesh mesh;

    void Awake () 
    {
        mesh = new Mesh 
        {
            name = "Procedural Mesh"
        };
        GetComponent<MeshFilter>().mesh = mesh;
    }

    void OnValidate () => enabled = true;

    void Update () 
    {
        GenerateMesh();
        enabled = false;
    }

    void GenerateMesh () 
    {
        Mesh.MeshDataArray meshDataArray = Mesh.AllocateWritableMeshData(1);
        Mesh.MeshData meshData = meshDataArray[0];
        
        jobs[(int)meshType](mesh, meshData, resolution, default).Complete();
        //MeshJob<SquareGrid, SingleStream>.ScheduleParallel(mesh, meshData, resolution, default).Complete();
        Mesh.ApplyAndDisposeWritableMeshData(meshDataArray, mesh);
    }
}