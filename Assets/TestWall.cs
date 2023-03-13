using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestWall : MonoBehaviour
{

    [Range(10, 500)]public float sizeX;
    [Range(10, 500)]public float sizeY;
    
    
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
        
        Gizmos.color = Color.red;
    }
}
