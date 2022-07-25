using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelTest : MonoBehaviour
{
    // Start is called before the first frame update
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
       
        Gizmos.DrawLine(pos, pos + new Vector3(1,0,0));
        Gizmos.DrawLine(pos, pos + new Vector3(0,0,1));
        Gizmos.DrawLine(pos + new Vector3(1,0,1), pos + new Vector3(1,0,1) - new Vector3(1,0,0));
        Gizmos.DrawLine(pos + new Vector3(1,0,1), pos + new Vector3(1,0,1) - new Vector3(0,0,1));
    }
}
