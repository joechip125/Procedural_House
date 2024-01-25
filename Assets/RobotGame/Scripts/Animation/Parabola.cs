using System;

namespace RobotGame.Scripts.Animation
{
   using System.Collections;
using System.Collections.Generic;
using UnityEngine;

   [Serializable]
    public class Parabola 
    {
        Vector3 start;
        Vector3 end;
        Vector3 startVel;
        Vector3 acceleration;
        public Vector3[] samplePositions;
        
        public float Plot( float speed, Vector3 gravity, bool shortest=true)
        {
            Vector3 direction = end - start;
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(direction, -gravity.normalized);
            float verticalDifference = Vector3.Dot( direction, -gravity.normalized );
            float horizontalDistance = horizontalDirection.magnitude;
            
            Debug.DrawLine(start,start+horizontalDirection,Color.red, 12f);
            Debug.DrawLine(start + horizontalDirection, start + horizontalDirection + Vector3.up * verticalDifference, Color.yellow, 12f);
            Vector2 d = new Vector2(horizontalDistance, verticalDifference);
            float speedSqr = speed * speed;
            float g = gravity.magnitude;

            float k = speedSqr / (g * d.x);
            float rootTerm2 = -1 + speedSqr * (speedSqr - 2 * g * d.y) / (g * g * d.x * d.x);
            if (rootTerm2 < 0)
            {
                throw new System.Exception("No valid solution for parabola");
            }
            
            var angle = shortest ? Mathf.Min(Mathf.Atan(k - Mathf.Sqrt(rootTerm2)), Mathf.Atan(k + Mathf.Sqrt(rootTerm2))) 
                : Mathf.Max(Mathf.Atan(k - Mathf.Sqrt(rootTerm2)), Mathf.Atan(k + Mathf.Sqrt(rootTerm2)));
            
            var horizontalSpeedComponent = speed * Mathf.Cos(angle);
            var verticalSpeedComponent = speed * Mathf.Sin(angle);
    
            acceleration = g * Vector3.down;
            startVel = horizontalSpeedComponent * horizontalDirection.normalized + verticalSpeedComponent * Vector3.up;
    
            Debug.DrawLine(start, start+startVel, Color.green );
            
            //duration
            return horizontalDistance / horizontalSpeedComponent;
        }

        public void GetSamples(Vector3 startPos,Vector3 endPos,Vector3 gravity, float speed, int numSamples = 12)
        {
            samplePositions = new Vector3[numSamples];
            var interval = 1f / numSamples;
            var totalTime = 0f;
            
            start = startPos;
            end = endPos;
            startVel = endPos;
            Plot(speed, gravity);

            for (int i = 0; i < numSamples; i++)
            {
                samplePositions[i] = GetPosition(totalTime = totalTime >= 1 ? totalTime +interval: 1);
            }
        }
        
        public Vector3 GetPosition(float t)
        {
            return start + t * startVel + 0.5f * acceleration * t * t;
        }
        
        public Vector3 GetPosition(Vector3 startPos,Vector3 startVelocity,Vector3 theAcceleration,float t)
        {
            return startPos + t * startVelocity + 0.5f * theAcceleration * t * t;
        }
    
        public Vector3 GetDirection(float t) 
        {
            return ( startVel + acceleration * t ).normalized;
        }
    
        public Vector3 GetStartVelocity() 
        {
            return startVel;
        }
    
        public static float GetMaxDistance(float speed, float gravity = 9.81f) 
        {
            return speed * speed / gravity;
        }
    
        public static float GetMaxDistance(float speed, float yDiff, float gravity)
        {
            return Mathf.Sqrt(speed*speed+2* gravity * -yDiff ) * speed / gravity;
        }
    }
}