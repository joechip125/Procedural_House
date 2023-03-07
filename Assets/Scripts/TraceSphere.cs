using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TraceSphere : MonoBehaviour
{
    [SerializeField] private Transform target;
    
    
    void Update()
    {
       // var pos = transform.position;
       // var y = GetLevelHeight(new Vector3(0,-1,0), transform.position, 3);
       if (target)
       {
           Debug.DrawLine(transform.position, target.position, Color.green);
       }
       
    }
    
    private float GetLevelHeight(Vector3 dir, Vector3 pos, float traceDistance)
    {
        var ray = new Ray(pos, dir);

        if (Physics.Raycast(ray, out var hit, traceDistance))
        {
            var layer = hit.collider.gameObject.layer;
            Debug.DrawRay(pos, dir, Color.red);

            return hit.point.y;
        }

        return 0;
    }
}
