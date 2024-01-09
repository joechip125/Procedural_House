using System;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class Bullet : PersistentObject
    {
        public Vector3 direction;
        public float speed;
        public Action<string> OnTargetHit;
        public Action<int> BulletHitTarget;
        private bool hit;

        private const float LifeSpan = 8;

        private float timeAlive;
        
        private void Update()
        {
            if (hit) return;
            timeAlive += Time.deltaTime;
            if (timeAlive >= LifeSpan)
            {
                gameObject.SetActive(false);
            }
            transform.localPosition += direction * (speed * Time.deltaTime);
        }

        private void OnCollisionEnter(Collision collision)
        {
            if (hit) return;
            OnTargetHit?.Invoke(collision.body.tag);
            if (collision.body.CompareTag("Player"))
            {
                BulletHitTarget?.Invoke(1);    
            }
            else if (collision.body.CompareTag("EnemyCommander"))
            {
                BulletHitTarget?.Invoke(2);
            }
            gameObject.SetActive(false);
            hit = true;
        }
    }
}