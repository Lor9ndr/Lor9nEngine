using Lor9nEngine.GameObjects;
using Lor9nEngine.GameObjects.Lights;
using Lor9nEngine.Rendering;

using Newtonsoft.Json;

namespace Lor9nEngine.Scenes
{
    internal class Scene
    {
        public string Name { get; set; }
        public List<IGameObject> GameObjects { get; set; }
        public IEnumerable<ILight> Lights => GameObjects.OfType<ILight>();
        public IEnumerable<IGameObject> GameObjectsWithOutLights => GameObjects.Except(Lights);
        public SkyBox SkyBox => GameObjects.OfType<SkyBox>().FirstOrDefault();
        public Camera Camera => GameObjects.OfType<Camera>().First();
        [JsonConstructor]
        public Scene(string name, List<IGameObject> gameObjects)
        {
            Name = name;
            GameObjects = gameObjects;
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
            SkyBox?.Render();
            EngineGL.Instance.UseShader(shader);
            SetupCamera(Camera, shader);
            RenderObjects(GameObjectsWithOutLights, shader);
            RenderLights(shader);
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
            RenderObjects(lights, shader);
        }
        public void RenderLightBoxes(Shader shader) => RenderLightBoxes(Lights, shader);


    }
}
