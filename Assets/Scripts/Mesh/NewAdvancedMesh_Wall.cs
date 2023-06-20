using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

        var theSize = new Vector2(100, 50);
        SimplePanel(new Vector3(size2.x / 2,size2.y + theSize.y / 2,-size2.z / 2), -normalDir, theSize);
        SimplePanel(new Vector3(size2.x / 2,size2.y + theSize.y / 2,+size2.z / 2), normalDir, theSize);
    }

    private void RebuildWall()
    {
        ClearMesh();
        BuildWall();
    }
    
    public void BuildWall()
    {
        var wallNormal = new Vector3(0, 0, 1);
        var wallRight = Vector3.Cross(Vector3.up, wallNormal) +  Vector3.up;
        var panelSize = new Vector2(400, 100);

        if (wallInfos.Count(x => x.type != WallTypes.Blank) == 0)
        {
            var aPos = Vector3.Scale(wallRight, new Vector3(panelSize.x, panelSize.y, panelSize.x)) / 2;
            SimplePanel(aPos, -wallNormal, panelSize);
            SimplePanel(aPos + wallNormal * 5, wallNormal, panelSize);
        }

        for (int i = 0; i < wallInfos.Count(); i++)
        {
            
            switch (wallInfos[i].type)
            {
                case WallTypes.Blank:
                    break;
                case WallTypes.Door:
                    break;
            }
        }
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

    private void MakeWall(Vector3 upDir, Vector3 position, float size)
    {
        var pos2 = position + new Vector3(0, height,0);
        var pos3 = position + new Vector3(0, height,0) + upDir * size;
        var pos4 = position + upDir * size;

        AddQuad(position, pos2, pos3, pos4);
    }

    public void BasicWall(int numTiles, Vector3 dir, Vector3 size, Vector3 pos)
    {
        for (int i = 0; i < numTiles; i++)
        {
            var pos2 = pos + Vector3.up * size.y;
            var pos3 = pos + (dir * size.x) + (Vector3.up * size.y);
            var pos4 = pos + dir * size.x;
            AddQuad(pos, pos2, pos3, pos4);
            pos += Vector3.Scale(size, dir);
        }
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
