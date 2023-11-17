
public interface IMeshGenerator
{
    void Execute<S> (int i, S streams) where S : struct, IMeshStreams;
    
    int VertexCount { get; }
    int IndexCount { get; }
}
