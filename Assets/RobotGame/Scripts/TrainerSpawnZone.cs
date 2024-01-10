using UnityEngine;

namespace RobotGame.Scripts
{
    public class TrainerSpawnZone : SpawnZone
    {
        public override Vector3 SpawnPoint
        {
            get
            {
                Vector3 p;
                p.x = Random.Range(-0.5f, 0.5f);
                p.y = 0;
                p.z = Random.Range(-0.5f, 0.5f);
                
                return transform.TransformPoint(p);
            }
        }
        
        void OnDrawGizmos () 
        {
            Gizmos.color = Color.cyan;
            Gizmos.matrix = transform.localToWorldMatrix;
            Gizmos.DrawWireCube(Vector3.zero, Vector3.one);
        }
    }
}