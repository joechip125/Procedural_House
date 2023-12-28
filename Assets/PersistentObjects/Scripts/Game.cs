using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PersistentObjects.Scripts
{
    public class Game : MonoBehaviour
    {
        public Transform prefab;

        public KeyCode createKey = KeyCode.C;

        private void Update()
        {
            if (Input.GetKeyDown(createKey))
            {
                CreateObject();
            }
        }

        private void CreateObject()
        {
            var t =Instantiate(prefab, transform);
            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
        }
    }
}