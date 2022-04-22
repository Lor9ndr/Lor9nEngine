using Lor9nEngine.Rendering.Base;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface IBufferVerticesObject : IBufferObject
    {
        public IBufferVerticesObject Setup(Vertex[] vertices);
        public IBufferVerticesObject Setup(Vertex[] vertices, int[] indices);
        public IBufferVerticesObject Setup(int[] indices);
        public void Bind(BufferTarget target);
        public void Unbind(BufferTarget target);

    }
}
