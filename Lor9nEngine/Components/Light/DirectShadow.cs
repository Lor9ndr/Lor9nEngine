using Lor9nEngine.GameObjects;
using Lor9nEngine.GameObjects.Lights;
using Lor9nEngine.Rendering;

using OpenTK.Mathematics;

namespace Lor9nEngine.Components.Light
{
    public class DirectShadow : Shadow
    {
        public static Shader DepthDirectShader = new Shader(Game.SHADOW_SHADERS_PATH + "Depth.vert", Game.SHADOW_SHADERS_PATH + "Depth.frag");
        private string MatrixProperty;

        public Matrix4 LightSpaceMatrix { get; set; }

        public DirectShadow(BaseLight light) : base(GetDefaultFrameBuffer(), light)
        {
            MatrixProperty = light.Name + nameof(LightSpaceMatrix);
        }

        public override void RenderDepth(IEnumerable<IGameObject> gameObjects)
        {
            FBO.Activate();
            EngineGL.Instance.UseShader(DepthDirectShader)
                .SetShaderData(nameof(LightSpaceMatrix), LightSpaceMatrix);
            EngineGL.Instance.SetShaderData("lightPos", _attachedLight.Transform.Position)
             .SetShaderData("far_plane", Far)
             .SetShaderData("model", _attachedLight.Transform.Model);
            foreach (var item in gameObjects)
            {
                item.RenderWithOutTextures(DepthDirectShader);
            }
            FBO.Unbind();
        }
        public override void Render(Shader shader)
        {
            base.Render(shader);
            EngineGL.Instance.SetShaderData(MatrixProperty, LightSpaceMatrix);
        }
        public override async Task UpdateAsync()
        {
            await Task.Run(() =>
            {
                Matrix4 view = Matrix4.LookAt(_attachedLight.Transform.Position, _attachedLight.Transform.Direction, Vector3.UnitY);
                LightSpaceMatrix = view * _attachedLight.Projection;
            });
        }

    }
}
