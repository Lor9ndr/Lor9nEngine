using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Components
{
    internal interface IComponent : IRenderable
    {
        new public void Render()
        {
            throw new NotImplementedException();
        }

        new public void Render(Shader shader)
        {
            throw new NotImplementedException();
        }

        new public void Render(PrimitiveType type)
        {
            throw new NotImplementedException();
        }
    }
}