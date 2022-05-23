using Lor9nEngine.Rendering.Base;

namespace Lor9nEngine.Rendering.Interfaces
{
    public interface IBufferVerticesObject : IBufferObject
    {
        public IBufferVerticesObject Setup(Vertex[] vertices);
        public IBufferVerticesObject Setup(Vertex[] vertices, int[] indices);
        public IBufferVerticesObject Setup(int[] indices);
    }
}
