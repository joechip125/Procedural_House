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
    Commander,
    Platform,
    Enemy,
    None
}


public class TracerEyes : MonoBehaviour
{
    private int multiMask;
    private float traceInterval = 0.4f;
    private float timeSinceTrace;
    private Vector3 currentDir;
    public bool PlayerSeen { get; private set; }
    public bool CommanderSeen { get; private set; }
    public bool WallSeen { get; private set; }
    
    public float DistanceToWall { get; private set; }

    public event Action<TraceType> objectHit;

    private void Awake()
    {
        DistanceToWall = 999;
        multiMask = 1 << 7 | 1 << 6 | 1 << 8;
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
       var some = DoSingleTrace(transform.forward, transform.position, 34f);
       if (some != TraceType.None)
       {
           objectHit?.Invoke(some);
       }
    }

    private TraceType DoSingleTrace(Vector3 dir, Vector3 pos, float traceDistance)
    {
        var ray = new Ray(pos, dir);

        if (Physics.Raycast(ray, out var hit, traceDistance, multiMask))
        {
            var layer = hit.collider.gameObject.layer;

            switch (layer)
            {
                case 7:
                    Debug.DrawRay(pos, dir *hit.distance, Color.green, traceInterval);
                    DistanceToWall = hit.distance;
                    return TraceType.Ground | TraceType.Wall;
                
                case 6:
                    Debug.DrawRay(pos, dir *hit.distance, Color.blue, traceInterval);
                    return TraceType.Player;
                
                case 8:
                    Debug.DrawRay(pos, dir *hit.distance, Color.black, traceInterval);
                    return TraceType.Commander;
            }
        }

        else
        {
            Debug.DrawRay(pos, dir *traceDistance, Color.red, traceInterval);
            return TraceType.None;
        }
        
        return TraceType.None;
    }
}
