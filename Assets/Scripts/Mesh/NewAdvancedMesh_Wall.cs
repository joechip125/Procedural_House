using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public enum WallTypes
{
    Blank,
    Door,
    Window
}

[Serializable]
public class WallInfo
{
    public WallTypes type;
}

public class NewAdvancedMesh_Wall : NewAdvancedMesh
{

    [SerializeField] private Vector3 direction;
    [SerializeField] private float length;
    [SerializeField] private float height;
    [SerializeField] private int numberTiles;

    private List<WallInfo> wallInfos = new();

    public Material aMaterial;
    
    private readonly Vector3[] corners = new[]
    {   new Vector3(-1,0,-1), 
        new Vector3(0, 0, 1),
        new Vector3(1, 0, 1), 
        new Vector3(1, 0, 0)};
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
       // Activate();
       AddDoor(transform.position);
    }


    private void AddDoor(Vector3 start)
    {
        MakeWall(Vector3.Cross(direction, Vector3.up),Vector3.up, start, new Vector2(100, 10));
        MakeWall(direction,Vector3.Cross(direction, Vector3.up), start, new Vector2(10, 100));
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


    private void MakeWalls()
    {
        var directions = new Vector3(1,0,0);
        for (int i = 0; i < 5; i++)
        {
            directions += new Vector3(0, 0, 0.1f);
        }
    }
    
    private void MakeWall(Vector3 upDir, Vector3 position, float size, int segments)
    {
        var singleS = size / segments;
        
        for (int i = 0; i < segments; i++)
        {
            MakeWall(upDir, position, singleS);
            position += upDir * singleS;
           // aDirection += new Vector3(0, 0, 0.1f);
        }
    }
    
    private void MakeWall(Vector3 upDir, Vector3 rightDir, Vector3 position, Vector2 size)
    {
        var pos2 = position + rightDir * size.x;
        var pos3 = position + (rightDir * size.x) + (upDir * size.y);
        var pos4 = position + upDir * size.y;

        AddQuad(position, pos2, pos3, pos4);
    }
    
    
    private void MakeWall(Vector3 upDir, Vector3 position, float size)
    {
        var pos2 = position + new Vector3(0, height,0);
        var pos3 = position + new Vector3(0, height,0) + upDir * size;
        var pos4 = position + upDir * size;

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
        MakeWall(direction, transform.position,length, 5);
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
