using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PersistentObjects.Scripts
{
    public class Game : MonoBehaviour
    {
        public Transform prefab;

        public KeyCode createKey = KeyCode.C;
        public KeyCode newGameKey = KeyCode.N;

        private List<Transform> objects;
        private string savePath;

        private void Awake()
        {
            objects = new List<Transform>();
            savePath = Path.Combine(Application.persistentDataPath, "saveFile");
        }

        private void Update()
        {
            if (Input.GetKeyDown(createKey))
            {
                CreateObject();
            }
            else if (Input.GetKeyDown(newGameKey))
            {
                BeginNewGame();
            }
        }
        
        private void Save()
        {
            using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
            {
                writer.Write(objects.Count);
            }
        }
        
        private void BeginNewGame()
        {
            for (int i = 0; i < objects.Count; i++)
            {
                Destroy(objects[i].gameObject);
            }
        }

        private void CreateObject()
        {
            var t =Instantiate(prefab, transform);
            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * Random.Range(0.1f, 1f);
            objects.Add(t);
        }
    }
}