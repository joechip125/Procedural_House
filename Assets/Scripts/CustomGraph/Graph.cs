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

        [SerializeField] 
        private FunctionLibrary.FunctionName functionName;

        private Transform[] points;

        private void OnValidate()
        {
            
        }

        private void Update()
        {
            var time = Time.time;
            FunctionLibrary.Function f = FunctionLibrary.GetFunction(functionName);;
            for (int i = 0; i < points.Length; i++)
            {
                var point = points[i];
                var position = point.localPosition;
                FunctionLibrary.GetFunction(functionName);
                position.y = f(position.x, time);
                point.localPosition = position;
            }
        }

        private void Awake()
        {
          float step = 2f / resolution;
          var position = Vector3.zero;
          var scale = Vector3.one *step;
          points = new Transform[resolution];

          for (int i = 0; i < points.Length; i++)
          {
              var point = points[i] = Instantiate(pointPrefab, transform, false);
              position.x = (i + 0.5f) * step - 1f;
              //position.y = position.x * position.x * position.x;
              point.localPosition = position;
              point.localScale = scale;
          }
        }
    }
}