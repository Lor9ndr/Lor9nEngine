using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Interfaces;

namespace Lor9nEngine.Components.Light
{
    internal class LightConstants : IComponent
    {
        public float Constant { get; set; }
        public float Linear { get; set; }
        public float Quadratic { get; set; }

        public LightConstants(float constant = 1.0f, float linear = 0.0014f, float quadratic = 0.07f)
        {
            Constant = constant;
            Linear = linear;
            Quadratic = quadratic;
        }
        public void Render(Shader shader, string name)
        {
            EngineGL.Instance.UseShader(shader)
                .SetShaderData(name + "constant", Constant)
                .SetShaderData(name + "linear", Linear)
                .SetShaderData(name + "quadratic", Quadratic);
        }

        void IRenderable.Render(Shader shader)
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
