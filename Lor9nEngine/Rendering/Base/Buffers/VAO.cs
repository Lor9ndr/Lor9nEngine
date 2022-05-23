using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Base.Buffers
{
    public class VAO : IBufferVerticesObject
    {
        private int _handle;

        public int Handle { get => _handle; set => _handle = value; }

        public void Bind() => EngineGL.Instance.BindVAO(this);

        public void Dispose()
        {
            EngineGL.Instance.UnbindVAO().DeleteVAO(this);
            Handle = 0;
        }
        public void Unbind() => EngineGL.Instance.UnbindVAO();
        public IBufferVerticesObject Setup(Vertex[]? vertices = null)
        {
            throw new NotImplementedException();
        }

        public IBufferVerticesObject Setup(Vertex[] vertices, int[] indices) => throw new NotImplementedException();
        public IBufferVerticesObject Setup(int[] indices) => throw new NotImplementedException();

        public void Bind(BufferTarget target) => throw new NotImplementedException();

        public void Unbind(BufferTarget target) => throw new NotImplementedException();
        public static bool operator ==(VAO vao1, VAO vao2)
        {
            return vao1.Equals(vao2.Handle);
        }
        public static bool operator !=(VAO vao1, VAO vao2)
        {
            return !vao1.Equals(vao2.Handle);
        }

        public override bool Equals(object obj)
        {
            return obj is VAO && Handle.Equals(((VAO)obj).Handle);
        }

        public override int GetHashCode() => Handle.GetHashCode();

        public IBufferObject Setup()
        {
            EngineGL.Instance.GenVertexArray(out _handle).BindVAO(this);
            return this;
        }
    }
}
