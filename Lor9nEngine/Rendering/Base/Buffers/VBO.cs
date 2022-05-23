using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Base.Buffers
{
    public class VBO : IBufferVerticesObject
    {
        private int _handle;

        public int Handle { get => _handle; set => _handle = value; }

        public void Dispose() => EngineGL.Instance.DeleteBuffer(this);
        public IBufferVerticesObject Setup(Vertex[] vertices)
        {
            EngineGL.Instance.GenBuffer(out _handle)
                    .BindBuffer(BufferTarget.ArrayBuffer, this)
                    .BufferData(BufferTarget.ArrayBuffer, vertices.Length * Vertex.Size, vertices, BufferUsageHint.StaticDraw);
            return this;
        }
        public IBufferVerticesObject Setup(int[] indices) => throw new NotImplementedException();
        public IBufferVerticesObject Setup(Vertex[] vertices, int[] indices) => throw new NotImplementedException();

        public void Bind(BufferTarget target) => EngineGL.Instance.BindBuffer(target, this);

        public void Unbind(BufferTarget target) => EngineGL.Instance.UnbindBuffer(target);
        public void Unbind() => throw new NotImplementedException();

        public void Bind() => throw new NotImplementedException();

        public IBufferObject Setup()
        {
            throw new NotImplementedException();
        }
    }
}
