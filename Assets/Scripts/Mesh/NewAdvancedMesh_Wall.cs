using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{

    [SerializeField] private Vector3 direction;
    [SerializeField] private float length;
    [SerializeField] private float height;
    [SerializeField] private int numberTiles;
    
    public Material aMaterial;
    
    private readonly Vector3[] corners = new[]
    {   new Vector3(-1,0,-1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0)};
    
    private void Awake()
    {
        InitMesh();
        Activate();
    }


    private IEnumerator Delay()
    {
        yield return new WaitForSeconds(5);
        ClearMesh();
    }

    private void NewWall(Vector3 aDirection)
    {
        var aSize = 100;
        var pos = transform.position;
        MakeWall(aDirection ,pos, aSize);

        pos += direction * (aSize + 40);
        
        MakeWall(aDirection ,pos, aSize);
    }
    
    private void MakeWall(Vector3 aDirection, Vector3 position, float size)
    {
        var pos2 = position + new Vector3(0, height,0);
        var pos3 = position + new Vector3(0, height,0) + aDirection * size;
        var pos4 = position + aDirection * size;

        AddQuad(position, pos2, pos3, pos4);
    }

    protected override void Activate()
    {
        base.Activate();
        ApplyMaterial(aMaterial);
        
        var pos = transform.position;
        var single = length / numberTiles;
       
        for (int i = 0; i < numberTiles; i++)
        {
            pos += direction.normalized * single;
        }
        NewWall(direction);
    }


    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var single = length / numberTiles;
        Gizmos.DrawSphere(pos, 3f);
        Gizmos.DrawSphere(pos + new Vector3(0, height,0), 3f);
        pos += direction.normalized * single;

        for (int i = 0; i < numberTiles; i++)
        {
            Gizmos.DrawSphere(pos, 3f);
            Gizmos.DrawSphere(pos + new Vector3(0, height,0), 3f);
            pos += direction.normalized * single;
        }
    }
}
