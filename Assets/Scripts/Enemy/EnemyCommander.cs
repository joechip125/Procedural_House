using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommander : MonoBehaviour, IEnemyCommands
{

    [SerializeField] private CurrentCommand command;
    
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

    public void GetNextDestination(Action<CurrentCommand> callBack)
    {
        throw new NotImplementedException();
    }
}
