using Lor9nEngine.Components;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Interfaces;

namespace Lor9nEngine.GameObjects
{
    internal interface IGameObject : IRenderable, IUpdatable, IDisposable
    {
        public Model Model { get;set;}
        public List<IGameObject> Childrens { get; set; }
        public IGameObject Parent { get; set; }
        public ITransform Transform { get; set; }
    }
}
