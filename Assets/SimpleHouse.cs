using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class MeshHolder
{
    public NewAdvancedMesh mesh;
    public Transform gameObjectTransform;
}

public class SimpleHouse : MonoBehaviour
{
    private List<NewAdvancedMesh> meshes = new();
    [SerializeField] private List<GameObject> roomTiles = new();
    private Vector3 startSize;
    

    void Start()
    {
        var center = new Vector3();
        meshes.Add(Instantiate(roomTiles[0], transform).GetComponent<NewAdvancedMesh>());
        //DoSomething(0);
        AddWall();
    }

    private void AddCallback(Vector3 start, Vector3 direction)
    {
        var add = direction * 155f;
       
        var newStart = start + add;
        meshes.Add(Instantiate(roomTiles[0], newStart, 
            Quaternion.identity, transform).GetComponent<NewAdvancedMesh>());
        
        var temp = (NewAdvancedMesh_Floor) meshes[^1];
        temp.SetValuesAndActivate(100, 4,4);
        AddWall();

    }

    private void AddWall()
    {
        var place = new Vector3(200, 0,200);
        meshes.Add(Instantiate(roomTiles[1],place, Quaternion.identity, transform )
            .GetComponent<NewAdvancedMesh>());
        var temp = (NewAdvancedMesh_Wall)meshes[^1];
                
    }
    
    private void DoSomething(int index)
    {
        var temp = (NewAdvancedMesh_Floor)meshes[index];
        temp.Callback = AddCallback;
        temp.SetValuesAndActivate(100, 5,5);
        startSize = new Vector3(500,100,500);
        
        
        temp.AddAnOpen();
    }
}
