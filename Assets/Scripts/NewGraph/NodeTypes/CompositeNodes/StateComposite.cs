using System.Collections.Generic;

namespace NewGraph.NodeTypes.CompositeNodes
{
    public class StateComposite : BaseNode
    {
        public Dictionary<STATE, BaseNode> states;

        public override void OnStart()
        {
            
        }

        public override void OnExit()
        {
           
        }

        public override State OnUpdate()
        {
            return State.Update;
        }
    }
}