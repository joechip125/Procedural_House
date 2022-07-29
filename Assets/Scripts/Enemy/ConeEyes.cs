using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class ConeEyes : MonoBehaviour
{
    public List<Vector3> TracepointsList = new();

    [Header("Cone Values")]
    [Range(30f, 180f),SerializeField] private float angle;
    [Range(1f, 60f),SerializeField] private float maxDistance;
    // Start is called before the first frame update
    
    void Start()
    {
        
    }

    public void SetTraceList(bool overwriteOrExtend, List<Vector3> newPoints)
    {
        if (overwriteOrExtend)
        {
            TracepointsList.Clear();
        }
        
        TracepointsList.AddRange(newPoints);
       
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public enum ObjectTypes
    {
        Wall,
        Player,
        PickupPoint,
        DropOffPoint,
        DownedEnemy,
        DownedPlayer
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
        if(interaction.HasFlag(interaction)) return;

        if (addORemove)
        {
            
        }
        
        if (!interaction.HasFlag(interaction))
        {
            objInt |= interaction;
        }
        
        if(interaction is ObjectInteractions.CanBuild and interaction is ObjectInteractions.CanDestroy)
    }
    
    public void RegisterWithObject()
    {
        
    }
    
    public bool CheckVisibilityToPoint(Vector3 worldPoint)
    {
        
        var directionToTarget = worldPoint - transform.position;
        
        var degreesToTarget =
            Vector3.Angle(transform.forward, directionToTarget);
        
        var withinArc = degreesToTarget < (angle / 2);
        
        var distanceToTarget = directionToTarget.magnitude;
        
        var rayDistance = Mathf.Min(maxDistance, distanceToTarget);

        return withinArc;
    }
}
