using System.Linq;
using Enemy;
using UnityEngine;

namespace NewGraph.NodeTypes.ActionNodes
{
    public class InteractNode : ActionNode
    {
        public Instruction instruct;
        public bool interactDone;
        private CurrentCommand command;
        public override void OnStart()
        {
            command = agent.commandQueue.Dequeue();
            Interact();
        }

        
        
        public override void OnExit()
        {
           
        }

   
        private void Interact()
        {
            var interact = agent.enemyEyes.currentInteractable.GetComponentInParent<IInteract>();
            
            if (interact != null)
            {
                switch (command)
                {
                    case CurrentCommand.PickupItem:
                        agent.heldItem = interact.GetItem();
                        break;
                    case CurrentCommand.GetInstructions:
                        interact.SetInstruction(agent);
                        break;
                    case CurrentCommand.DropOfItem:
                        interact.GiveItem(agent.heldItem);
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