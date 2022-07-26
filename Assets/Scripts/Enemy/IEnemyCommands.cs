using System;
using UnityEngine;


public interface IEnemyCommands
{
    public void MoveToDestination(Vector3 destination);

    public void SetNextCommand(CurrentCommand command);

    public void GetNextDestination(Action<CurrentCommand, Vector3> callBack);
}
