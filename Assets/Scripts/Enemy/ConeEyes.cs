using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class ConeEyes : MonoBehaviour
{
    public List<Vector3> tracePointsList = new();

    [Header("Cone Values")]
    [Range(30f, 180f),SerializeField] public float angle;
    [Range(1f, 60f),SerializeField] public float maxDistance;

    [Header("Trace Values")] 
    [Range(0, 2), SerializeField] public float traceInterval;

    private float _timeSinceTrace;

    public void SetTraceList(bool overwriteOrExtend, List<Vector3> newPoints)
    {
        if (overwriteOrExtend)
        {
            tracePointsList.Clear();
        }
        
        tracePointsList.AddRange(newPoints);
    }

    public enum ObjectTypes
    {
        Wall,
        Player,
        PickupPoint,
        DropOffPoint,
        DownedEnemy,
        DownedPlayer,
        Building
    }

    public ObjectInteractions objInt;
    
    [Flags]
    public enum ObjectInteractions
    {
        None = 0,
        CanPutDownItem = 1,
        CanPickUpItem = 2,
        CanDestroy = 4,
        CanBuild = 8
    };


    public void AddAttributeToItem(bool addORemove, ObjectInteractions interaction)
    {
       var hasFlag = objInt.HasFlag(interaction);

        if (addORemove)
        {
            if (hasFlag) return;
            objInt |= interaction;
        }
        else
        {
            if (!hasFlag) return;
            objInt &= ~interaction;
        }
        
    }
    
    public void RegisterWithObject()
    {
        if ((int) objInt == 0)
        {
            
        }

        if (objInt.HasFlag(ObjectInteractions.CanBuild))
        {
            
        }
    }

    private void Update()
    {
        if (_timeSinceTrace >= traceInterval)
        {
            SphereCaster();
            _timeSinceTrace -= traceInterval;
        }

        _timeSinceTrace += Time.deltaTime;
    }

    public void CheckManyPoints(List<Transform> targets)
    {
        
    }

    private void SphereCaster()
    {
        var thisPos = transform.position;
        var hits = Physics.SphereCastAll(thisPos, maxDistance, transform.forward);

        foreach (var h in hits)
        {
            var directionToTarget = h.collider.transform.position - thisPos;
            
            var degreesToTarget =
                Vector3.Angle(transform.forward, directionToTarget);
            var withinArc = degreesToTarget < (angle / 2);
            
            if (!withinArc) continue;
            
            var distanceToTarget = directionToTarget.magnitude;
            var rayDistance = Mathf.Min(maxDistance, distanceToTarget);
            var ray = new Ray(thisPos, directionToTarget);
            
            if (Physics.Raycast(ray, out var hit, rayDistance))
            {
            }
            else
            {
            }
        }
    }
    
    public bool CheckVisibilityToPoint(Vector3 worldPoint, Transform target)
    {
        var thisPos = transform.position;
        var directionToTarget = worldPoint - thisPos;
        
        var degreesToTarget =
            Vector3.Angle(transform.forward, directionToTarget);
        var withinArc = degreesToTarget < (angle / 2);

        if (!withinArc) return false;
        
        var distanceToTarget = directionToTarget.magnitude;
        var rayDistance = Mathf.Min(maxDistance, distanceToTarget);
        var ray = new Ray(thisPos, directionToTarget);

        if (Physics.Raycast(ray, out var hit, rayDistance))
        {
            if (hit.collider.transform == target)
            {
                return true;
            }

            return false;
        }
        else
        {
            return true;
        }
        
    }

    private void OnDrawGizmos()
    {
       // var pos = transform.forward * maxDistance;
       // Gizmos.DrawSphere(pos, 1f);
    }
}

#if UNITY_EDITOR
[CustomEditor(typeof(ConeEyes))]
public class EnemyVisibilityEditor : Editor 
{
    private void OnSceneGUI()
    {
        var visibility = target as ConeEyes;
        Handles.color = new Color(1, 1, 1, 0.1f);
        var visPos = visibility.transform.position;
        
        var forwardPointMinusHalfAngle =
            // rotate around the y-axis by half the angle
            Quaternion.Euler(0, -visibility.angle / 2, 0)
            // rotate the forward direction by this
            * visibility.transform.forward;
        
        var arcStart = forwardPointMinusHalfAngle * visibility.maxDistance;
        
        Handles.DrawSolidArc(visPos,Vector3.up,arcStart,
            visibility.angle, visibility.maxDistance);
        Handles.color = Color.white;
        
        var handlePosition = visPos + visibility.transform.forward * visibility.maxDistance;
        
        visibility.maxDistance 
            = Handles.ScaleValueHandle(visibility.maxDistance, handlePosition,               
            visibility.transform.rotation,1, Handles.ConeHandleCap, 0.25f);                      
    }
}
#endif
