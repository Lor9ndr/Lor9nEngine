using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;

using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects.Lights
{

    public interface ILight : IGameObject
    {
        public Matrix4 Projection { get; set; }
        public Shadow Shadow { get; set; }
        public void RenderLight(Shader shader);
        public void RenderDepth(IEnumerable<IGameObject> gameObjects);


    }
}
