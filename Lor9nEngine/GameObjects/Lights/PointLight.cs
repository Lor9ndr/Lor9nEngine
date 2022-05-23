using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;

using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects.Lights
{
    public class PointLight : BaseLight, IConstantableLight
    {
        private static int id;
        private int _currentID;

        public string PositionProperty;
        public LightConstants LightConstants { get; set; }

        public PointLight(LightConstants constants, LightData lightData, Model model) : base(lightData, model)
        {
            LightConstants = constants;
            _currentID = id;
            id++;
            _name = $"pointLights[{_currentID}].";
            PositionProperty = _name + "position";
            lightData.Setup(_name);
            Setup();

        }
        public override void Setup()
        {
            Shadow = new PointShadow(this);
            Shadow.Setup();
            RecreateProjection();
        }
        public override void RecreateProjection()
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(MathHelper.DegreesToRadians(90), 1.0f, Shadow.Near, Shadow.Far);
        }

        public override void RenderLight(Shader shader)
        {
            EngineGL.Instance.UseShader(shader)
                            .SetShaderData(PositionProperty, Transform.Position);
            Shadow.Render(shader);

            LightData.Render(shader);
            LightConstants.Render(shader, _name);
        }
    }
}
