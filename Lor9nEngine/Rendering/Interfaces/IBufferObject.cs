using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Interfaces
{
    public interface IBufferObject : IGLObject
    {
        public IBufferObject Setup();
        public void Bind(BufferTarget target);
        public void Unbind(BufferTarget target);
    }
}
