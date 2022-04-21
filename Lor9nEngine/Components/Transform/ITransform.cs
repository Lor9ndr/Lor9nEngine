using Lor9nEngine.Rendering.Interfaces;
using OpenTK.Mathematics;
using System.Runtime.Serialization;

namespace Lor9nEngine.Components
{

    internal interface ITransform : IComponent,IUpdatable
    {
        public Vector3 Position { get;set;}
        public Vector3 Direction { get; set; } 
        public Vector3 Rotation { get; set; }
        public Vector3 Scale { get; set; }
        public Matrix4 Model { get; set; }
    }
}
