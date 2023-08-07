using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

public class SingleRoom : MonoBehaviour
{
    [HideInInspector]public List<NewAdvancedMesh> meshes = new();
    [SerializeField] private List<GameObject> spawnable;
    private Vector3 theSize;

    private List<TileInfo> infos = new();
    private List<Vector3> corners = new();
    private Vector3[] tempVectors;
    [SerializeField] private float adjust = 0;
    [SerializeField, Range(0, 1000)] private float sizeX;
    [SerializeField, Range(0, 1000)] private float sizeY;
    
    private void Awake()
    {
        InitRoom();
        AddDoor(3);
    }
    
    private void InitRoom()
    {
        var center = transform.position;
        SetSquare(center, new Vector2(sizeX, sizeY));
        
        meshes.Add(Instantiate(spawnable[0], transform).GetComponent<NewAdvancedMesh>());
        var temp = (NewAdvancedMesh_Floor)meshes[^1];
        temp.SetValuesAndActivate(sizeX, sizeY);

        for (int i = 0; i < corners.Count; i++)
        {
            meshes.Add(Instantiate(spawnable[1], corners[i], Quaternion.identity, transform).GetComponent<NewAdvancedMesh>());
            var temp1 = (NewAdvancedMesh_Wall)meshes[^1];
            var angle = Quaternion.AngleAxis(180+ 90 * i, Vector3.up) *Vector3.right;
            temp1.InitWall(angle, i % 2 == 0 ? new Vector2(sizeY, 100) : new Vector2(sizeX, 100), 4);
        }
    }

    private void SetSquare(Vector3 pos, Vector2 size, float degreeAdjust = 0)
    {
        tempVectors = new Vector3[4];
        if (tempVectors.Length != 4)
        {
            tempVectors = new Vector3[4];
        }
        for (int i = 0; i < tempVectors.Length; i++)
        {
            tempVectors[i] = Quaternion.AngleAxis(degreeAdjust + 90 * i, Vector3.up) *Vector3.right;
        }
        
        corners.Clear();
        var tempPos = new Vector3();

        size /= 2;
        for (int i = 0; i < tempVectors.Length; i++)
        {
            if (i == tempVectors.Length - 1) tempPos = pos + tempVectors[0] * size.x + tempVectors[^1] * size.y;
            else
            {
                if (i % 2 == 0) tempPos = pos + tempVectors[i] * size.x + tempVectors[i + 1] * size.y;
                else tempPos = pos + tempVectors[i + 1] * size.x + tempVectors[i] * size.y;
            }
            corners.Add(tempPos);
        }
    }

    private void AddDoor(int number)
    {
        
    }
    
    private void AddWalls(int numWalls)
    {
        var add = 45;
        var pos = transform.position;
        var distance = Mathf.Sqrt(Mathf.Pow(200, 2) + Mathf.Pow(200, 2));
        var wallSize = new Vector2(400, 100);
        for (int i = 0; i < 4; i++)
        {
            var aCrossUp = Quaternion.AngleAxis(add +  90 *i, Vector3.up) *Vector3.right;
            var aCrossUp2 = Quaternion.AngleAxis(add + 135 +  90 *i, Vector3.up) *Vector3.right;
            var next = pos + aCrossUp * distance;
            meshes.Add(Instantiate(spawnable[1], next, Quaternion.identity, transform)
                .GetComponent<NewAdvancedMesh>());
            var temp = (NewAdvancedMesh_Wall)meshes[^1];
            temp.InitWall(aCrossUp2, wallSize, numWalls);
        }
        
    }

    private void AddStairs()
    {
        meshes.Add(Instantiate(spawnable[2], new Vector3(), Quaternion.identity, transform)
            .GetComponent<NewAdvancedMesh>());
        var temp = (NewAdvancedMesh_Staircase)meshes[^1];
        temp.MakeStairs(new Vector3(1,0,0), 5);
    }


    private void OnDrawGizmos()
    {
      //  if (Application.isPlaying) return;
        var temp = new Vector3[4];
        for (int i = 0; i < temp.Length; i++)
        {
            temp[i] = Quaternion.AngleAxis( adjust + 90 * i, Vector3.up) *Vector3.right;
        }
        
        var pos = transform.position;
        var tempDir = new Vector3();
        
        for (int i = 0; i < temp.Length; i++)
        {
            Gizmos.color = Color.red;
            if (i == temp.Length - 1) tempDir = pos + temp[0] * sizeX + temp[^1] * sizeY;
            
            else
            {
                if (i % 2 == 0) tempDir = pos + temp[i] * sizeX + temp[i + 1] * sizeY;
                else tempDir = pos + temp[i + 1] * sizeX + temp[i] * sizeY;
            }
            Gizmos.DrawLine(pos, pos + tempDir); 
         
        }
    }
    
}
