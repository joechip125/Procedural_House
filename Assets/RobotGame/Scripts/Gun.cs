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
            
        }
    }
}