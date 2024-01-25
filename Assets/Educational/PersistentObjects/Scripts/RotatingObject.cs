using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class RotatingObject : PersistableObject
    {
        [SerializeField]
        Vector3 angularVelocity;

        void FixedUpdate () 
        {
            transform.Rotate(angularVelocity * Time.deltaTime);
        }
    }
}