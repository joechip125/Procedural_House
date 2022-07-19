using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Idle : ActionNode
{
    private float time;
    
    public override void OnStart()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override State OnUpdate()
    {
        time += Time.deltaTime * Random.Range(0, 11);
        if (time > 10)
        {
            return State.Success;
        }

        return State.Update;
    }
}
