using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    Idle, Patrol, Pursue, Attack, Rest, Jump
}

public class MoveNode : ActionNode
{
    private Transform parent;
    public bool stop;
    
    public override void OnStart()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override State OnUpdate()
    {
        
        
        return stop ? State.Success : State.Update;
    }
}
