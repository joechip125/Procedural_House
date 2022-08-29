using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControlPoint : MonoBehaviour
{
    [SerializeField] private Transform targetLimb;
    public float DistanceFromTarget { get; private set; }

    [HideInInspector] public bool movePoint;
    private bool _setNewTarget;

    private Vector3 _keepPos;
    private MathParabola _parabola = new MathParabola();
    
    private float _timer;
    private Vector3 _targetStart;
    private Vector3 _targetEnd;
    


    private void Start()
    {
        _keepPos = transform.position;
    }

    private float GroundTrace()
    {
        var pos = transform.position + new Vector3(0, 0.3f);
        var layer = 1 << 7;
        var ray = new Ray(pos, new Vector3(0, -1));
        Physics.Raycast(ray, out var hit, 1, layer);
        return hit.point.y;
    }

    public void SetNewPos(Vector3 moveDir, float moveAmount)
    {
        _keepPos += new Vector3(moveDir.x * moveAmount, 0, moveDir.z * moveAmount);
    }
    
    private void TargetTrace()
    {
        var pos = transform.position;
        var tarPos = targetLimb.position;
        DistanceFromTarget = Vector3.Distance(pos, tarPos);
        Debug.DrawLine(pos, tarPos);
    }
    
    private void MoveTarget(float time)
    {
        var pos = _parabola.Parabola(_targetStart, _targetEnd, 0.3f, time);
        _keepPos = pos;
        transform.position = pos;
    }
    
    private void SetNewTargets()
    {
        _targetStart = transform.position;
        var fwd = transform.forward;
        _targetEnd = _targetStart +new Vector3(fwd.x * 0.3f, 0, fwd.z * 0.3f);
    }
    
    void LateUpdate()
    {
        var thePos = transform.position;
        var groundY = GroundTrace();

        if (!movePoint)
        {
            transform.position = new Vector3(_keepPos.x, groundY + 0.1f, _keepPos.z);
        }
        else
        {
            if (!_setNewTarget)
            {
                SetNewTargets();
                _setNewTarget = true;
            }
            
            _timer += Time.deltaTime * 0.5f;
            MoveTarget(_timer);
            if (_timer >= 1)
            {
                _timer --;
                movePoint = false;
                _setNewTarget = false;
            }
            
        }
    }
    
#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
    }
#endif
}
