using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Interfaces;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lor9nEngine.Components.Light
{
    [Serializable]

    internal class LightData : IComponent
    {

        public Vector3 Ambient { get;set;}
        public Vector3 Diffuse { get;set;}
        public Vector3 Specular { get;set;}
        public Vector3 Color { get;set;}
        public float Intensity { get;set;} = 10.0f;
        public LightData(Vector3 ambient, Vector3 diffuse, Vector3 specular, Vector3 color)
        {
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Color = color;
        }
        public void Render(Shader shader, string name)
        {
            EngineGL.Instance.UseShader(shader)
                .SetShaderData(name + "ambient", Ambient)
                .SetShaderData(name + "diffuse", Diffuse)
                .SetShaderData(name + "specular", Specular)
                .SetShaderData(name + "color", Color)
                .SetShaderData(name + "intensity", Intensity);
        }

        public static LightData Default => new LightData(new Vector3(1), new Vector3(1), new Vector3(1), new Vector3(255));


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
