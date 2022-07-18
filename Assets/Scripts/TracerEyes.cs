using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Flags]
public enum TraceType
{
    Ground,
    Wall,
    Player,
    Platform,
    Enemy,
    None
}


public class TracerEyes : MonoBehaviour
{
    private int multiMask;
    private float traceInterval = 0.4f;
    private float timeSinceTrace;

    private void Awake()
    {
        multiMask = 1 << 6 | 1 << 8;
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceTrace += Time.deltaTime;

        if (timeSinceTrace >= traceInterval)
        {
            timeSinceTrace -= traceInterval;

            DoMultiTrace();
        }
    }

    private void DoMultiTrace()
    {
        
    }

    private TraceType DoSingleTrace(Vector3 dir, Vector3 pos, float traceDistance)
    {
        var ray = new Ray(pos, dir);

        if (Physics.Raycast(ray, out var hit, traceDistance, multiMask))
        {
            var layer = hit.collider.gameObject.layer;
            if (layer is 6)
            {
                Debug.DrawRay(pos, dir *hit.distance, Color.green, traceInterval);
            
                return TraceType.Ground | TraceType.Wall;
            }
            
            if(layer is 8)
            {
                Debug.DrawRay(pos, dir *hit.distance, Color.blue, traceInterval);
            
                return TraceType.Player;
            }
        }
        
        return TraceType.None;
    }
}
