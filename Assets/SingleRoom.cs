using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleRoom : MonoBehaviour
{
    [HideInInspector]public List<NewAdvancedMesh> meshes = new();
    [SerializeField] private List<GameObject> spawnable;
    private Vector3 theSize;

    void Start()
    {
        InitRoom(new Vector3(100,100,100));
        AddWalls();
    }

    private void InitRoom(Vector3 roomSize)
    {
        meshes.Add(Instantiate(spawnable[0], transform).GetComponent<NewAdvancedMesh>());
        var temp = (NewAdvancedMesh_Floor)meshes[^1];
        temp.SetValuesAndActivate(roomSize.x, 5,5);
        
    }

    private void AddWalls()
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
            meshes.Add(Instantiate(spawnable[1], next, Quaternion.identity, transform).GetComponent<NewAdvancedMesh>());
            var temp = (NewAdvancedMesh_Wall)meshes[^1];
            temp.BuildWall(aCrossUp2, wallSize);
        }
    }


    private void OnDrawGizmos()
    {
        var pos = transform.position;
        var cChange = 0f;
        var add = 45;
        var distance = Mathf.Sqrt(Mathf.Pow(200, 2) + Mathf.Pow(200, 2));
        for (int i = 0; i < 4; i++)
        {
            
            var aCrossUp = Quaternion.AngleAxis(add +  90 *i, Vector3.up) *Vector3.right;
            var aCrossUp2 = Quaternion.AngleAxis(add + 135 +  90 *i, Vector3.up) *Vector3.right;
            var next = pos + aCrossUp * distance;
            Gizmos.color = new Color(0,cChange,0);
            Gizmos.DrawLine(pos, next);
            Gizmos.DrawLine(next, next  + aCrossUp2 * 100);
            
            cChange += 0.25f;
        }
    }
}