using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using NewGraph.NodeTypes.ActionNodes;
using UnityEditor;
using UnityEngine;

public class SelectNode : CompositeNode
{
    [HideInInspector] public bool choiceMade;
    [HideInInspector] public CurrentCommand currentCommand;
    [HideInInspector] public STATE nextState;
    private Dictionary<CurrentCommand, BaseNode> ownedNodes = new();

    public override void OnStart()
    {
        choiceMade = false;
        SetPossibleNodes();
        currentCommand = agent.commandQueue.Peek();
        choiceMade = true;
    }

    private void ChooseNode()
    {
        foreach (var n in children)
        {
            var traveler = n as TravelNode;
            
            if (currentCommand == CurrentCommand.MoveToPosition)
            {
                
                continue;
            }

            var interact = n as InteractNode;
            
            if (interact)
            {
                
                continue;
            }
        }
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

    public void SetPossibleNodes()
    {
        foreach (var n in children)
        {
            var traveler = n as TravelNode;

            if (traveler)
            {
                ownedNodes.Add(CurrentCommand.MoveToPosition, traveler);
                continue;
            }

            var interact = n as InteractNode;
            
            if (interact)
            {
                ownedNodes.Add(CurrentCommand.Interact, interact);
                ownedNodes.Add(CurrentCommand.GetInstructions, interact);
                continue;
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
    

    private BaseNode FindRightChoice(ActionNodeFunction stateChoice)
    {
        var choice = children.SingleOrDefault(x =>
        {
            var y = x as ActionNode;
            if (!y) return false;
            return y.stateType == stateChoice;
        });

        return choice;
    }
    
    public override State OnUpdate()
    {
        if (!choiceMade || agent.commandQueue.Count < 1) return State.Update;
        
        var child = ownedNodes[currentCommand];

        switch (child.Update())
        {
            case State.Failure:
                return State.Failure;
            
            case State.Update:
                return State.Update;

            case State.Success:
                currentCommand = agent.commandQueue.Peek();
                break;
        }

        return State.Update;
    }
}
