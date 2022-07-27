using System.Linq;
using Enemy;

namespace NewGraph.NodeTypes.ActionNodes
{
    public class InteractNode : ActionNode
    {
        public Instruction instruct;
        public bool interactDone;
        private CurrentCommand command;
        public override void OnStart()
        {
            Interact();
        }

        
        
        public override void OnExit()
        {
           
        }

        private void Interact()
        {
           var interact = agent.enemyEyes.currentMem?.Transform.GetComponent<IInteract>();
            
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
            
            interactDone = true;
        }
        
        public override State OnUpdate()
        {
            return interactDone ? State.Success : State.Update;
        }
    }
}