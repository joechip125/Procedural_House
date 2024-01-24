using System;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class Movement : MonoBehaviour
    {
        [SerializeField] 
        private KeyCode forward = KeyCode.W;
        [SerializeField] 
        private KeyCode back = KeyCode.S;
        [SerializeField] 
        private KeyCode left= KeyCode.A;
        [SerializeField] 
        private KeyCode right= KeyCode.D;

        [SerializeField] private float speed = 4f;
        
        private void Update()
        {
            var moveAmount = Vector3.zero;
            
            if (Input.GetKey(forward))
            {
                moveAmount += Vector3.forward * (Time.deltaTime * speed);
            }
            else if (Input.GetKey(back))
            {
                moveAmount += Vector3.back * (Time.deltaTime * speed);
            }
            
            if (Input.GetKey(left))
            {
                moveAmount += Vector3.left * (Time.deltaTime * speed);
            }
            else if (Input.GetKey(right))
            {
                moveAmount += Vector3.right * (Time.deltaTime * speed);
            }

            transform.position += moveAmount;
        }
    }
}