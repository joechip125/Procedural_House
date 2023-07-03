using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
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
    [SerializeField]private Vector2 wallSize;
    [SerializeField]private Vector3 wallNormal;

    public List<WallInfo> wallInfos = new();

    private int lastVert;

    public Material aMaterial;
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        BuildBox(new Vector3(1,0,0), new Vector3(100,50, 12), Vector3.zero);
    }
    
    
    private void AddDoor(Vector3 pos, Vector3 size, Vector3 normalDir)
    {
        var aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        pos -= normalDir * size.z / 2;
        var maxHeight = 100f;
        
        
        for (int i = 0; i < 4; i++)
        {
            var panelSize = i % 2 == 0 ? new Vector2(size.z, size.x) : new Vector2(size.z, size.y);

            var aCrossUp = Quaternion.AngleAxis((90 * i), normalDir) *aCrossForward;
            var aPlace = pos + Vector3.Scale(size / 2, aCrossUp);

            SimplePanel(aPlace, -aCrossUp, panelSize);
        }

        if (size.y >= maxHeight) return;
        
        var theSize = new Vector2(size.x, maxHeight - size.y);
        var addUp = Vector3.up * (size.y / 2 + theSize.y / 2);
        SimplePanel( pos + addUp + normalDir * (-size.z / 2), -normalDir, theSize);
        SimplePanel(pos + addUp +normalDir * (size.z / 2), normalDir, theSize);
    }

    public void AddDoor(int place)
    {
        wallInfos[place].type = WallTypes.Door;
        BuildWall();
    }

    public void InitWall(Vector3 normal, Vector2 size, int numTiles)
    {
        wallNormal = normal;
        wallSize = size;
        for (int i = 0; i < numTiles; i++)
        {
            wallInfos.Add(new WallInfo(){type = WallTypes.Blank});
        }
        BuildWall();
    }
    
    private void BuildWall()
    {
        ClearMesh();
        var wallRight = Vector3.Cross(Vector3.up, wallNormal);
        var numTiles = wallInfos.Count();

        var singlePanel = new Vector2(wallSize.x / numTiles, wallSize.y);
        var start = wallRight * singlePanel.x / 2 + (Vector3.up * wallSize.y) / 2;
        
        var panelSize2 = new Vector3(singlePanel.x, singlePanel.y, 10);
      
        for (int i = 0; i < numTiles; i++)
        {
            switch (wallInfos[i].type)
            {
                case WallTypes.Blank:
                    SimplePanel(start, wallNormal, singlePanel);
                    break;
                case WallTypes.Door:
                    AddDoor(start, panelSize2, wallNormal);
                    break;
            }
            
            start += wallRight * singlePanel.x;
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
    }

    private void SetPositionsSquare(Vector3 aNormal, Vector2 size, Vector3 addPos)
    {
        var aCrossForward = Vector3.Cross(aNormal, Vector3.up).normalized;
        
        if(Positions.Length != 4) Positions = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(90+(90 * i), aNormal) *aCrossForward;
            var aCrossUp3 = Quaternion.AngleAxis( 180+(90 * i), aNormal) *aCrossForward;
            
            if (i % 2 == 0)
            {
                Positions[i] =  aCrossUp2 * size.y / 2 + aCrossUp3 * size.x / 2;
            }
            else
            {
                Positions[i] =  aCrossUp2 * size.x / 2 + aCrossUp3 * size.y / 2;
            }
            Positions[i] += addPos;
        }        
    }
    
    private void BuildBox(Vector3 wallDirection, Vector3 size, Vector3 addPos)
    {
        var frwAmount = wallDirection * size.z;
        var aCross = Vector3.Cross(Vector3.up, wallDirection);

        SetPositionsSquare(wallDirection, size, addPos);

        for (int i = 0; i < Positions.Length; i++)
        {
            if (i != Positions.Length - 1)
                AddQuad(Positions[i], Positions[i] + frwAmount, Positions[i + 1] + frwAmount, Positions[i + 1]);
            else
            {
                AddQuad(Positions[i], Positions[i] + frwAmount, Positions[0] + frwAmount, Positions[0]);
            }
        }

        var newSize = new Vector2(20, size.y * 2);
        var newSize2 = new Vector2(size.x, size.y / 2);
        
        for (int i = 0; i < 4; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(90+(90 * i), wallDirection) *aCross;
            if (i % 2 == 0)
            {
                SetPositionsSquare(wallDirection, newSize2, 
                    addPos + aCrossUp2 * ((size.y + newSize2.y) / 2));
                AddQuad(Positions[3], Positions[2], Positions[1] ,Positions[0]);
                
                SetPositionsSquare(-wallDirection, newSize2, 
                    frwAmount + addPos + aCrossUp2 * ((size.y + newSize2.y) / 2));
                AddQuad(Positions[3], Positions[2], Positions[1] ,Positions[0]);
            }
            else
            {
                SetPositionsSquare(wallDirection, newSize, 
                    addPos + aCrossUp2 * ((size.x + newSize.x) / 2));
                AddQuad(Positions[3], Positions[2], Positions[1] ,Positions[0]);
                
                SetPositionsSquare(-wallDirection, newSize, 
                    frwAmount + addPos + aCrossUp2 * ((size.x + newSize.x) / 2));
                AddQuad(Positions[3], Positions[2], Positions[1] ,Positions[0]);
            }
        }
    }

    private void BuildOuter(Vector2 size, Vector3 wallDirection, Vector3 center, Vector2 innerSize)
    {
        var aSize = size - innerSize;
        var cross = Vector3.Cross(Vector3.up, wallDirection);
        var start = center - (cross * size.x + Vector3.up * size.y)/ 2;
        SetPositionsSquare(wallDirection, size, center);
        AddQuad(Positions[0], Positions[1], Positions[2], Positions[3]);

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(Positions[i]);
        }
    }

    private void InnerOuter(Vector3 innerS, Vector3 outerS, Vector3 pos)
    {
        var min = pos - outerS;
        var max = pos + outerS;
    }
    
    private void SetDirections()
    {
        
    }
    
    private void OnDrawGizmos()
    {
        var aCrossForward = Vector3.Cross(wallNormal, Vector3.up).normalized;
        var theRed = 0f;
        Directions = new Vector3[4];
        var size = new Vector3(100, 50, 10);
        var pos = transform.position;

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(90+(90 * i), wallNormal) *aCrossForward;
            var aCrossUp3 = Quaternion.AngleAxis( 180+(90 * i), wallNormal) *aCrossForward;
            
            if (i % 2 == 0)
            {
                Directions[i] =  aCrossUp2 * size.y / 2 + aCrossUp3 * size.x / 2;
            }
            else
            {
                Directions[i] =  aCrossUp2 * size.x / 2 + aCrossUp3 * size.y / 2;
            }

            Directions[i] += pos;
        }
        
        for (int i = 0; i < 4; i++)
        {
            var otherVec = wallNormal * size.z;
            Gizmos.color = new Color(theRed, 0, 0);
            Gizmos.DrawSphere(Directions[i], 1.5f);
            Gizmos.DrawSphere(Directions[i] + otherVec, 1.5f);
            theRed += 0.25f;
        }
    }
}
