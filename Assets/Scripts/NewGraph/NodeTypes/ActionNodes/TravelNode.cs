using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TravelNode : ActionNode
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
        return dotProd > 0.98f;
    }

    private bool ArrivedAtTarget()
    {
        return Vector3.Distance(agent.enemyTransform.position, agent.currentDestination) < 2;
    }

    
    
    public override State OnUpdate()
    {
        if (!CheckIfLookingAtTarget())
        {
            Vector3 targetDirection = agent.currentDestination - agent.enemyTransform.position;
            var singleStep = Time.deltaTime * 1;
            Vector3 newDirection = Vector3.RotateTowards(agent.enemyTransform.forward, targetDirection, singleStep, 0.0f);
            agent.enemyTransform.rotation = Quaternion.LookRotation(newDirection);
        }

        agent.enemyTransform.position += agent.enemyTransform.forward * (Time.deltaTime * 1);

        if (ArrivedAtTarget()) return State.Success;

        return State.Update;
    }
}
