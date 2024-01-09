using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class WatcherAgent : Agent
    {
        private bool targetHit;

        [SerializeField] 
        private Gun gun;

        private Transform start;
        
        public override void Initialize()
        {
            start = gun.transform;
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

           gun.Rotation(rotation);
           if(shoot == 1) gun.Use();
           
           if (targetHit)
           {
               SetReward(1.0f);
               EndEpisode();
           }
        }

        public override void OnEpisodeBegin()
        {
            gun.transform.rotation = start.transform.rotation;
        }
    }
}