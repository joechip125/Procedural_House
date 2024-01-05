using UnityEngine;

namespace PersistentObjects.Scripts
{
    public abstract class SpawnZone : MonoBehaviour
    {
        public abstract Vector3 SpawnPoint { get; }
        
    }
}