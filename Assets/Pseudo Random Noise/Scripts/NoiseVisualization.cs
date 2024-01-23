using RandomNoise;
using Unity.Burst;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

namespace Pseudo_Random_Noise.Scripts
{
	public class NoiseVisualization : Visualization
	{

		[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
		struct HashJob : IJobFor 
		{

			[ReadOnly]
			public NativeArray<float3x4> positions;

			[WriteOnly]
			public NativeArray<uint4> hashes;

			public SmallXXHash4 hash;

			public float3x4 domainTRS;

			float4x3 TransformPositions (float3x4 trs, float4x3 p) => float4x3(
				trs.c0.x * p.c0 + trs.c1.x * p.c1 + trs.c2.x * p.c2 + trs.c3.x,
				trs.c0.y * p.c0 + trs.c1.y * p.c1 + trs.c2.y * p.c2 + trs.c3.y,
				trs.c0.z * p.c0 + trs.c1.z * p.c1 + trs.c2.z * p.c2 + trs.c3.z
			);

			public void Execute(int i) 
			{
				float4x3 p = TransformPositions(domainTRS, transpose(positions[i]));

				int4 u = (int4)floor(p.c0);
				int4 v = (int4)floor(p.c1);
				int4 w = (int4)floor(p.c2);

				hashes[i] = hash.Eat(u).Eat(v).Eat(w);
			}
		}
		
		private static int
			noiseId = Shader.PropertyToID("_Noise");
		
		[SerializeField]
		int seed;

		[SerializeField]
		SpaceTRS domain = new SpaceTRS 
		{
			scale = 8f
		};

		NativeArray<uint4> noise;
		ComputeBuffer noiseBuffer;
		
		protected override void EnableVisualization(int dataLength, MaterialPropertyBlock propertyBlock)
		{
			noise = new NativeArray<uint4>(dataLength, Allocator.Persistent);
			noiseBuffer = new ComputeBuffer(dataLength * 4,4);
			propertyBlock.SetBuffer(noiseId, noiseBuffer);
		}

		protected override void DisableVisualization()
		{
			noise.Dispose();
			noiseBuffer.Release();
			noiseBuffer = null;
		}

		protected override void UpdateVisualization(NativeArray<float3x4> positions, int resolution, JobHandle handle)
		{
			new HashJob {
				positions = positions,
				hashes = noise,
				hash = SmallXXHash.Seed(seed),
				domainTRS = domain.Matrix
			}.ScheduleParallel(noise.Length, resolution, handle).Complete();

			noiseBuffer.SetData(noise.Reinterpret<uint>(4 * 4));
		}
	}
}