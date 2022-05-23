using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Mathematics;

namespace Lor9nEngine.Components.Light
{

    public class LightData : IComponent
    {
        public Vector3 Ambient { get; set; }
        public Vector3 Diffuse { get; set; }
        public Vector3 Specular { get; set; }
        public Vector3 Color { get; set; }
        public float Intensity { get; set; } = 10.0f;

        public string ambientProperty;
        public string diffuseProperty;
        public string specularProperty;
        public string colorProperty;
        public string intensityProperty;
        public LightData(Vector3 ambient, Vector3 diffuse, Vector3 specular, Vector3 color)
        {
            Ambient = ambient;
            Diffuse = diffuse;
            Specular = specular;
            Color = color;
        }

        public void Setup(string name)
        {
            ambientProperty = name + "ambient";
            diffuseProperty = name + "diffuse";
            specularProperty = name + "specular";
            colorProperty = name + "color";
            intensityProperty = name + "intensity";
        }

        public void Render(Shader shader)
        {
            EngineGL.Instance.UseShader(shader)
                .SetShaderData(ambientProperty, Ambient)
                .SetShaderData(diffuseProperty, Diffuse)
                .SetShaderData(specularProperty, Specular)
                .SetShaderData(colorProperty, Color)
                .SetShaderData(intensityProperty, Intensity);
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

        void IRenderable.RenderWithOutTextures(Shader shader)
        {
            throw new NotImplementedException();
        }
    }
}
