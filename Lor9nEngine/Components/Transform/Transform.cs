using Lor9nEngine.GameObjects;
using Lor9nEngine.Rendering;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lor9nEngine.Components
{
    [Serializable]

    internal class Transform : ITransform
    {
        private Vector3 _position;
        private Vector3 _rotation;
        private Vector3 _scale;
        private Matrix4 _model;
        private Vector3 _direction;

        public Vector3 Position { get => _position; set => _position = value; }
        public Vector3 Direction { get => _direction; set => _direction = value; }
        public Vector3 Rotation { get => _rotation; set => _rotation = value; }
        public Vector3 Scale { get => _scale; set => _scale = value; }
        public Matrix4 Model { get => _model; set => _model = value; }
        public Transform(Vector3 position)
        {
            _position = position;
            _scale = new Vector3(1);
            _rotation = default;
            _model = Matrix4.Identity;
            _direction = Vector3.UnitZ;
        }


        public Transform(Vector3 position,  Vector3 rotation)
            : this(position) => _rotation = rotation;
        [JsonConstructor]
        public Transform(Vector3 position, Vector3 rotation, Vector3 scale)
            : this(position, rotation) => _scale = scale;


        public void Render(Shader shader)
            => EngineGL.Instance.UseShader(shader)
                            .SetShaderData("model", Model);

        public void CreateRotation(Vector3 rotation) => Rotation += rotation;

        public void CreateScale(Vector3 scale) => Scale += scale;

        public void CreateTranslation(Vector3 transform) => Position += transform;

        public void Render() => EngineGL.Instance.SetShaderData("model", Model);

        public void Update()
        {
        }

        public async Task UpdateAsync()
        {
            var t1 = Matrix4.CreateTranslation(Position);
            var r1 = Matrix4.CreateRotationX(Rotation.X);
            var r2 = Matrix4.CreateRotationY(Rotation.Y);
            var r3 = Matrix4.CreateRotationZ(Rotation.Z);
            var s = Matrix4.CreateScale(Scale);
            Model = r1 * r2 * r3 * s * t1;
            await Task.CompletedTask;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
