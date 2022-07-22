using UnityEngine;

namespace NewGraph.NodeTypes.ActionNodes
{
    public class SearchNode : ActionNode
    {
        public override void OnStart()
        {
            
        }

        public override void OnExit()
        {
            
        }

        public override State OnUpdate()
        {
            agent.enemyTransform.Rotate(new Vector3(1,0,0), 2 * Time.deltaTime);
            
           
            
            return State.Update;
        }
    }
}