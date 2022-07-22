using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectNode : CompositeNode
{
    [HideInInspector] public bool choiceMade;
    [HideInInspector] public int theChoice;
    [HideInInspector] public STATE nextState;
    
    public override void OnStart()
    {
        theChoice = 1;
        choiceMade = true;
    }

    public override void OnExit()
    {
        
    }

    private void FindRightChoice(STATE stateChoice)
    {
        var choice = children.SingleOrDefault(x =>
        {
            var y = x as ActionNode;
            if (!y) return false;
            return y.stateType == stateChoice;
        });
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
