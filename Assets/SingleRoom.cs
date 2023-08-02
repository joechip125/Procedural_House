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
    [SerializeField, Range(0, 1000)] private float sizeX = 400;
    [SerializeField, Range(0, 1000)] private float sizeY = 400;
    
    private void Awake()
    {
        InitRoom();
    }
    
    private void InitRoom()
    {
        var center = transform.position;
        AddSquare(center);
        
        meshes.Add(Instantiate(spawnable[0], transform).GetComponent<NewAdvancedMesh>());
        var temp = (NewAdvancedMesh_Floor)meshes[^1];
        temp.SetValuesAndActivate(sizeX, sizeY);
        
        Debug.Log($"corner 0:{corners[0]}");
        
        meshes.Add(Instantiate(spawnable[1], corners[0], Quaternion.identity, transform).GetComponent<NewAdvancedMesh>());
        var temp1 = (NewAdvancedMesh_Wall)meshes[^1];
        var angle = Quaternion.AngleAxis(180, Vector3.up) *Vector3.right;
        temp1.InitWall(angle, new Vector2(sizeY,100), 4);
    }

    private void AddSquare(Vector3 pos, float degreeAdjust = 0)
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
        
        var tempPos = new Vector3();
        sizeX /= 2;
        sizeY /= 2;
        Debug.Log($"size_x: {sizeX} size_y {sizeY}");
        for (int i = 0; i < tempVectors.Length; i++)
        {
            if (i == tempVectors.Length - 1) tempPos = pos + tempVectors[0] * sizeX + tempVectors[^1] * sizeY;
            else
            {
                if (i % 2 == 0) tempPos = pos + tempVectors[i] * sizeX + tempVectors[i + 1] * sizeY;
                else tempPos = pos + tempVectors[i + 1] * sizeX + tempVectors[i] * sizeY;
            }
            corners.Add(tempPos);
        }
    }

    public void AddDoor(int number)
    {
        number = Mathf.Clamp(number, 1, 4);
        var temp = (NewAdvancedMesh_Wall)meshes[number];
        temp.AddDoor(2);
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

    private void RoomTest()
    {
        var pos = transform.position;
        infos.Clear();

        infos.Add(new TileInfo()
        {
            center = pos,
            size = new Vector3(400,100,400)
        });

        var nextSize = new Vector3(400,100,200);
        pos += Vector3.forward * (infos[^1].size.z / 2 + nextSize.z / 2);
        
        infos.Add(new TileInfo()
        {
            center = pos,
            size = nextSize
        });
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
