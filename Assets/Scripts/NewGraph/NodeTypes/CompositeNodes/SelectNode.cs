using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectNode : CompositeNode
{
    [HideInInspector] public bool choiceMade;
    [HideInInspector] public CurrentCommand currentCommand;
    [HideInInspector] public STATE nextState;
    
    public override void OnStart()
    {
        if (currentCommand == CurrentCommand.None)
            currentCommand = CurrentCommand.FindCommander;
        
        agent.enemyEyes.objectHit -= OnObjectSeen;
        agent.enemyEyes.objectHit += OnObjectSeen;
    }

    private void OnObjectSeen(TraceType obj)
    {
        if (currentCommand == CurrentCommand.None)
        {
            if (obj == TraceType.Commander)
            {
                
            }
        }
    }

    public override void OnExit()
    {
        agent.enemyEyes.objectHit -= OnObjectSeen;
    }

    private void ChooseNode(int tryThis)
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
        var child = children[(int)currentCommand];

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
