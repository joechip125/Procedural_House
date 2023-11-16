using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Rendering;

public class SingleStream : IMeshStreams
{
    public void Setup(Mesh.MeshData meshData, int vertexCount, int indexCount)
    {
        var descriptor = new NativeArray<VertexAttributeDescriptor>(
            4, Allocator.Temp, NativeArrayOptions.UninitializedMemory
        );
        descriptor[0] = new VertexAttributeDescriptor(dimension: 3);
        descriptor[1] = new VertexAttributeDescriptor(VertexAttribute.Normal, dimension: 3);
        descriptor[2] = new VertexAttributeDescriptor(VertexAttribute.Tangent, dimension: 4);
        descriptor[3] = new VertexAttributeDescriptor(VertexAttribute.TexCoord0, dimension: 2);
        
        meshData.SetVertexBufferParams(vertexCount, descriptor);
        descriptor.Dispose();

        meshData.SetIndexBufferParams(indexCount, IndexFormat.UInt32);
			
        meshData.subMeshCount = 1;
        meshData.SetSubMesh(0, new SubMeshDescriptor(0, indexCount));
    }

    public void SetVertex(int index, Vertex data)
    {
        throw new System.NotImplementedException();
    }

    public void SetTriangle(int index, int3 triangle)
    {
        throw new System.NotImplementedException();
    }
}
