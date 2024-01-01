using System;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class Bullet : MonoBehaviour
    {
        private void OnCollisionEnter(Collision collision)
        {
            
            gameObject.SetActive(false);
        }
    }
}