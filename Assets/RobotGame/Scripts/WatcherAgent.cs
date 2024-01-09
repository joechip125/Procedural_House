using System;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class WatcherAgent : Agent
    {
        private int targetHit;

        [SerializeField] 
        private Gun gun;

        private Transform start;

        [SerializeField, Range(0.5f,5)] 
        private float shootInterval = 1f;

        private float timeSinceShoot;
        private bool canShoot;

        private void Update()
        {
            timeSinceShoot += Time.deltaTime;

            if (timeSinceShoot >= shootInterval)
            {
                timeSinceShoot -= shootInterval;
                canShoot = true;
            }
        }

        public override void Initialize()
        {
            start = gun.transform;
            gun.TargetHit = SetHitValue;
        }

        private void SetHitValue(int hitValue)
        {
            targetHit = hitValue;
        }
        
        public override void CollectObservations(VectorSensor sensor)
        {
           sensor.AddObservation(gun.transform.localRotation.y);
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
            
           transform.Rotate(Vector3.up, rotation);
           gun.Rotation(rotation);
           if (shoot == 1 && canShoot)
           {
               gun.Use();
               canShoot = false;
           }
           
           if (targetHit == 1)
           {
               SetReward(1.0f);
               EndEpisode();
           }
           else if (targetHit == 2)
           {
               SetReward(-1.0f);
               EndEpisode();
           }
        }

        public override void OnEpisodeBegin()
        {
            gun.transform.rotation = start.transform.rotation;
            targetHit = 0;
        }
    }
}