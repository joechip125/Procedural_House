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
    private Vector3 currentDir;
    public bool PlayerSeen { get; private set; }

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
            currentDir = transform.forward + new Vector3(-1,-1);
        }
    }

    private void DoMultiTrace()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                DoSingleTrace(currentDir, transform.position, 23);
                currentDir += transform.forward + new Vector3(0, 0f, 0.3f);
            }
            currentDir += transform.forward + new Vector3(0, 0.2f);
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
                case 6:
                    Debug.DrawRay(pos, dir *hit.distance, Color.green, traceInterval);
                    return TraceType.Ground | TraceType.Wall;
                
                case 8:
                    Debug.DrawRay(pos, dir *hit.distance, Color.blue, traceInterval);
                    return TraceType.Player;
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
