using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class EnemyCommander : MonoBehaviour, IEnemyCommands, IInteract
{

    [SerializeField] private CurrentCommand command;
    [SerializeField] private List<Transform> goals;
    [SerializeField] private AreaControl areaControl;

    public void MoveToDestination(Vector3 destination)
    {
    }

    public void SetNextCommand(CurrentCommand theCommand)
    {
        
    }

    public void GetNextDestination(Action<Instruction> callBack)
    {
        if (goals.Count > 0)
        {
            callBack?.Invoke(new Instruction()
            {
                finalDestination =  goals[0].position
            });
        }
    }

    public void GetInstruction(Action<Instruction> instruction)
    {
        instruction?.Invoke(new Instruction()
        {
            finalDestination = goals[0].position
        });
    }

    public void SearchArea()
    {
        
    }
    
    
    
    public void SetInstruction(AiAgent agent)
    {
        agent.commandQueue.Enqueue(CurrentCommand.SearchArea);

        if (agent.area == default)
        {
            agent.area = areaControl;
        }
    }

    public GameObject GetItem()
    {
        throw new NotImplementedException();
    }

    public void GiveItem(GameObject theItem)
    {
        throw new NotImplementedException();
    }
}
