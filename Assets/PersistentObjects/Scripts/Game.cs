using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace PersistentObjects.Scripts
{
    public class Game : PersistableObject
    {
        [SerializeField] Slider creationSpeedSlider;
        [SerializeField] Slider destructionSpeedSlider;
        
        [SerializeField]
        private ShapeFactory shapeFactory;

        [SerializeField]
        private PersistentStorage storage;

        const int saveVersion = 3;

        [SerializeField]
        private KeyCode createKey = KeyCode.C;
        [SerializeField]
        private KeyCode newGameKey = KeyCode.N;
        [SerializeField]
        private KeyCode saveKey = KeyCode.S;
        [SerializeField]
        private KeyCode loadKey = KeyCode.L;
        [SerializeField]
        private KeyCode destroyKey = KeyCode.X;
        [SerializeField]
        private int levelCount;
        int loadedLevelBuildIndex;

        [SerializeField] 
        private bool reseedOnLoad;

        public SpawnZone SpawnZoneOfLevel{get; set; }

        private List<Shape> shapes;
        private string savePath;
        
        public float CreationSpeed { get; set; }
        public float DestructionSpeed { get; set; }
        
        float creationProgress, destructionProgress;
        
        private Random.State mainRandomState;

        private void Start()
        {
            BeginNewGame();
            shapes = new List<Shape>();
            savePath = Path.Combine(Application.persistentDataPath, "saveFile");

            if (Application.isEditor)
            {
                for (int i = 0; i < SceneManager.sceneCount; i++) 
                {
                    Scene loadedScene = SceneManager.GetSceneAt(i);
                    if (loadedScene.name.Contains("Level ")) 
                    {
                        SceneManager.SetActiveScene(loadedScene);
                        loadedLevelBuildIndex = loadedScene.buildIndex;
                        return;
                    }
                }
            }

            StartCoroutine(LoadLevel(1));
        }

        private void Update()
        {
            if (Input.GetKeyDown(createKey))
            {
                CreateShape();
            }
            else if (Input.GetKeyDown(newGameKey))
            {
                BeginNewGame();
                StartCoroutine(LoadLevel(loadedLevelBuildIndex));
            }
            else if (Input.GetKeyDown(saveKey))
            {
                storage.Save(this, saveVersion);
            }
            else if (Input.GetKeyDown(loadKey))
            {
                BeginNewGame();
                storage.Load(this);
            }
            else if (Input.GetKeyDown(destroyKey))
            {
               DestroyShape();
            }
            
            else 
            {
                for (int i = 1; i <= levelCount; i++) 
                {
                    if (Input.GetKeyDown(KeyCode.Alpha0 + i)) 
                    {
                        BeginNewGame();
                        StartCoroutine(LoadLevel(i));
                        return;
                    }
                }
            }

        }

        private void FixedUpdate()
        {
            creationProgress += Time.deltaTime * CreationSpeed;
            while (creationProgress >= 1f) 
            {
                creationProgress -= 1f;
                CreateShape();
            }

            destructionProgress += Time.deltaTime * DestructionSpeed;
            while (destructionProgress >= 1f) 
            {
                destructionProgress -= 1f;
                DestroyShape();
            }
        }

        void DestroyShape () 
        {
            if (shapes.Count > 0) 
            {
                int index = Random.Range(0, shapes.Count);
                shapeFactory.Reclaim(shapes[index]);
                int lastIndex = shapes.Count - 1;
                shapes[index] = shapes[lastIndex];
                shapes.RemoveAt(lastIndex);
            }
        }

        public override void Save (GameDataWriter writer) 
        {
            writer.Write(shapes.Count);
            writer.Write(Random.state);
            writer.Write(CreationSpeed);
            writer.Write(creationProgress);
            writer.Write(DestructionSpeed);
            writer.Write(destructionProgress);
            writer.Write(loadedLevelBuildIndex);
            for (int i = 0; i < shapes.Count; i++) 
            {
                writer.Write(shapes[i].ShapeID);
                writer.Write(shapes[i].MaterialId);
                shapes[i].Save(writer);
            }
        }


        public override void Load (GameDataReader reader)
        {
            int version = reader.Version;
            if (version > saveVersion) 
            {
                Debug.LogError("Unsupported future save version " + version);
                return;
            }
            StartCoroutine(LoadGame(reader));
        }

        IEnumerator LoadGame(GameDataReader reader)
        {
            var version = reader.Version;
            var count = version <= 0 ? -version : reader.ReadInt();

            if (version >= 3) 
            {
                var state = reader.ReadRandomState();
                if (!reseedOnLoad) 
                {
                    Random.state = state;
                }
                creationSpeedSlider.value = CreationSpeed = reader.ReadFloat();
                creationProgress = reader.ReadFloat();
                destructionSpeedSlider.value = DestructionSpeed = reader.ReadFloat();
                destructionProgress = reader.ReadFloat();
            }
            
            yield return LoadLevel(version < 2 ? 1 : reader.ReadInt());
            if (version >= 3) 
            {
                GameLevel.Current.Load(reader);
            }

            for (int i = 0; i < count; i++) 
            {
                int shapeId = version > 0 ? reader.ReadInt() : 0;
                int materialId = version > 0 ? reader.ReadInt() : 0;
                Shape instance = shapeFactory.Get(shapeId, materialId);
                instance.Load(reader);
                shapes.Add(instance);
            }
            
        }
        
        IEnumerator LoadLevel (int levelBuildIndex)
        {
            enabled = false;
            if (loadedLevelBuildIndex > 0) 
            {
                yield return SceneManager.UnloadSceneAsync(loadedLevelBuildIndex);
            }
            loadedLevelBuildIndex = levelBuildIndex;
            yield return SceneManager.LoadSceneAsync(levelBuildIndex, LoadSceneMode.Additive);
            SceneManager.SetActiveScene(SceneManager.GetSceneByBuildIndex(levelBuildIndex));
            enabled = true;
        }

        private void BeginNewGame()
        {
            creationSpeedSlider.value = CreationSpeed = 0;
            destructionSpeedSlider.value = DestructionSpeed = 0;

            Random.state = mainRandomState;
                                                    //bitwise exclusive or
            int seed = Random.Range(0, int.MaxValue) ^ (int)Time.unscaledTime;
            mainRandomState = Random.state;
            Random.InitState(seed);
            
            for (int i = 0; i < shapes.Count; i++)
            {
                shapeFactory.Reclaim(shapes[i]);
            }
        }

        private void CreateShape()
        {
            var instance = shapeFactory.GetRandom();
            var t = instance.transform;
            t.localPosition = GameLevel.Current.SpawnPoint;
            t.localRotation = Random.rotation;
            t.localScale = Vector3.one * Random.Range(0.1f, 1f);
            instance.SetColor(Random.ColorHSV(0f, 1f, 0.5f, 1f, 0.25f, 1f, 1f, 1f));
            shapes.Add(instance);
        }
    }
}