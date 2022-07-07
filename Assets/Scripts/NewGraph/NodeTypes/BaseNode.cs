using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode : ScriptableObject
{
    public State state = State.Update;
    public bool started;

    public string guid;

    public enum State
    {
        Failure,
        Update,
        Success
    }

    public State Update()
    {
        if (!started)
        {
            started = true;
            OnStart();
        }

        state = OnUpdate();

        if (state is State.Failure or State.Success)
        {
            OnExit();
            started = false;
        }

        return state;
    }

    public abstract void OnStart();
    public abstract void OnExit();
    public abstract State OnUpdate();

}
