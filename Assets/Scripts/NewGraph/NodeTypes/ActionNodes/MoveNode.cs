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
    public int numberRot;
    
    public override void OnStart()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override State OnUpdate()
    {
        if (numberRot < 45)
        {
            agent.enemyTransform.Rotate(new Vector3(1, 0, 0), 2 * Time.deltaTime);
            numberRot++;
        }
        //  agent.enemyTransform.position += agent.enemyTransform.up * (Time.deltaTime * 2);
        
        return stop ? State.Success : State.Update;
    }
}
