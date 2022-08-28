using System;
using UnityEditor;
using UnityEngine;
public class MoveTest : MonoBehaviour
{
    [SerializeField] Transform lookAtTarget;
    [SerializeField] Transform moveTarget;
    [SerializeField] Transform enemyTransform;
    [SerializeField] Transform legTargetRight;
    [SerializeField] Transform legTargetLeft;
    

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
    
    private void Update()
    {
        if (!CheckIfLookingAtTarget())
        {
            var targetDirection = (moveTarget.position - enemyTransform.position).normalized;
            var singleStep = Time.deltaTime * 1;
            var newDirection = Vector3.RotateTowards(enemyTransform.forward, targetDirection, singleStep, 0.0f);
            enemyTransform.rotation = Quaternion.LookRotation(newDirection);
        }

        if (!ArrivedAtTarget())
        {
            enemyTransform.position += enemyTransform.forward * (Time.deltaTime * 1);
        }
    }

    #if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Gizmos.DrawSphere(moveTarget.position, 0.1f);
        Handles.DrawSolidArc(legTargetRight.position,Vector3.up,new Vector3(),
            360, 0.2f);
    }
    #endif
}
