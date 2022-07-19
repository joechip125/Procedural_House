using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectNode : CompositeNode
{
    [HideInInspector] public bool choiceMade;
    [HideInInspector] public int theChoice;
    
    public override void OnStart()
    {
        
    }

    public override void OnExit()
    {
        
    }

    public override State OnUpdate()
    {
        var child = children[theChoice];

        if (!choiceMade) return State.Update;
        
        switch (child.Update())
        {
            case State.Failure:
                return State.Failure;
            
            case State.Update:
                return State.Update;

            case State.Success:
                choiceMade = false;
                break;
        }

        return State.Update;
    }
}
