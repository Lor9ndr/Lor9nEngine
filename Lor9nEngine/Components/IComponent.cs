using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Interfaces;

namespace Lor9nEngine.Components
{
    public interface IComponent : IRenderable
    {
        new public void Render(Shader shader)
        {
            throw new NotImplementedException();
        }
    }
}