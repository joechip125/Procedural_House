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
        agent.enemyTransform.position += new Vector3(1 * (Time.deltaTime * 3), 0);
        
        return stop ? State.Success : State.Update;
    }
}
