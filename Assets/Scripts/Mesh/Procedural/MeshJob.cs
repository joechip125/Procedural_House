
using Unity.Burst;
using Unity.Jobs;

[BurstCompile(FloatPrecision.Standard, FloatMode.Fast, CompileSynchronously = true)]
public struct MeshJob<G, S> : IJobFor
    where G : struct, IMeshGenerator
    where S : struct, IMeshStreams
{
    G generator;
    S streams;

    public void Execute (int i) => generator.Execute(i, streams);
}