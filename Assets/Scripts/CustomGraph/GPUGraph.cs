﻿using System;
using UnityEngine;

namespace CustomGraph
{
    public class GPUGraph : MonoBehaviour
    {
        static readonly int positionsId = Shader.PropertyToID("_Positions"),
            stepId = Shader.PropertyToID("_Steps"),
            timeId = Shader.PropertyToID("_Time"),
            resolutionId = Shader.PropertyToID("_Resolution");

        [SerializeField, Range(10,200)] 
        private int resolution;

        [SerializeField] 
        private FunctionLibrary.FunctionName functionName;

        [SerializeField] 
        private ComputeShader computeShader;

        private ComputeBuffer positionBuffer;
        
        void UpdateFunctionOnGPU () 
        {
            float step = 2f / resolution;
            computeShader.SetInt(resolutionId, resolution);
            computeShader.SetFloat(stepId, step);
            computeShader.SetFloat(timeId, Time.time);
        }
        
        private void Update()
        {
            var time = Time.time;
            var step = 2f / resolution;
            float v = 0.5f * step - 1f;
            FunctionLibrary.Function f = FunctionLibrary.GetFunction(functionName);
        }

        private void OnDisable()
        {
            positionBuffer.Release();
            positionBuffer = null;
        }

        private void OnEnable()
        {
            positionBuffer = new ComputeBuffer(resolution * resolution, 3 * 4);
        }
    }
}