using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestDoor : MonoBehaviour
{
    [Range(4, 200)] public float sizeX;
    [Range(4, 200)] public float sizeY;
    [Range(4, 200)] public float sizeZ;

    
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnDrawGizmos()
    {
        var pos = transform.position;
        Gizmos.DrawWireCube(pos, new Vector3(sizeX, sizeY, sizeZ));
    }
}
