﻿using System;
using UnityEngine;

namespace RobotGame.Scripts
{
    public class AssetManager : MonoBehaviour
    {
        private GameObject[] objects;

        [SerializeField] 
        private GameObject prototype;

        [SerializeField, Range(5,100)] 
        private int maxObjects = 10;
        
        private void OnEnable()
        {
            objects = new GameObject[maxObjects];
            
            for (int i = 0; i < maxObjects; i++)
            {
                var temp = Instantiate(prototype);
                objects[i] = temp;
                temp.SetActive(false);
            }
        }
    }
}