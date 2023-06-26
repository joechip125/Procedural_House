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
        BuildBox();
    }

    private void AddSomeNew(Vector3 normalDir, Vector3 start)
    {
        var aSize = new Vector2(10,10);
        var aCrossForward = Vector3.Cross(normalDir, Vector3.up).normalized;
        var aCrossUp = Quaternion.AngleAxis(0, normalDir) *aCrossForward;
        var sides = Mathf.Sqrt(Mathf.Pow(aSize.x, 2) + Mathf.Pow(aSize.y, 2));

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp3 = Quaternion.AngleAxis(i * 90 + 45, normalDir) *aCrossForward;
            SimplePanel(start + aCrossUp3 * sides, normalDir, aSize);
        }
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

    private void SetDirections(Vector3 aNormal)
    {
        
    }
    
    private void BuildBox()
    {
        var aCrossForward = Vector3.Cross(wallNormal, Vector3.up).normalized;
        var addDeg = 45f;
        var adder = 0;
        var pos = transform.position + wallNormal * 200 + Vector3.up * 50;
        Positions = new Vector3[8];
        Vector2 size = new Vector2(100, 50);
        
        for (int i = 0; i < 4; i++)
        {
            var aCrossUp = Quaternion.AngleAxis(addDeg +(90 * i), wallNormal) *aCrossForward;
            var aCrossUp2 = Quaternion.AngleAxis(addDeg +(90 * i), wallNormal) *aCrossForward;
            var newPos = pos + aCrossUp * 50;
            Positions[adder] = newPos;
            Positions[adder + 1] = newPos + wallNormal * 10;
            adder += 2;
            var newPos2 = aCrossUp2 * size.x / 2;
            var newPos3 = aCrossUp2 * size.y / 2;
            
            if (i % 2 == 0)
            {
                Directions[i] = newPos2;
            }
            else
            {
               
                Directions[i] = newPos3;
            }
        }
  
        adder = 0;
        
        for (int i = 0; i < Positions.Length / 2; i++)
        {
            if (i == Positions.Length / 2 - 1)
            {
                AddQuad(Positions[adder], Positions[adder + 1], Positions[1], Positions[0]);    
            }
            else
            {
                AddQuad(Positions[adder], Positions[adder + 1], Positions[adder + 3], Positions[adder + 2]);
            }
            adder += 2;
        }
    }

    private void OnDrawGizmos()
    {
        var aCrossForward = Vector3.Cross(wallNormal, Vector3.up).normalized;
        var theRed = 0f;
        var addDeg = 45f;
        Positions = new Vector3[8];
        Directions = new Vector3[4];
        var adder = 0;
        Vector2 size = new Vector2(100, 50);
        var pos = transform.position;
        var wallThick = 10f;

        for (int i = 0; i < 4; i++)
        {
            Gizmos.color = new Color(theRed, 0, 0);
            
            var aCrossUp = Quaternion.AngleAxis(addDeg +(90 * i), wallNormal) *aCrossForward;
            var aCrossUp2 = Quaternion.AngleAxis(90+(90 * i), wallNormal) *aCrossForward;
            var aCrossUp3 = Quaternion.AngleAxis( 180+(90 * i), wallNormal) *aCrossForward;
            var newPos = pos + aCrossUp * 50;
            var newPos2 = aCrossUp2 * size.x / 2;
            var newPos3 = aCrossUp2 * size.y / 2;
            Positions[adder] = newPos;
            Positions[adder + 1] = newPos + wallNormal * 10;

            if (i % 2 == 0)
            {
                Gizmos.DrawSphere(pos + newPos3, 1.5f);
                Directions[i] = newPos3;
            }
            else
            {
                Gizmos.DrawSphere(pos + newPos2, 1.5f);
                Directions[i] = newPos2;
            }

            Gizmos.DrawSphere(newPos, 1.5f);
            Gizmos.DrawSphere(newPos + wallNormal * 10, 1.5f);
            theRed += 0.25f;
            adder += 2;
            Debug.Log($"{aCrossUp2} across2 ");
        }

        adder = 0;
        theRed = 0f;
        
        for (int i = 0; i < 4; i++)
        {
            Gizmos.color = new Color(theRed, 0, 0);
            if (i != 3)
            {
                Gizmos.DrawSphere(Directions[i]+ Directions[i + 1], 1.5f);
                Gizmos.DrawSphere(Directions[i]+ Directions[i + 1] + wallNormal * -wallThick, 1.5f);
            }
            else
            {
                Gizmos.DrawSphere(Directions[i]+ Directions[0], 1.5f);
                Gizmos.DrawSphere(Directions[i]+ Directions[0] + wallNormal * -wallThick, 1.5f);
            }
            theRed += 0.25f;
        }


        var last = 0;
        for (int i = 0; i < Positions.Length / 2; i++)
        {
            if (i == Positions.Length / 2 - 1)
            {
                Gizmos.DrawLine(Positions[last], Positions[last + 1]);
                Gizmos.DrawLine(Positions[last + 1], Positions[1]);
                Gizmos.DrawLine(Positions[1], Positions[0]);
            }
            else
            {
                Gizmos.DrawLine(Positions[adder], Positions[adder + 1]);
                Gizmos.DrawLine(Positions[adder + 1], Positions[adder + 3]);
                Gizmos.DrawLine(Positions[adder + 3], Positions[adder + 2]);
                last = adder + 2;
            }
            adder += 2;
        }
        
    }
}
