﻿using System;
using UnityEditor;
using UnityEngine;
public class MoveTest : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget;
    [SerializeField] Transform moveTarget;
    [SerializeField] Transform enemyTransform;
    [SerializeField] Transform rightTargetZone;
    [SerializeField] Transform leftTargetZone;
    [SerializeField] Transform rightLegTarget;

    private float _startY;
    private Vector3 _starter;

    private MathParabola _parabola = new MathParabola();

    private Vector3 _rightStart;
    private Vector3 _rightEnd;

    private float _timer;

    private void Start()
    {
        _starter = rightLegTarget.position;
        SetNewTargets();
    }

    private bool CheckIfLookingAtTarget()
    {
        var dirFromAtoB = (enemyTransform.position - moveTarget.position).normalized;
        var dotProd = Vector3.Dot(dirFromAtoB, enemyTransform.forward);
        return dotProd > 0.9f;
    }
    
    private bool ArrivedAtTarget()
    {
        return Vector3.Distance(enemyTransform.position, moveTarget.position) < 1f;
    }

    private void MoveTarget(float time)
    {
        var pos = _parabola.Parabola(_rightStart, _rightEnd, 0.3f, time);

        rightLegTarget.position = pos;

    }

    private void SetNewTargets()
    {
        _rightStart = rightLegTarget.position;
        var fwd = transform.forward;
        _rightEnd = _rightStart +new Vector3(fwd.x * 0.3f, 0, fwd.z * 0.3f);
    }
    
    private void Update()
    {
        if (!CheckIfLookingAtTarget())
        {
            var targetDirection = (moveTarget.position - enemyTransform.position).normalized;
            var singleStep = Time.deltaTime * 1;
            var newDirection = Vector3.RotateTowards(enemyTransform.forward, targetDirection, singleStep, 0.0f);
            enemyTransform.rotation = Quaternion.LookRotation(newDirection);
        }

        _timer += Time.deltaTime * 0.3f;

        if (_timer > 1)
        {
            _timer --;
        }
       // MoveTarget(_timer);

       rightLegTarget.position = _rightStart;

        if (!ArrivedAtTarget())
        {
            enemyTransform.position += enemyTransform.forward * (Time.deltaTime * 1);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(moveTarget.position, 0.1f);
        
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(rightTargetZone.position, 0.1f);
        
    }
    #endif
}
