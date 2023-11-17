using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public struct SquareGrid : IMeshGenerator
{
    public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
    {
        
    }

    public int VertexCount { get; }
    public int IndexCount { get; }
    public int JobLength => 1;
}
