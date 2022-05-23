namespace Lor9nEngine.Rendering.Interfaces
{
    public interface IRenderable : IDisposable
    {
        public void Render(Shader shader);
        public void RenderWithOutTextures(Shader shader);
    }
}
