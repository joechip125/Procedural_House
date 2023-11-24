using System;
using UnityEngine;

namespace CustomGraph
{
    public class Graph : MonoBehaviour
    {
        [SerializeField]
        Transform pointPrefab;

        [SerializeField, Range(10,200)] 
        private int resolution;
        
        
        private void Awake()
        {
          var point =  Instantiate(pointPrefab);
          var position = Vector3.zero;
          var scale = Vector3.one / 5f;

          for (int i = 0; i < resolution; i++)
          {
              position.x = (i + 0.5f) / 5f - 1f;
              position.y = position.x;
              point.localPosition = position;
              point.localScale = scale;
          }
        }
    }
}