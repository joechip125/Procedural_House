using UnityEngine;


public interface IEnemyCommands
{
    public void MoveToDestination(Vector3 destination);

    public void SetNextCommand(CurrentCommand command);
}
