using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
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


public class Memory
{
    public TraceType type;
    public Transform Transform;
}

public class TracerEyes : MonoBehaviour
{
    private int multiMask;
    private float traceInterval = 0.4f;
    private float timeSinceTrace;
    private Vector3 currentDir;
    private object m_Hit;
    public bool PlayerSeen { get; private set; }
    public bool CommanderSeen { get; private set; }
    public bool WallSeen { get; private set; }
    private Vector3 cubeSize = new Vector3(2, 2, 2);
    private bool m_HitDetect;

    public List<Memory> Memories;

    public Memory currentMem;

    public float DistanceToObject { get; private set; }

    public event Action<TraceType> objectHit;

    private void Awake()
    {
        DistanceToObject = 999;
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
        DoBoxTrace();
    }

    private void DoMultiTrace()
    {
       var some = DoSingleTrace(transform.forward, transform.position, 34f);
       if (some != TraceType.None)
       {
           objectHit?.Invoke(some);
       }
    }
    

    private void DoBoxTrace()
    {
        
        var hits = Physics.BoxCastAll(transform.position + transform.forward * 1,
            cubeSize / 2, transform.forward, transform.rotation, 0.5f).ToList();

       var somet = hits.Where(h => h.collider.gameObject.layer == 8).ToList();
       m_HitDetect = somet.Count > 0;

       somet.ForEach(x =>
       {
           if (x.collider.gameObject.layer == 8)
           {
               currentMem = new Memory()
               {
                    type = TraceType.Commander,
                    Transform = x.transform
               };
               if (Memories.SingleOrDefault(y => y.type == TraceType.Commander) != default)
               {
                   
               }
               else
               {
                   Memories.Add(new Memory()
                   {
                       Transform = x.transform,
                       type = TraceType.Commander
                   });
               }
           }
       });
       
       if (m_HitDetect)
       {
           
       }

    }

    private void OnDrawGizmos()
    {
        if (m_HitDetect)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireCube(transform.position + transform.forward * 1, cubeSize);
        }
        else
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireCube(transform.position + transform.forward * 1, cubeSize);
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
                    DistanceToObject = hit.distance;
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
