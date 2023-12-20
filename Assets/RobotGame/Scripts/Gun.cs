using UnityEngine;

namespace RobotGame.Scripts
{
    public class Gun : MonoBehaviour, IComponent
    {

        [SerializeField] 
        private Transform exit;

        [SerializeField] 
        private GameObject bullet;
        
        public void Use()
        {
            var dir =transform.InverseTransformDirection(Vector3.forward);
            var tempB =Instantiate(bullet, exit.localPosition, Quaternion.identity, transform);
        }
    }
}