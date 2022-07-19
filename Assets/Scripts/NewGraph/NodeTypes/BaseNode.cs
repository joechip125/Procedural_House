using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseNode : ScriptableObject
{
    [HideInInspector] public State state = State.Update;
    [HideInInspector] public bool started;
    [HideInInspector] public string guid;

    [HideInInspector] public Vector2 position;

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


    public virtual BaseNode Clone()
    {
        return Instantiate(this);
    }
    
    public abstract void OnStart();
    public abstract void OnExit();
    public abstract State OnUpdate();

}
