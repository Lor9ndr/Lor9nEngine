using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;

using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects.Lights
{
    public class SpotLight : BaseLight, IConstantableLight
    {
        private static int id;
        private int _currentID;
        private float _cutOff;
        private float _outerCutOff;

        public float CutOff { get => MathF.Cos(MathHelper.DegreesToRadians(_cutOff)); set => _cutOff = value; }
        public float OuterCutOff { get => MathF.Cos(MathHelper.DegreesToRadians(_outerCutOff)); set => _outerCutOff = value; }
        public LightConstants LightConstants { get; set; }

        public string PositionProperty;
        public string CutOffProperty;
        public string OuterCutOffProperty;
        public string DirectionProperty;

        public SpotLight(LightConstants constants, LightData lightData, Model model, float cutOff = 12.5f, float outerCutOff = 20.0f)
            : base(lightData, model)
        {
            LightConstants = constants;
            _currentID = id;
            id++;
            _cutOff = cutOff;
            _outerCutOff = outerCutOff;
            _name = $"spotLights[{_currentID}].";

            CutOffProperty = _name + "cutOff";
            OuterCutOffProperty = _name + "outerCutOff";
            DirectionProperty = _name + "direction";
            PositionProperty = _name + "position";

            LightData.Setup(_name);
            Setup();
        }
        public override void Setup()
        {
            Shadow = new DirectShadow(this);
            Shadow.Setup();
            RecreateProjection();
        }
        public override void RecreateProjection()
        {
            Projection = Matrix4.CreatePerspectiveFieldOfView(CutOff, 1, Shadow.Near, Shadow.Far);
        }


        public override void RenderLight(Shader shader)
        {
            EngineGL.Instance.UseShader(shader)
                .SetShaderData(CutOffProperty, CutOff)
                .SetShaderData(OuterCutOffProperty, OuterCutOff)
                .SetShaderData(DirectionProperty, Transform.Direction)
                .SetShaderData(PositionProperty, Transform.Position);
            LightData.Render(shader);
            LightConstants.Render(shader, _name);
            Shadow.Render(shader);
        }
    }
}
