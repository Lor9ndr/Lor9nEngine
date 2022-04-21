using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;
using OpenTK.Mathematics;
using System.Text.Json.Serialization;

namespace Lor9nEngine.GameObjects.Lights
{
    [Serializable]

    internal class DirectLight : BaseLight
    {
        
        private string _name = "directLight.";

        [JsonConstructor]
        public DirectLight(LightData lightData, Vector3 direction, Model model)
            :base(lightData, model)
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
