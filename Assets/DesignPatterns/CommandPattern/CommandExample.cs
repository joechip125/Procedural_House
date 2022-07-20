using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICommand
{
    void Execute();
}

public class BaseCommand : ICommand
{
    public void Execute()
    {
       
    }
}
class SimpleCommand : ICommand
{
    private string _payload = string.Empty;

    public SimpleCommand(string payload)
    {
        _payload = payload;
    }

    public void Execute()
    {
        Debug.Log($"SimpleCommand: See, I can do simple things like printing ({_payload})");
    }
}


public class Invoker
{
    private ICommand _onStart;
    private ICommand _onFinish;
    
    public void SetOnStart(ICommand command)
    {
        _onStart = command;
    }

    public void SetOnFinish(ICommand command)
    {
        _onFinish = command;
    }
    

    public void DoSomethingImportant()
    {
        Debug.Log("Invoker: Does anybody want something done before I begin?");
        _onStart?.Execute();

        Debug.Log("Invoker: ...doing something really important...");
            
        Debug.Log("Invoker: Does anybody want something done after I finish?");
        _onFinish?.Execute();
    }
}

[Serializable]
public class CommandExample 
{
    Invoker _invoker = new Invoker();
}
