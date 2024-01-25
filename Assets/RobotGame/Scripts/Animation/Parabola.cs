namespace RobotGame.Scripts.Animation
{
   using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public class Parabola 
    {
        Vector3 start;
        Vector3 end;
        Vector3 startVel;
        Vector3 acceleration;
    
        public float Plot(Vector3 startPos, Vector3 endPos, float speed, Vector3 gravity, bool shortest=true)
        {
            Vector3 direction = endPos - startPos;
            Vector3 horizontalDirection = Vector3.ProjectOnPlane(direction, -gravity.normalized);
            float verticalDifference = Vector3.Dot( direction, -gravity.normalized );
            float horizontalDistance = horizontalDirection.magnitude;
          
    
            Debug.DrawLine(startPos,startPos+horizontalDirection,Color.red);
            Debug.DrawLine(startPos + horizontalDirection, startPos + horizontalDirection + Vector3.up * verticalDifference, Color.yellow);
            Vector2 d = new Vector2(horizontalDistance, verticalDifference);
            float speedSqr = speed * speed;
            float g = gravity.magnitude;
            start = startPos;
            end = endPos;
            
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
    
            Debug.DrawLine(startPos, startPos+startVel, Color.green );
            
            //duration
            return horizontalDistance / horizontalSpeedComponent;
        }
    
        public Vector3 GetPosition(float t)
        {
            return start + t * startVel + 0.5f * acceleration * t * t;
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