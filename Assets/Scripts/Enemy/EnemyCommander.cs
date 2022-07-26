using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyCommander : MonoBehaviour, IEnemyCommands
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

    public void GetNextDestination(Action<CurrentCommand, Vector3> callBack)
    {
        if (goals.Count > 0)
        {
            callBack?.Invoke(CurrentCommand.Find, goals[0].position);
        }
    }
}
