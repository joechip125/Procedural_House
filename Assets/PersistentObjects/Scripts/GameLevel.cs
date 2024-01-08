using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class GameLevel : MonoBehaviour
    {
        [SerializeField]
        SpawnZone spawnZone;
        
        public static GameLevel Current { get; private set; }

        void Start ()
        {
            Current = this;
            Game.Instance.SpawnZoneOfLevel = spawnZone;
        }
    }
}