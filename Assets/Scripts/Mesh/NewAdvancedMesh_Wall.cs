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
    [SerializeField]private Vector3 wallSize;
    [SerializeField]private Vector3 wallNormal;
   
    public List<WallInfo> wallInfos = new();

    private int lastVert;

    public Material aMaterial;
    
    private void Awake()
    {
        InitMesh();
        ApplyMaterial(aMaterial);
        //TunnelVerts(Vector3.zero, direction, wallSize);
        SideVerts(Vector3.zero, direction, wallSize);
    }
    
    
    private void AddDoor(Vector3 pos, Vector3 size, Vector3 normalDir)
    {
        TunnelVerts(pos, normalDir, size);
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
        
        if(TempVectors.Length != 4) TempVectors = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(90+(90 * i), aNormal) *aCrossForward;
            var aCrossUp3 = Quaternion.AngleAxis( 180+(90 * i), aNormal) *aCrossForward;
            
            if (i % 2 == 0)
            {
                TempVectors[i] =  aCrossUp2 * size.y / 2 + aCrossUp3 * size.x / 2;
            }
            else
            {
                TempVectors[i] =  aCrossUp2 * size.x / 2 + aCrossUp3 * size.y / 2;
            }
            TempVectors[i] += addPos;
        }        
    }
    
    private void BuildBox(Vector3 wallDirection, Vector3 size, Vector3 addPos)
    {
        var frwAmount = wallDirection * size.z;
        var aCross = Vector3.Cross(Vector3.up, wallDirection);

        SetPositionsSquare(wallDirection, size, addPos);

        for (int i = 0; i < TempVectors.Length; i++)
        {
            if (i != TempVectors.Length - 1)
                AddQuad(TempVectors[i], TempVectors[i] + frwAmount, TempVectors[i + 1] + frwAmount, TempVectors[i + 1]);
            else
            {
                AddQuad(TempVectors[i], TempVectors[i] + frwAmount, TempVectors[0] + frwAmount, TempVectors[0]);
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
                AddQuad(TempVectors[3], TempVectors[2], TempVectors[1] ,TempVectors[0]);
                
                SetPositionsSquare(-wallDirection, newSize2, 
                    frwAmount + addPos + aCrossUp2 * ((size.y + newSize2.y) / 2));
                AddQuad(TempVectors[3], TempVectors[2], TempVectors[1] ,TempVectors[0]);
            }
            else
            {
                SetPositionsSquare(wallDirection, newSize, 
                    addPos + aCrossUp2 * ((size.x + newSize.x) / 2));
                AddQuad(TempVectors[3], TempVectors[2], TempVectors[1] ,TempVectors[0]);
                
                SetPositionsSquare(-wallDirection, newSize, 
                    frwAmount + addPos + aCrossUp2 * ((size.x + newSize.x) / 2));
                AddQuad(TempVectors[3], TempVectors[2], TempVectors[1] ,TempVectors[0]);
            }
        }
    }
    
    private void BuildOuter(Vector2 size, Vector3 wallDirection, Vector3 center, Vector2 innerSize)
    {
        var aSize = size - innerSize;
        var cross = Vector3.Cross(Vector3.up, wallDirection);
        var start = center - (cross * size.x + Vector3.up * size.y)/ 2;
        SetPositionsSquare(wallDirection, size, center);
        AddQuad(TempVectors[0], TempVectors[1], TempVectors[2], TempVectors[3]);

        for (int i = 0; i < 4; i++)
        {
            Debug.Log(TempVectors[i]);
        }
    }

    private void AddVerts(Vector3 center, Vector3 size, int numTiles)
    {
        var min = center - size;
        var max = center + size;
        
        var aDir2 = new Vector3(0, 1, 0);
        var singleO = new Vector2((max.x - min.x) / numTiles, (max.y - min.y)/ numTiles);
        var dir = new Vector3(0, 0, 1);
        
        for (int i = 0; i < 4; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(90 + (90 * i), dir) * -aDir2;
            for (int j = 0; j < numTiles; j++)
            {
                Vertices.Add(min);
                min += aCrossUp2 * singleO.y;
            }
        }
    }
    
    private void AddDots(Vector3 center, Vector3 size, int numTiles)
    {
        var min = center - size;
        var max = center + size;
        
        var aDir2 = new Vector3(0, 1, 0);
        var singleO = new Vector2((max.x - min.x) / numTiles, (max.y - min.y)/ numTiles);
        var dir = new Vector3(0, 0, 1);
        var aColor = 0f;

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(90 + (90 * i), dir) * -aDir2;
            for (int j = 0; j < numTiles; j++)
            {
                
                Gizmos.color = new Color(aColor, 0, 0);
                Gizmos.DrawSphere(min, 4);
                
                min += aCrossUp2 * singleO.y;
                aColor += 0.05f;
            }
        }
    }

    private void Tunnel(Vector3 center, Vector3 aDir, Vector3 size)
    {
        var aCrossForward = Vector3.Cross(aDir, Vector3.up).normalized;
        
        if(Directions.Length != 4) Directions = new Vector3[4];

        for (int i = 0; i < 4; i++)
        {
            var aCrossUp2 = Quaternion.AngleAxis(90+(90 * i), aDir) *aCrossForward;
            var aCrossUp3 = Quaternion.AngleAxis( 180+(90 * i), aDir) *aCrossForward;
            
            if (i % 2 == 0)
            {
                Directions[i] =  aCrossUp2 * size.y / 2 + aCrossUp3 * size.x / 2;
            }
            else
            {
                Directions[i] =  aCrossUp2 * size.x / 2 + aCrossUp3 * size.y / 2;
            }

            Directions[i] += center;
        }

        var count = 0;
        Gizmos.color = Color.red;
        
        for (int i = 0; i < 4; i++)
        {
            var aVec = Vector3.zero;
            for (int j = 0; j < 4; j++)
            {
                Gizmos.DrawSphere(Directions[i] + aVec, 1.5f);
                Handles.Label(Directions[i] + aVec, $"{count}");
                aVec += aDir * size.z;
                count++;
            }
        }
        
    }
    private void TunnelVerts(Vector3 center, Vector3 aDir, Vector3 size)
    {
        SetSquare(aDir, center, new Vector2(size.x, size.y));
        var add = Vertices.Count;

        foreach (var c in Corners)
        {
            Vertices.Add(c);
            Vertices.Add(c + aDir * size.z);
        }
        
        for (int i = 0; i < Corners.Count; i++)
        {
            if (i == Corners.Count - 1)
            {
                Triangles.Add(add);
                Triangles.Add(add + 1);
                Triangles.Add(1);
                
                Triangles.Add(1);
                Triangles.Add(0);
                Triangles.Add(add);
                continue;
            }
            Triangles.Add(add);
            Triangles.Add(add + 1);
            Triangles.Add(add + 2);
            
            Triangles.Add(add + 1);
            Triangles.Add(add + 3);
            Triangles.Add(add + 2);
            
            add+= 2;
        }
        UpdateMesh();
    }

    private void SideVerts(Vector3 pos, Vector3 dir, Vector3 innerSize)
    {
        var start = Vertices.Count;
        for (int i = 0; i < 2; i++)
        {
            SetSquare(dir, pos, innerSize);
            foreach (var t in Corners)
            {
                Vertices.Add(t);
            }
            innerSize += new Vector3(50, 50, 0);
        }

        for (int i = 0; i < 4; i++)
        {
            var current = start + i;

            if (i == 3)
            {
                Triangles.Add(current);
                Triangles.Add(current + 4);
                Triangles.Add(start + 4);      
                continue;
            }
            Triangles.Add(current);
            Triangles.Add(current + 4);
            Triangles.Add(current + 5);    
        }
        
        UpdateMesh();
    }

    private void AddCorner()
    {
        var aCrossForward = Vector3.Cross(Vector3.up, Vector3.right).normalized;
    }
    
    private void AddStrip(Vector3 start, Vector2 size, int numTiles)
    {
        var dir = new Vector3(1, 0, 0);
        var aDir2 = new Vector3(0, 1, 0);
        var amount = -20;
        var start2 = -60;
        var toggle = true;
        var min = 0.4f;
        var max = 1f;
        var sin =Mathf.Cos((Mathf.PI / 180) * start2);
        
        for (int i = 0; i <= numTiles; i++)
        {
            if (sin <= min && toggle)
            {
                amount = 20;
                toggle = false;
            }
            
            if (sin >= max && !toggle)
            {
                amount = -20;
                toggle = true;
            }

            start2 += amount;
            
            sin =Mathf.Cos((Mathf.PI / 180) * start2);
            
            Gizmos.DrawSphere(start, 2);
            Gizmos.DrawSphere(start + Vector3.up * (size.y * sin), 2);
            start += dir * size.x;
        }
    }

    private void AddStripsSin(Vector3 start, Vector2 size, int numTiles)
    {
        var dir = new Vector3(1, 0, 0);
        var amount = -20;
        var start2 = -60;
        var toggle = true;
        var min = 0.4f;
        var max = 1f;
        var sin =Mathf.Cos((Mathf.PI / 180) * start2);
        
        for (int i = 0; i <= numTiles; i++)
        {
            if (sin <= min && toggle)
            {
                amount = 20;
                toggle = false;
            }
            
            if (sin >= max && !toggle)
            {
                amount = -20;
                toggle = true;
            }

            start2 += amount;
            sin =Mathf.Cos((Mathf.PI / 180) * start2);
            
            Vertices.Add(start);
            Vertices.Add(start + Vector3.up * (size.y * sin));
            
            start += dir * size.x;
        }
        
        var add = 0;
        for (int i = 0; i < numTiles; i++)
        {
            Triangles.Add(add);
            Triangles.Add(add + 1);
            Triangles.Add(add + 2);
            
            Triangles.Add(add + 1);
            Triangles.Add(add + 3);
            Triangles.Add(add + 2);
            add += 2;
        }
        
        UpdateMesh();
    }
    
    private void AddStripVerts(Vector3 start, Vector2 size, int numTiles)
    {
        var dir = new Vector3(1, 0, 0);
        
        for (int i = 0; i <= numTiles; i++)
        {
            Vertices.Add(start);
            Vertices.Add(start + Vector3.up * size.y);
            start += dir * size.x;
        }

        var add = 0;
        for (int i = 0; i < numTiles; i++)
        {
            Triangles.Add(add);
            Triangles.Add(add + 1);
            Triangles.Add(add + 2);
            
            Triangles.Add(add + 1);
            Triangles.Add(add + 3);
            Triangles.Add(add + 2);
            add += 2;
        }
        
        UpdateMesh();
    }
    
    private void InnerOuter(Vector3 innerS, Vector3 outerS, Vector3 pos)
    {
        AddVerts(pos, outerS, 4);
        AddVerts(pos, innerS, 2);

        if (!Application.isPlaying) return;

        for (int i = 0; i < 3; i++)
        {
            Triangles.Add(i);
            Triangles.Add(i + 16);
            Triangles.Add(i + 1);
            
            if(i < 1) continue;
            Triangles.Add(i);
            Triangles.Add(i + 15);
            Triangles.Add(i + 16);
            
        }
        Triangles.Add(3);
        Triangles.Add(5);
        Triangles.Add(4);
        
        UpdateMesh();
    }
    
    private void SetDirections()
    {
        
    }

    private void AddVertSquare()
    {
        
    }
    
    private void SetPoints(Color pointColor,int num)
    {
        Gizmos.color = pointColor;
        foreach (var c in Corners)
        {
            Gizmos.DrawSphere(c, 4);
            Handles.Label(c, $"{num++}");
        }
    }
    
    private void SetPoints2(Color pointColor,int num)
    {
        Gizmos.color = pointColor;
        
        for (int i = 0; i < Corners.Count; i++)
        {
            Gizmos.DrawSphere(Corners[i], 4);
            Handles.Label(Corners[i], $"{num++}");
            Gizmos.DrawSphere(Corners[i] + direction * wallSize.z, 4);
            Handles.Label(Corners[i]+ direction * wallSize.z, $"{num++}");
        }
    }
    
    private void OnDrawGizmos()
    {
        var pos = transform.position;
        SetSquare(direction, pos, wallSize);
        var count = 0;
        SetPoints(Color.green, count);
        count += 4;
        SetSquare(direction, pos, wallSize + new Vector3(50,50,0));
        SetPoints(Color.red, count);

        for (int i = 0; i < Corners.Count; i++)
        {
            //Gizmos.DrawSphere(Corners[i], 4);
            //Handles.Label(Corners[i], $"{count++}");
            //Gizmos.DrawSphere(Corners[i] + direction * wallSize.z, 4);
            //Handles.Label(Corners[i]+ direction * wallSize.z, $"{count++}");
        }
    }
}
