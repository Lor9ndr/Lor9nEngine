using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;

namespace Lor9nEngine.GameObjects.Lights
{
    internal class PointLight : BaseLight
    {
        private static int id;
        private int _currentID;
        private string _name;
        public LightConstants LightConstants { get; set; }
        public PointLight(LightConstants constants, LightData lightData, Model model) : base(lightData, model)
        {
            LightConstants = constants;
            _currentID = id;
            id++;
            _name = $"pointLights[{_currentID}].";
        }
        public override void RenderLight(Shader shader)
        {
            EngineGL.Instance.UseShader(shader)
                            .SetShaderData($"{_name}position", Transform.Position);
            LightData.Render(shader, _name);
            LightConstants.Render(shader, _name);
        }

    }
}
