using Lor9nEngine.Components.Transform;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Interfaces;

namespace Lor9nEngine.GameObjects
{
    public interface IGameObject : IRenderable, IUpdatable, IDisposable
    {
        Model Model { get; set; }
        List<IGameObject> Childrens { get; set; }
        IGameObject? Parent { get; set; }
        ITransform Transform { get; set; }
        public void RenderModel(Shader shader);

    }
}
