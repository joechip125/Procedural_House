using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveNode : ActionNode
{
    public override void OnStart()
    {
        
    }

    public override void OnExit()
    {
        
    }
    
    private bool CheckIfLookingAtTarget()
    {
        var dirFromAtoB = (agent.enemyTransform.position - agent.currentDestination).normalized;
        var dotProd = Vector3.Dot(dirFromAtoB, agent.enemyTransform.forward);
            
        return dotProd > 0.95f;
    }

    private bool ArrivedAtTarget()
    {
        return Vector3.Distance(agent.enemyTransform.position, agent.currentDestination) < 2;
    }

    public override State OnUpdate()
    {
        if (!CheckIfLookingAtTarget())
        {
            var singleStep = Time.deltaTime * 2;
            Vector3 newDirection = Vector3.RotateTowards(agent.enemyTransform.forward, agent.currentDestination, singleStep, 0.0f);
            agent.enemyTransform.rotation = Quaternion.LookRotation(newDirection);
        }

        agent.enemyTransform.position += agent.enemyTransform.forward * (Time.deltaTime * 2);

        if (ArrivedAtTarget()) return State.Success;

        return State.Update;
    }
}
