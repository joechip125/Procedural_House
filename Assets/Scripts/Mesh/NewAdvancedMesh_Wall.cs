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

    private int lastVert;

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
       //AddDoor(transform.position);
       AddSomething();
    }


    private void AddDoor(Vector3 start)
    {
        var place = start + new Vector3(100, 0, 10);
        MakeWall(Vector3.Cross(direction, Vector3.up),Vector3.up, start, new Vector2(100, 10));
        MakeWall(direction,Vector3.Cross(direction, Vector3.up), start, new Vector2(10, 100));
       
        MakeWall(Vector3.Cross(-direction, Vector3.up),Vector3.up, place, new Vector2(100, 10));
     
    }

    private void AddSomething()
    {
        var place = new Vector3(50,50,0);
        var size = new Vector2(10, 100);
        
        var size2 = new Vector3(100, 100,10);
        var normalDir = new Vector3(0,0,1);
        Vector3 aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp = Quaternion.AngleAxis((90 * i), normalDir) *aCrossForward;
            var aPlace = place + Vector3.Scale(size2 / 2, aCrossUp);

            SimplePanel(aPlace, -aCrossUp, size);
        }
        
        SimplePanel(new Vector3(50,125,-5), new Vector3(0,0,-1), new Vector2(100,50));
        SimplePanel(new Vector3(50,125,5), new Vector3(0,0,1), new Vector2(100,50));
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
        }
    }
    
    private void MakeWall(Vector3 upDir, Vector3 rightDir, Vector3 position, Vector2 size)
    {
        var pos2 = position + rightDir * size.x;
        var pos3 = position + (rightDir * size.x) + (upDir * size.y);
        var pos4 = position + upDir * size.y;

        AddQuad(position, pos2, pos3, pos4);
    }

    private void BasicWall(int numTiles, Vector3 dir, Vector2 size)
    {
        for (int i = 0; i < numTiles; i++)
        {
            
        }
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
    
    private void SimplePanel(Vector3 addPos, Vector3 normalDir, Vector2 theSize, int addDegree = 0)
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
            
            corners[i] = poss + addPos;
            
            flip = !flip;
        }
        
        lastVert = AddQuad(corners[0], corners[1], corners[2], corners[3]);
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
