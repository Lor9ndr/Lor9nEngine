using Lor9nEngine.Rendering.Interfaces;

namespace Lor9nEngine.Rendering.Base
{
    internal class BaseGLObject : IRenderable
    {
        public IObjectSetupper ObjectSetupper;

        public BaseGLObject(Vertex[] vertices, int[]? indices = default)
        {
            if (indices != default)
            {
                ObjectSetupper = new IndicedObjectSetupper(vertices, indices);
            }
            else
            {
                ObjectSetupper = new ObjectSetupper(vertices);
            }
        }
      
        public BaseGLObject(IObjectSetupper objectSetupper) => ObjectSetupper = objectSetupper;

        public virtual void Render(Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            ObjectSetupper.Render(shader);
        }

        public virtual void Dispose()
        {
            ObjectSetupper.Dispose();
        }
    }
}
