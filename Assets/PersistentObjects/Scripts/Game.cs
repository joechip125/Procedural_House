﻿using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using Random = UnityEngine.Random;

namespace PersistentObjects.Scripts
{
    public class Game : MonoBehaviour
    {
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
                Save();
            }
            else if (Input.GetKeyDown(loadKey))
            {
                Load();
            }
        }
        
        private void Save()
        {
            using (var writer = new BinaryWriter(File.Open(savePath, FileMode.Create)))
            {
                writer.Write(objects.Count);

                for (int i = 0; i < objects.Count; i++)
                {
                    var t = objects[i].transform;
                    writer.Write(t.localPosition.x);
                    writer.Write(t.localPosition.y);
                    writer.Write(t.localPosition.z);
                }
            }
        }
        
        void Load () 
        {
            BeginNewGame();
            using (var reader = new BinaryReader(File.Open(savePath, FileMode.Open)))
            {
                int count = reader.ReadInt32();
                for (int i = 0; i < count; i++) 
                {
                    Vector3 p;
                    p.x = reader.ReadSingle();
                    p.y = reader.ReadSingle();
                    p.z = reader.ReadSingle();
                    var t = Instantiate(prefab);
                    t.transform.localPosition = p;
                    objects.Add(t);
                }
                
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