using Lor9nEngine.Rendering;

namespace Lor9nEngine.GameObjects.Lights
{

    internal interface ILight : IGameObject
    {
        public void RenderLight(Shader shader);
    }
}
