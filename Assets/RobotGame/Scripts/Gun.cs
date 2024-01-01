using System;
using System.Collections.Generic;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class Gun : MonoBehaviour, IComponent
    {

        [SerializeField] 
        private Transform exit;

        [SerializeField] 
        private GameObject bullet;

        [SerializeField] 
        private KeyCode shoot = KeyCode.C;

        private List<Bullet> bullets = new();

       

        private void Hit(string hitTag)
        {
            
        }

        private void Update()
        {
            if (Input.GetKeyDown(shoot))
            {
                Use();
            }
        }

        public void Use()
        {
            var dir =transform.InverseTransformDirection(Vector3.forward);
            var tempB =Instantiate(bullet, exit.localPosition, Quaternion.identity, transform);
            var b =tempB.GetComponent<Bullet>().OnTargetHit = Hit;
            
        }
    }
}