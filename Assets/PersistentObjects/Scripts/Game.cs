using System;
using UnityEngine;

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
            Instantiate(prefab, transform);
        }
    }
}