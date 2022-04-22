using Lor9nEngine.Components;
using Lor9nEngine.GameObjects;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;

using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lor9nEngine
{
    internal class Camera : IGameObject
    {
        public float CameraSpeed
        {
            get => _cameraSpeed;
        }
        public float Sensitivity { get => _sensitivity; }
        private float _cameraSpeed = 50.0f;

        public bool CanMove = true;

        private readonly float _sensitivity = 0.2f;
        private Keyboard? _keyboard;

        private Vector3 _up = Vector3.UnitY;

        private Vector3 _right = Vector3.UnitX;


        // Rotation around the X axis (radians)
        private float _pitch = -MathHelper.PiOver2;

        // Rotation around the Y axis (radians)
        private float _yaw = -MathHelper.PiOver2; // Without this you would be started rotated 90 degrees right

        // The field of view of the camera (radians)
        private float _fov = MathHelper.PiOver2;
        private bool disposedValue;

        public Model Model { get; set; }
        public List<IGameObject> Childrens { get; set; }
        public IGameObject Parent { get; set; }
        public ITransform Transform { get; set; }



        // This is simply the aspect ratio of the viewport, used for the projection matrix
        public float AspectRatio { get; set; }

        public Vector3 Front => Transform.Direction;

        public Vector3 Up => _up;

        public Vector3 Right => _right;
        public Camera(Vector3 position, float aspectRatio)
        {
            AspectRatio = aspectRatio;
            Transform = new Transform(position, Vector3.UnitZ);
        }
        public void Enable(Game window)
        {
            _keyboard = Game.Keyboard;
            window.MouseMove += Window_MouseMove;
            HandleKeyBoard();
        }
        public void Disable(Game window)
        {
            window.MouseMove -= Window_MouseMove;
            _keyboard = null;
        }

        private void HandleKeyBoard()
        {
            if (_keyboard == null)
            {
                return;
            }
            var self = this;
            _keyboard.BindKey(Keys.W, new Action(() => self.Transform.Position += self.Front * self.CameraSpeed * Game.DeltaTime), PressType.Pressing); // Front
            _keyboard.BindKey(Keys.S, new Action(() => self.Transform.Position -= self.Front * self.CameraSpeed * Game.DeltaTime), PressType.Pressing); // Backwards
            _keyboard.BindKey(Keys.A, new Action(() => self.Transform.Position -= self.Right * self.CameraSpeed * Game.DeltaTime), PressType.Pressing); // Left
            _keyboard.BindKey(Keys.D, new Action(() => self.Transform.Position += self.Right * self.CameraSpeed * Game.DeltaTime), PressType.Pressing); // Right
            _keyboard.BindKey(Keys.Space, new Action(() => self.Transform.Position += self.Up * self.CameraSpeed * Game.DeltaTime), PressType.Pressing); // Up
            _keyboard.BindKey(Keys.LeftShift, new Action(() => self.Transform.Position -= self.Up * self.CameraSpeed * Game.DeltaTime), PressType.Pressing); // Down
            _keyboard.BindKey(Keys.RightAlt, new Action(() => self._cameraSpeed++), PressType.Pressing); // ++speed
            _keyboard.BindKey(Keys.RightControl, new Action(() => self._cameraSpeed--), PressType.Pressing); // --speed
        }
        private void Window_MouseMove(MouseMoveEventArgs mouse)
        {
            if (CanMove)
            {
                Yaw += mouse.DeltaX * _sensitivity;
                Pitch -= mouse.DeltaY * _sensitivity;
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Pitch
        {
            get => MathHelper.RadiansToDegrees(_pitch);
            set
            {
                // We clamp the pitch value between -89 and 89 to prevent the camera from going upside down, and a bunch
                // of weird "bugs" when you are using euler angles for rotation.
                // If you want to read more about this you can try researching a topic called gimbal lock
                var angle = MathHelper.Clamp(value, -89f, 89f);
                _pitch = MathHelper.DegreesToRadians(angle);
                UpdateVectors();
            }
        }

        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Yaw
        {
            get => MathHelper.RadiansToDegrees(_yaw);
            set
            {
                _yaw = MathHelper.DegreesToRadians(value);
                UpdateVectors();
            }
        }

        // The field of view (FOV) is the vertical angle of the camera view, this has been discussed more in depth in a
        // previous tutorial, but in this tutorial you have also learned how we can use this to simulate a zoom feature.
        // We convert from degrees to radians as soon as the property is set to improve performance
        public float Fov
        {
            get => MathHelper.RadiansToDegrees(_fov);
            set
            {
                var angle = MathHelper.Clamp(value, 45f, 120f);
                _fov = MathHelper.DegreesToRadians(angle);
            }
        }



        // Get the view matrix using the amazing LookAt function described more in depth on the web tutorials
        public Matrix4 GetViewMatrix() => Matrix4.LookAt(Transform.Position, Transform.Position + Transform.Direction, _up);

        // Get the projection matrix using the same method we have used up until this point
        public Matrix4 GetProjectionMatrix() => Matrix4.CreatePerspectiveFieldOfView(_fov, AspectRatio, 0.1f, 10000.0f);

        // This function is going to update the direction vertices using some of the math learned in the web tutorials
        private void UpdateVectors()
        {
            // First the front matrix is calculated using some basic trigonometry
            Transform.Direction = new Vector3(MathF.Cos(_pitch) * MathF.Cos(_yaw), MathF.Sin(_pitch), MathF.Cos(_pitch) * MathF.Sin(_yaw));

            // We need to make sure the vectors are all normalized, as otherwise we would get some funky results
            Transform.Direction = Vector3.Normalize(Transform.Direction);

            // Calculate both the right and the up vector using cross product
            // Note that we are calculating the right from the global up, this behaviour might
            // not be what you need for all cameras so keep this in mind if you do not want a FPS camera
            _right = Vector3.Normalize(Vector3.Cross(Front, Vector3.UnitY));
            _up = Vector3.Normalize(Vector3.Cross(_right, Front));
        }
        public void Update(float dt)
        {
        }

        public void Render()
        {
            var view = GetViewMatrix();
            var projection = GetProjectionMatrix();
            EngineGL.Instance.SetShaderData("VP", view * projection)
                .SetShaderData("projection", projection)
                .SetShaderData("view", view)
                .SetShaderData("viewPos", Transform.Position);
        }

        public void Render(Shader shader)
        {
            var view = GetViewMatrix();
            var projection = GetProjectionMatrix();
            EngineGL.Instance.UseShader(shader)
                .SetShaderData("VP", view * projection)
                .SetShaderData("projection", projection)
                .SetShaderData("view", view)
                .SetShaderData("viewPos", Transform.Position);
        }



        public void Update()
        {
        }

        public async Task UpdateAsync()
        {
            await Transform.UpdateAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        ~Camera()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
