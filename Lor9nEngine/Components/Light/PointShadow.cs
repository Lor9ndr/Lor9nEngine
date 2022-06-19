
using Lor9nEngine.GameObjects;
using Lor9nEngine.GameObjects.Lights;
using Lor9nEngine.Rendering;

using OpenTK.Mathematics;

namespace Lor9nEngine.Components.Light
{
    public class PointShadow : Shadow
    {
        public static Shader DepthPointShader = new Shader(Game.SHADOW_SHADERS_PATH + "DepthCubeMap.vert", Game.SHADOW_SHADERS_PATH + "DepthCubeMap.frag", Game.SHADOW_SHADERS_PATH + "DepthCubeMap.geom");

        public Matrix4[] LightSpaceMatrix = new Matrix4[6];

        private readonly Vector3[] _directions =
        {
            new Vector3(1.0f,  0.0f,  0.0f ),
            new Vector3(-1.0f, 0.0f,  0.0f ),
            new Vector3(0.0f,  1.0f,  0.0f ),
            new Vector3(0.0f, -1.0f,  0.0f ),
            new Vector3(0.0f,  0.0f,  1.0f ),
            new Vector3(0.0f,  0.0f, -1.0f )
        };
        private readonly Vector3[] _ups =
        {
            new Vector3(0.0f, -1.0f,  0.0f ),
            new Vector3(0.0f, -1.0f,  0.0f ),
            new Vector3(0.0f,  0.0f,  1.0f ),
            new Vector3(0.0f,  0.0f, -1.0f ),
            new Vector3(0.0f, -1.0f,  0.0f ),
            new Vector3(0.0f, -1.0f,  0.0f )
        };

        private string _matrixProperty;
        private string[] _matricesProperty = new string[6];

        public PointShadow(BaseLight light)
            : base(GetDefaultFrameBuffer(), light)
        {
            _matrixProperty = _attachedLight.Name + nameof(LightSpaceMatrix);
            for (int i = 0; i < 6; i++)
            {
                _matricesProperty[i] = $"{nameof(LightSpaceMatrix)}[{i}]";
            }
        }
        public override void RenderDepth(IEnumerable<IGameObject> gameObjects)
        {
            FBO.Activate();

            EngineGL.Instance.UseShader(DepthPointShader);
            // light space matrices
            for (int i = 0; i < 6; i++)
            {
                EngineGL.Instance.SetShaderData(_matricesProperty[i], LightSpaceMatrix[i]);
            }
            EngineGL.Instance.SetShaderData("lightPos", _attachedLight.Transform.Position)
                .SetShaderData("far_plane", Far)
                .SetShaderData("model", _attachedLight.Transform.Model);

            foreach (var item in gameObjects)
            {
                item.RenderWithOutTextures(DepthPointShader);
            }

            FBO.Texture.Unbind();
            FBO.Unbind();
        }
        public override async Task UpdateAsync()
        {
            await Task.Run(() =>
            {
                for (int i = 0; i < LightSpaceMatrix.Length; i++)
                {
                    LightSpaceMatrix[i] = Matrix4.LookAt(_attachedLight.Transform.Position,
                                                         _attachedLight.Transform.Position + _directions[i],
                                                         _ups[i]) * _attachedLight.Projection;
                }
            });
        }
    }
}
