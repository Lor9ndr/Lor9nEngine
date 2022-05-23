
using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;

using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects.Lights
{

    public class DirectLight : BaseLight
    {
        public string DirectionProperty;
        public string PositionProperty;
        public DirectLight(LightData lightData, Vector3 direction, Model model)
            : base(lightData, model)
        {
            _name = "directLight.";
            Transform.Direction = direction;
            DirectionProperty = _name + "direction";
            PositionProperty = _name + "position";
            lightData.Setup(_name);
            Setup();
        }
        public override void Setup()
        {
            Shadow = new DirectShadow(this);
            Shadow.Setup();
            RecreateProjection();
        }

        public override void RecreateProjection()
            => Projection = Matrix4.CreateOrthographicOffCenter(-Shadow.Far, Shadow.Far, -Shadow.Far, Shadow.Far, Shadow.Near, Shadow.Far);

        public override void RenderLight(Shader shader)
        {
            LightData.Render(shader);
            Shadow.Render(shader);
            EngineGL.Instance.UseShader(shader)
               .SetShaderData(DirectionProperty, Transform.Direction)
               .SetShaderData(PositionProperty, Transform.Position);
        }

    }
}
