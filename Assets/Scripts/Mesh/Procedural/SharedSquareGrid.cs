using Unity.Mathematics;
using UnityEngine;
using static Unity.Mathematics.math;

public struct SharedSquareGrid : IMeshGenerator
{
    public int VertexCount => 4 * Resolution * Resolution;
    public int IndexCount => 6 * Resolution * Resolution;
    public int JobLength => Resolution;
    public Bounds Bounds => new Bounds(Vector3.zero, new Vector3(1f, 0f, 1f));
    public int Resolution { get; set; }

    public void Execute<S>(int z, S streams) where S : struct, IMeshStreams
    {
        int vi = (Resolution + 1) * z, ti = 2 * Resolution * z;
        
        var vertex = new Vertex();
        vertex.normal.y = 1f;
        vertex.tangent.xw = float2(1f, -1f);
        
        for (int x = 0; x < Resolution; x++, vi += 4, ti += 2) 
        {
            
        }
    }
}
