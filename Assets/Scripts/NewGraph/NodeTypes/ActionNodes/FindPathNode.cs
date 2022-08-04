using UnityEngine;

namespace NewGraph.NodeTypes.ActionNodes
{
    public class FindPathNode : ActionNode
    {
        private Vector3 _destination;
        private CurrentCommand _command;
        
        public override void OnStart()
        {
           _command = agent.commandQueue.Peek();

           if (_command == CurrentCommand.SearchArea)
           {
               agent.area.GetCloseDestination(agent.enemyTransform.position);
           }
        }

        public override void OnExit()
        {
            
        }

        public override State OnUpdate()
        {
            throw new System.NotImplementedException();
        }
    }
}