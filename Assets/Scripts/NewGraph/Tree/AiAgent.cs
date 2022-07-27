﻿using System;
using System.Collections.Generic;
using UnityEngine;

public enum STATE
{
    Idle, Patrol, Pursue, Attack, Rest, Jump
}

public enum CurrentCommand
{
    None,
    FindCommander,
    Gather,
    Hunt,
    Find
}

[Serializable]
public class Instruction
{
    public Vector3 finalDestination;
}

[Serializable]
public class AiAgent
{
    public TracerEyes enemyEyes;
    public Transform enemyTransform;
    public Transform destination;
    public CurrentCommand currentCommand;

    public Vector3 currentDestination;

    public Instruction currentInstruction;
    public Transform currentCommander;

    public Queue<Vector3> targets = new();

    public AiAgent()
    {
        currentDestination = new Vector3(0, 0, 0);
    }
}