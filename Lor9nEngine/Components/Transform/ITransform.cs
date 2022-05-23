using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Mathematics;

namespace Lor9nEngine.Components.Transform
{

    public interface ITransform : IComponent, IUpdatable
    {
        public Vector3 Position { get; set; }
        public Vector3 Direction { get; set; }
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Matrix4 Model { get; set; }
    }
}
