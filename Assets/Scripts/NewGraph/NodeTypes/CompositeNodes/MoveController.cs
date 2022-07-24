using UnityEngine;

namespace NewGraph.NodeTypes.CompositeNodes
{
    public class MoveController : CompositeNode
    {
        public override void OnStart()
        {
            
        }

        public override void OnExit()
        {
           
        }

        private bool CheckIfLookingAtTarget()
        {
            var dirFromAtoB = (agent.enemyTransform.position - agent.currentDestination).normalized;
            var dotProd = Vector3.Dot(dirFromAtoB, agent.enemyTransform.forward);
            
            return dotProd > 0.95f;
        }

        private bool ArrivedAtTarget()
        {
            return Vector3.Distance(agent.enemyTransform.position, agent.currentDestination) < 2;
        }

        public override State OnUpdate()
        {
            return State.Update;
        }
    }
}