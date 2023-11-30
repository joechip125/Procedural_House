using System;
using UnityEngine;

namespace CustomGraph
{
    public class GPUGraph : MonoBehaviour
    {
        static readonly int positionsId = Shader.PropertyToID("_Positions"),
            stepId = Shader.PropertyToID("_Steps"),
            timeId = Shader.PropertyToID("_Time"),
            resolutionId = Shader.PropertyToID("_Resolution");

        [SerializeField] 
        private Mesh mesh;
        
        [SerializeField] 
        private Material material;
        
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

            computeShader.SetBuffer(0, positionsId, positionBuffer);
            var groups = Mathf.CeilToInt(resolution / 8f);
            computeShader.Dispatch(0, groups, groups, 1);
            
            material.SetBuffer(positionsId, positionBuffer);
            material.SetFloat(stepId, step);
            var bounds = new Bounds(Vector3.zero, Vector3.one * (2f + 2f / resolution));
            Graphics.DrawMeshInstancedProcedural(mesh, 0, material, bounds, positionBuffer.count);
        }
        
        private void Update()
        {
            UpdateFunctionOnGPU();
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