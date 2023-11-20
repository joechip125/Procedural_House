using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public struct SquareGrid : IMeshGenerator
{
    public void Execute<S>(int i, S streams) where S : struct, IMeshStreams
    {
        var vertex = new Vertex();
        vertex.normal.z = -1f;
        vertex.tangent.xw = float2(1f, -1f);
    }

    public int VertexCount => 4;
    public int IndexCount { get; }
    public int JobLength => 1;
}
