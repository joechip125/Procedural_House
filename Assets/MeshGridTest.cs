using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class MeshGridTest : MonoBehaviour
{
    [SerializeField, Range(0, 30)] private int numberX;
    [SerializeField, Range(0, 30)] private int numberZ;
    private List<Vector3> dots = new();

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
        dots.Clear();

        for (int i = 0; i <= numberZ; i++)
        {
            for (int j = 0; j <= numberX; j++)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(pos + new Vector3(100 * j,0,0), 3);
                dots.Add(pos + new Vector3(100 * j,0,0));
            }
            pos += new Vector3(0, 0, 100);
        }

        dots = dots.OrderBy(x => x.x).ThenBy(x => x.z).ToList();
        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(dots[0], dots[0] + new Vector3(0, 100,0));
        Gizmos.color = Color.red;
        Gizmos.DrawLine(dots[^1], dots[^1] + new Vector3(0, 100,0));

    }
}
