using UnityEngine;

namespace NewGraph.NodeTypes.ActionNodes
{
    public class FindPathNode : ActionNode
    {
        private Vector3 _destination;
        private CurrentCommand _command;
        private bool _exitOkay;
        
        public override void OnStart()
        {
           _command = agent.commandQueue.Peek();

           if (_command == CurrentCommand.SearchArea)
           {
              agent.currentDestination = agent.area.GetCloseDestination(agent.enemyTransform.position);
              _exitOkay = true;
           }
        }

        public override void OnExit()
        {
            
        }

        public override State OnUpdate()
        {
            return _exitOkay ? State.Success : State.Update;
        }
    }
}