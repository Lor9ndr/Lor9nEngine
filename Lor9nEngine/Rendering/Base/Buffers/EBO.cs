using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Base.Buffers
{
    internal class EBO : IBufferVerticesObject
    {
        private int _handle;

        public int Handle { get => _handle; set => _handle = value; }

        public void Bind() => throw new NotImplementedException();

        public void Dispose() => EngineGL.Instance.DeleteBuffer(this);
        public void Unbind() => throw new NotImplementedException();
        public IBufferVerticesObject Setup(Vertex[] vertices) => throw new NotImplementedException();

        public IBufferVerticesObject Setup(Vertex[] vertices, int[] indices) => throw new NotImplementedException();

        public IBufferVerticesObject Setup(int[] indices)
        {
            EngineGL.Instance.GenBuffer(out _handle)
               .BindBuffer(BufferTarget.ElementArrayBuffer, this)
               .BufferData(BufferTarget.ElementArrayBuffer, (IntPtr)(indices.Length * sizeof(int)), indices, BufferUsageHint.StaticDraw);
            return this;
        }

        public void Bind(BufferTarget target) => EngineGL.Instance.BindBuffer(target, this);

        public void Unbind(BufferTarget target) => EngineGL.Instance.UnbindBuffer(target);

        public IBufferObject Setup()
        {
            throw new NotImplementedException();
        }
    }
}
