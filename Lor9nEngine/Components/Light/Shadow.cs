
using Lor9nEngine.GameObjects;
using Lor9nEngine.GameObjects.Lights;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.FrameBuffer;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lor9nEngine.Components.Light
{
    public class Shadow : IComponent, ISettupable, IUpdatable
    {
        private const FramebufferAttachment _attachment = FramebufferAttachment.DepthAttachment;
        private const PixelInternalFormat _format = PixelInternalFormat.DepthComponent;
        private const PixelType _type = PixelType.Float;
        internal delegate void ProjectionHandler();
        internal event ProjectionHandler ChangeProjection;

        private static int id = 31;
        protected BaseLight _attachedLight;

        public string ShadowProperty;
        private float _far = 1000.0f;
        private float _near = 2.0f;

        public FrameBuffer FBO { get; set; }
        public Vector2i Size { get; set; } = new Vector2i(1024, 1024);

        public float Far
        {
            get => _far;
            set
            {
                if (_far != value)
                {
                    _far = Math.Clamp(value, 52f, 10000f);
                    ChangeProjection?.Invoke();
                }
            }
        }
        public float Near
        {
            get => _near;
            set
            {
                if (_near != value)
                {
                    _near = Math.Clamp(value, 1.0f, 50f);
                    ChangeProjection?.Invoke();
                }
            }
        }
        public static FrameBuffer GetDefaultFrameBuffer() => new FrameBuffer(new Vector2i(1024, 1024), ClearBufferMask.DepthBufferBit);

        public Shadow(FrameBuffer fbo, BaseLight light)
        {
            _attachedLight = light;
            FBO = fbo;
            ShadowProperty = light.Name + "shadow";

            ChangeProjection += _attachedLight.RecreateProjection;
        }

        public virtual void Setup()
        {
            FBO.Setup();
            FBO.Bind();
            FBO.DisableColorBuffer();
            if (_attachedLight is PointLight)
            {
                FBO.AttachCubeMap(_attachment, _format, _type);
            }
            else
            {
                FBO.AttachTexture2DMap(_attachment, _format, _type);
            }
        }

        public virtual void Render(Shader shader)
        {
            RefreshTexID();
            FBO.Texture.Use(TextureUnit.Texture0 + id);
            EngineGL.Instance
                .SetShaderData(ShadowProperty, id);

            id--;
        }

        protected static void RefreshTexID()
        {
            if (id == 0)
            {
                SetTextureIdDefault();
            }
        }
        public static void SetTextureIdDefault() => id = 31;

        public void Update()
        {
            throw new NotImplementedException();
        }

        public virtual Task UpdateAsync()
        {
            throw new NotImplementedException();
        }
        public virtual void RenderDepth(IEnumerable<IGameObject> gameObjects)
        {
            throw new NotImplementedException();
        }
        public void Dispose()
        {
            throw new NotImplementedException();
        }
        void IRenderable.RenderWithOutTextures(Shader shader)
        {
            throw new NotImplementedException();
        }
    }
}
