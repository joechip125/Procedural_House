using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PersistentObjects.Scripts
{
    public class Game : PersistableObject
    {
        
        public PersistentStorage storage;
        
        public PersistableObject prefab;

        public KeyCode createKey = KeyCode.C;
        public KeyCode newGameKey = KeyCode.N;
        public KeyCode saveKey = KeyCode.S;
        public KeyCode loadKey = KeyCode.L;
        
        private List<PersistableObject> objects;
        private string savePath;

        private void Awake()
        {
            objects = new List<PersistableObject>();
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
            else if (Input.GetKeyDown(saveKey))
            {
                storage.Save(this);
            }
            else if (Input.GetKeyDown(loadKey))
            {
                BeginNewGame();
                storage.Load(this);
            }
        }
        
        public override void Save (GameDataWriter writer) 
        {
            writer.Write(objects.Count);
            for (int i = 0; i < objects.Count; i++) 
            {
                objects[i].Save(writer);
            }
        }
        public override void Load (GameDataReader reader) 
        {
            int count = reader.ReadInt();
            for (int i = 0; i < count; i++) 
            {
                PersistableObject o = Instantiate(prefab);
                o.Load(reader);
                objects.Add(o);
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
            var o =Instantiate(prefab, transform);
            var t = o.transform;
            t.localPosition = Random.insideUnitSphere * 5f;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * Random.Range(0.1f, 1f);
            objects.Add(o);
        }
    }
}