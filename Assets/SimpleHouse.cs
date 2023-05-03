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
    

    void Start()
    {
        meshes.Add(Instantiate(roomTiles[0], transform).GetComponent<NewAdvancedMesh>());
        DoSomething();
    }

    private void AddCallback(Vector3 start, Vector3 direction)
    {
        var add = direction * 307.5f;
        var newStart = start + add;
        meshes.Add(Instantiate(roomTiles[0], newStart, 
            Quaternion.identity, transform).GetComponent<NewAdvancedMesh>());
    }
    
    private void DoSomething()
    {
        var temp = (NewAdvancedMesh_Floor)meshes[0];
        temp.callback = AddCallback;
        
        temp.AddAnOpen();
    }
}
