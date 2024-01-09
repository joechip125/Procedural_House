using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

namespace RobotGame.Scripts
{
    public class WatcherAgent : Agent
    {
        public override void Initialize()
        {
            
        }

        public override void CollectObservations(VectorSensor sensor)
        {
           sensor.AddObservation(transform.localRotation.y);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            
        }

        public override void Heuristic(in ActionBuffers actionsOut)
        {
            base.Heuristic(in actionsOut);
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
           var rotation = actions.ContinuousActions[0];
           var shoot = actions.DiscreteActions[0];
        }

        public override void OnEpisodeBegin()
        {
           
        }
    }
}