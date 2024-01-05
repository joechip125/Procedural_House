using UnityEngine;

namespace PersistentObjects.Scripts
{
    public class GameLevel : MonoBehaviour
    {
        [SerializeField]
        SpawnZone spawnZone;

        void Start () 
        {
            Game.Instance.SpawnZoneOfLevel = spawnZone;
        }
    }
}