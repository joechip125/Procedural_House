using System;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class Bullet : MonoBehaviour
    {
        public Vector3 direction;
        public float speed;
        public Action<string> OnTargetHit;
        private bool hit;
        
        private void Update()
        {
            if (hit) return;
            transform.localPosition += direction * (speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hit) return;
            OnTargetHit?.Invoke(collision.transform.tag);
            gameObject.SetActive(false);
            hit = true;
        }
    }
}