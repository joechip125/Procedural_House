using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class EnemyVars
{
    public STATE currentState;

    public EnemyVars()
    {
        currentState = STATE.Idle;
    }
}
