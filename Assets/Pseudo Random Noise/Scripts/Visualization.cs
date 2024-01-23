using RandomNoise;
using Unity.Burst;
using UnityEngine;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;

using static Unity.Mathematics.math;

namespace Pseudo_Random_Noise.Scripts
{
    public class Visualization : MonoBehaviour
    {
	    
		public enum Shape { Plane, Sphere, Torus }

		static Shapes.ScheduleDelegate[] shapeJobs = {
			Shapes.Job<Shapes.Plane>.ScheduleParallel,
			Shapes.Job<Shapes.Sphere>.ScheduleParallel,
			Shapes.Job<Shapes.Torus>.ScheduleParallel
		};

		static int
			positionsId = Shader.PropertyToID("_Positions"),
			normalsId = Shader.PropertyToID("_Normals"),
			configId = Shader.PropertyToID("_Config");

		[SerializeField]
		Mesh instanceMesh;

		[SerializeField]
		Material material;

		[SerializeField]
		Shape shape;

		[SerializeField, Range(0.1f, 10f)]
		float instanceScale = 2f;

		[SerializeField, Range(1, 512)]
		int resolution = 16;

		[SerializeField, Range(-0.5f, 0.5f)]
		float displacement = 0.1f;
		
		NativeArray<float3x4> positions, normals;

		ComputeBuffer hashesBuffer, positionsBuffer, normalsBuffer;

		MaterialPropertyBlock propertyBlock;

		bool isDirty;

		Bounds bounds;

		void OnEnable () 
		{
			isDirty = true;

			int length = resolution * resolution;
			length = length / 4 + (length & 1);
			positions = new NativeArray<float3x4>(length, Allocator.Persistent);
			normals = new NativeArray<float3x4>(length, Allocator.Persistent);
			hashesBuffer = new ComputeBuffer(length * 4, 4);
			positionsBuffer = new ComputeBuffer(length * 4, 3 * 4);
			normalsBuffer = new ComputeBuffer(length * 4, 3 * 4);

			propertyBlock ??= new MaterialPropertyBlock();
			propertyBlock.SetBuffer(positionsId, positionsBuffer);
			propertyBlock.SetBuffer(normalsId, normalsBuffer);
			propertyBlock.SetVector(configId, new Vector4(
				resolution, instanceScale / resolution, displacement
			));
		}

		void OnDisable () 
		{
			
			positions.Dispose();
			normals.Dispose();
			hashesBuffer.Release();
			positionsBuffer.Release();
			normalsBuffer.Release();
			hashesBuffer = null;
			positionsBuffer = null;
			normalsBuffer = null;
		}

		void OnValidate ()
		{
			if (hashesBuffer != null && enabled) 
			{
				OnDisable();
				OnEnable();
			}
		}

		void Update () 
		{
			if (isDirty || transform.hasChanged) 
			{
				isDirty = false;
				transform.hasChanged = false;

				JobHandle handle = shapeJobs[(int)shape](
					positions, normals, resolution, transform.localToWorldMatrix, default
				);

				
				hashesBuffer.SetData(hashes.Reinterpret<uint>(4 * 4));
				positionsBuffer.SetData(positions.Reinterpret<float3>(3 * 4 * 4));
				normalsBuffer.SetData(normals.Reinterpret<float3>(3 * 4 * 4));

				bounds = new Bounds(
					transform.position,
					float3(2f * cmax(abs(transform.lossyScale)) + displacement)
				);
			}

			Graphics.DrawMeshInstancedProcedural(
				instanceMesh, 0, material, bounds, resolution * resolution, propertyBlock
			);
		}
    }
}