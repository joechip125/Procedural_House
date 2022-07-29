using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ConeEyes : MonoBehaviour
{
    public List<Vector3> TracepointsList = new();
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
    
    public bool CheckVisibilityToPoint(Vector3 worldPoint)
    {
        
        var directionToTarget = worldPoint - transform.position;
        
        var degreesToTarget =
            Vector3.Angle(transform.forward, directionToTarget);
    }
}
