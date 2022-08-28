using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    public float DistanceFromTarget { get; private set; }
  

    private float GroundTrace()
    {
        var pos = transform.position + new Vector3(0, 0.3f);
        var layer = 1 << 7;
        var ray = new Ray(pos, new Vector3(0, -1));
        Physics.Raycast(ray, out var hit, 1, layer);
        return hit.point.y;
    }

    private void TargetTrace()
    {
        var pos = transform.position;
        var tarPos = targetLimb.position;
        DistanceFromTarget = Vector3.Distance(pos, tarPos);
        Debug.DrawLine(pos, tarPos);
    }
    
    void LateUpdate()
    {
        var thePos = transform.position;
        var groundY = GroundTrace();
        transform.position = new Vector3(thePos.x, groundY + 0.1f, thePos.z);
      //  TargetTrace();
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
#endif
}
