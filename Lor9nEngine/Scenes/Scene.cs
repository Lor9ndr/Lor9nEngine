
using Lor9nEngine.Components.Light;
using Lor9nEngine.GameObjects;
using Lor9nEngine.GameObjects.Lights;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.FrameBuffer;

using Newtonsoft.Json;

using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;
using OpenTK.Windowing.Common;

namespace Lor9nEngine.Scenes
{
    public class Scene
    {
        public event Action<ResizeEventArgs> OnResize;
        public string Name { get; set; }
        public List<IGameObject> GameObjects { get; set; }
        public IEnumerable<ILight> Lights => GameObjects.OfType<ILight>();
        public IEnumerable<IGameObject> GameObjectsWithOutLights => GameObjects.Except(Lights).Where(s => s.GetType() != typeof(SkyBox));
        public IEnumerable<IGameObject> GameObjectsForShadow => GameObjects.Except(Lights).Where(s => s.GetType() != typeof(Camera) && s.GetType() != typeof(SkyBox));
        public SkyBox? SkyBox => GameObjects.OfType<SkyBox>().FirstOrDefault();
        public Camera Camera => GameObjects.OfType<Camera>().First();
        public FrameBuffer DefaultFBO = new FrameBuffer(new Vector2i(Game.Width, Game.Height), ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

        [JsonConstructor]
        public Scene(string name, List<IGameObject> gameObjects)
        {
            DefaultFBO = new FrameBuffer(new Vector2i(Game.Width, Game.Height), ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            Name = name;
            GameObjects = gameObjects;
            OnResize += Resize;

        }

        protected virtual void Resize(ResizeEventArgs e)
        {
            DefaultFBO.Size = e.Size;
        }
        public void RenderObjects(Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            foreach (var go in GameObjects)
            {
                go.Render(shader);
            }
        }
        public void RenderLights(Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            foreach (var light in Lights)
            {
                light.RenderLight(shader);
            }
        }
        public void SetupCamera(Camera camera, Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            camera.Render(shader);
        }

        public void RenderAll(Shader shader)
        {
            RenderDepth(Lights, GameObjectsForShadow);

            DefaultFBO.Activate();
            EngineGL.Instance.Enable(EnableCap.DepthTest)
                .Enable(EnableCap.CullFace)
                .CullFace(CullFaceMode.Back)
                .ColorMask(true, true, true, true)
                .DepthMask(true)
                .Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
            RenderAllGoObjects(shader);
        }

        public void RenderDepth(IEnumerable<ILight> lights, IEnumerable<IGameObject> gameObjectCastingShadow)
        {
            SetupCamera(Camera, PointShadow.DepthPointShader);
            SetupCamera(Camera, DirectShadow.DepthDirectShader);

            foreach (var item in lights)
            {
                item.RenderDepth(gameObjectCastingShadow);
            }
        }

        private void RenderAllGoObjects(Shader shader)
        {
            SkyBox?.Render();
            EngineGL.Instance.UseShader(shader);
            SetupCamera(Camera, shader);
            RenderLights(shader);
            RenderObjects(GameObjectsWithOutLights, shader);
        }

        public void RenderObjects(IEnumerable<IGameObject> gameObjects, Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            foreach (var go in gameObjects)
            {
                go.Render(shader);
            }
        }

        public void RenderLightBoxes(IEnumerable<ILight> lights, Shader shader)
        {
            SetupCamera(Camera, shader);
            foreach (var go in lights)
            {
                go.RenderModel(shader);
            }
        }
        public void RenderLightBoxes(Shader shader) => RenderLightBoxes(Lights, shader);


    }
}
