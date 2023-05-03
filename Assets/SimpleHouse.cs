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
        meshes.Add(Instantiate(roomTiles[0], transform).GetComponent<NewAdvancedMesh>());
        DoSomething(0);
    }

    private void AddCallback(Vector3 start, Vector3 direction)
    {
        var add = direction * 155f;
        //var add = Vector3.Scale(startSize / 2, direction);
        var newStart = start + add;
        meshes.Add(Instantiate(roomTiles[0], newStart, 
            Quaternion.identity, transform).GetComponent<NewAdvancedMesh>());
        
        var temp = (NewAdvancedMesh_Floor) meshes[^1];
        temp.SetValuesAndActivate(100, 4,4);

    }

    private void AddWall()
    {
        
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
