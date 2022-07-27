using System;
using System.Collections;
using System.Collections.Generic;
using Enemy;
using UnityEngine;

public class EnemyCommander : MonoBehaviour, IEnemyCommands, IInteract
{

    [SerializeField] private CurrentCommand command;
    [SerializeField] private List<Transform> goals;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

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

    public void SetInstruction(AiAgent agent)
    {
        agent.TargetQueue.Enqueue(goals[0].position);
        agent.TargetQueue.Enqueue(transform.position);
        agent.commandQueue.Enqueue(CurrentCommand.MoveToPosition);
        agent.commandQueue.Enqueue(CurrentCommand.PickupItem);
        agent.commandQueue.Enqueue(CurrentCommand.MoveToPosition);
        agent.commandQueue.Enqueue(CurrentCommand.DropOfItem);
        
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
