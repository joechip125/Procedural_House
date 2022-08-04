using System.Linq;
using Enemy;
using UnityEngine;

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
            var interact = agent.coneEyes.InteractInterface;
            
            if (interact != null)
            {
                switch (agent.commandQueue.Dequeue())
                {
                    case CurrentCommand.PickupItem:
                        agent.heldItem = interact.GetItem();
                        break;
                    case CurrentCommand.GetInstructions:
                        interact.SetInstruction(agent);
                        break;
                    case CurrentCommand.DropOfItem:
                        interact.GiveItem(agent.heldItem);
                        agent.heldItem = null;
                        break;
                }
            }
            else
            {
              
            }
            
            interactDone = true;
        }
        
        public override State OnUpdate()
        {
            return interactDone ? State.Success : State.Update;
        }
    }
}