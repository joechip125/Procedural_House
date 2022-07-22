using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    Idle, Patrol, Pursue, Attack, Rest, Jump
}

public class MoveNode : ActionNode
{
    public bool stop;
    
    public override void OnStart()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override State OnUpdate()
    {
        if (agent.enemyEyes.DistanceToWall < 2f)
        {
            return State.Success;
        }
        
      //  agent.enemyTransform.position += agent.enemyTransform.up * (Time.deltaTime * 2);
        
        return stop ? State.Success : State.Update;
    }
}
