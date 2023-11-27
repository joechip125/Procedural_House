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
            var step = 2f / resolution;
            float v = 0.5f * step - 1f;
            FunctionLibrary.Function f = FunctionLibrary.GetFunction(functionName);;
            for (int i = 0, x = 0, z = 0; i < points.Length; i++, x++)
            {
                if (x == resolution)
                {
                    x = 0;
                    z++;
                    v = (z + 0.5f) * step - 1f;
                }
                var u = (x + 0.5f) * step - 1f;
                points[i].localPosition = f(u, v, time);
                
                var point = points[i];
                var position = point.localPosition;
                FunctionLibrary.GetFunction(functionName);
                position = f(position.x,position.z, time);
                point.localPosition = position;
            }
        }

        private void Awake()
        {
          var step = 2f / resolution;
          var scale = Vector3.one *step;
          points = new Transform[resolution * resolution];

          for (int i = 0; i < points.Length; i++)
          {
              var point = points[i] = Instantiate(pointPrefab, transform, false);
              point.localScale = scale;
          }
        }
    }
}