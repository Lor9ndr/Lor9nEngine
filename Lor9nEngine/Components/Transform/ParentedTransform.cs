using Lor9nEngine.GameObjects;
using Lor9nEngine.Rendering;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lor9nEngine.Components
{
    internal class ParentedTransform : IParentedTransform
    {
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;
        private Matrix4 _model;
        private Vector3 _direction;
        private readonly IGameObject _parent;

        public Vector3 Position { get => _position; set => _position = value; }
        public Vector3 Direction { get => _direction; set => _direction = value; }
        public Vector3 Rotation { get => _rotation; set => _rotation = value; }
        public Vector3 Scale { get => _scale; set => _scale = value; }
        public Matrix4 Model { get => _model; set => _model = value; }
        public IGameObject Parent => _parent;
        public ParentedTransform(Vector3 position,IGameObject parent)
        {
            _position = position;
            _scale = new Vector3(1);
            _rotation = default;
            _model = Matrix4.Identity;
            _parent = parent;
            _direction = Vector3.UnitZ;
        }



        public ParentedTransform(Vector3 position, IGameObject parent, Vector3 rotation)
            : this(position, parent) => _rotation = rotation;

        public ParentedTransform(Vector3 position, IGameObject parent, Vector3 rotation, Vector3 scale)
            : this(position, parent, rotation) => _scale = scale;

        [JsonConstructor]
        public ParentedTransform(ITransform transform, IGameObject parent)
        {
            _position = transform.Position;
            _scale = transform.Scale;
            _rotation = transform.Rotation;
            _model = transform.Model;
            _parent = parent;
        }

        public void Render(Shader shader)
            => EngineGL.Instance.UseShader(shader)
                            .SetShaderData("model", Model);

        public void CreateRotation(Vector3 rotation) => Rotation += rotation;

        public void CreateScale(Vector3 scale) => Scale += scale;

        public void CreateTranslation(Vector3 transform) => Position += transform;

        public void Render() => throw new NotImplementedException();


        public async Task UpdateAsync()
        {
            var t1 = Matrix4.CreateTranslation(Position);
            var r1 = Matrix4.CreateRotationX(Rotation.X);
            var r2 = Matrix4.CreateRotationY(Rotation.Y);
            var r3 = Matrix4.CreateRotationZ(Rotation.Z);
            var s = Matrix4.CreateScale(Scale);
            Model = Parent.Transform.Model *  (r1 * r2 * r3 * s * t1);
            await Task.CompletedTask;
        }

        public void Update()
        {
          
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
