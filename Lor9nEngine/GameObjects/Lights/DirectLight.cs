using System.Text.Json.Serialization;

using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;

using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects.Lights
{

    internal class DirectLight : BaseLight
    {

        private string _name = "directLight.";

        public DirectLight(LightData lightData, Vector3 direction, Model model)
            : base(lightData, model)
        {
            Transform.Direction = direction;

        }
        public override void RenderLight(Shader shader)
        {
            LightData.Render(shader, _name);
            EngineGL.Instance.UseShader(shader)
               .SetShaderData($"{_name}direction", Transform.Direction)
               .SetShaderData($"{_name}position", Transform.Position);
        }

    }
}
