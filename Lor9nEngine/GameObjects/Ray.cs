
using Lor9nEngine.Components.Transform;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects
{
    internal struct Ray : IGameObject
    {

        public Vector3 Origin;
        public Vector3 Direction;
        IGameObject IGameObject.Parent { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        ITransform IGameObject.Transform { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        List<IGameObject> IGameObject.Childrens { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        Model IGameObject.Model { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }


        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Render(Shader shader)
        {
        }

        public void Update()
        {
            throw new NotImplementedException();
        }

        public Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
        public Vector3 GetPoint(float deltaTime)
        {
            return Origin + Direction * deltaTime;
        }

        public static Ray GetWorldSpaceRay(Matrix4 inverseProjection, Matrix4 inverseView, Vector3 worldPosition, Vector2 normalizedDeviceCoords)
        {
            Vector4 rayEye = new Vector4(normalizedDeviceCoords.X, normalizedDeviceCoords.Y, -1.0f, 1.0f) * inverseProjection;
            rayEye.Z = -1.0f;
            rayEye.W = 0.0f;
            return new Ray { Origin = worldPosition, Direction = (rayEye * inverseView).Xyz.Normalized() };
        }

        void IRenderable.Render(Shader shader)
        {
            throw new NotImplementedException();
        }

        void IRenderable.RenderWithOutTextures(Shader shader)
        {
            throw new NotImplementedException();
        }

        void IUpdatable.Update()
        {
            throw new NotImplementedException();
        }

        Task IUpdatable.UpdateAsync()
        {
            throw new NotImplementedException();
        }

        void IDisposable.Dispose()
        {
            throw new NotImplementedException();
        }

        public void RenderModel(Shader shader)
        {
            throw new NotImplementedException();
        }
    }
}
