using System;
using UnityEngine;

public enum STATE
{
    Idle, Patrol, Pursue, Attack, Rest, Jump
}

public enum CurrentCommand
{
    None,
    Gather,
    Hunt
}

[Serializable]
public class AiAgent
{
    public TracerEyes enemyEyes;
    public Transform enemyTransform;
    public Transform destination;
    public CurrentCommand currentCommand;

    public Vector3 currentDestination;
}