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
           sensor.AddObservation(transform.localRotation);
        }

        public override void WriteDiscreteActionMask(IDiscreteActionMask actionMask)
        {
            
        }

        public override void OnActionReceived(ActionBuffers actions)
        {
           
        }

        public override void OnEpisodeBegin()
        {
           
        }
    }
}