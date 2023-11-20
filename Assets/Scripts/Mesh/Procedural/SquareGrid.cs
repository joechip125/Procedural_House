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
        
        streams.SetVertex(0, vertex);
        
        vertex.position = right();
        vertex.texCoord0 = float2(1f, 0f);
        streams.SetVertex(1, vertex);

        vertex.position = up();
        vertex.texCoord0 = float2(0f, 1f);
        streams.SetVertex(2, vertex);

        vertex.position = float3(1f, 1f, 0f);
        vertex.texCoord0 = 1f;
        streams.SetVertex(3, vertex);
    }

    public int VertexCount => 4;
    public int IndexCount => 6;
    public int JobLength => 1;
}
