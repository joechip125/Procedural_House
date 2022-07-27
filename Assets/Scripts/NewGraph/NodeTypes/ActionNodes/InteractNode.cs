using System.Linq;
using Enemy;

namespace NewGraph.NodeTypes.ActionNodes
{
    public class InteractNode : ActionNode
    {
        public Instruction instruct;
        public bool interactDone;
        public override void OnStart()
        {
            Interact();
        }

        
        
        public override void OnExit()
        {
           
        }

        private void Interact()
        {
            if (agent.enemyEyes.currentMem?.Transform.GetComponent<IInteract>() != null)
            {
                agent.enemyEyes.currentMem?.Transform.GetComponent<IInteract>().GetInstruction(x =>
                {
                    instruct = x;
                });            
            }

            agent.currentDestination = instruct.finalDestination;
            

            interactDone = true;
        }
        
        public override State OnUpdate()
        {
            return interactDone ? State.Success : State.Update;
        }
    }
}