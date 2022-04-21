using Lor9nEngine.Components;
using Lor9nEngine.Components.Light;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Interfaces;
using Newtonsoft.Json;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects.Lights
{
    [Serializable]

    internal class BaseLight : ILight
    {
        public LightData LightData { get;set;}
        public Model Model { get;set; } 
        public List<IGameObject> Childrens { get;set;} = new List<IGameObject>();
        public IGameObject Parent { get;set;}
        public ITransform Transform { get;set; }
        public BaseLight(LightData lightData, Model model)
        {
            Model = model;
            LightData = lightData;
            Transform = new Transform(new Vector3(10), new Vector3(0), new Vector3(5));
        }

        [JsonConstructor]
        public BaseLight(LightData lightData, Model model, Transform transform) 
            : this(lightData, model)
        {
            Transform = transform;
        }
        public void Update()
        {
            Transform.Update();
            Model?.Update();
            foreach (var item in Childrens)
            {
                item.Update();
            }
        }

        public async Task UpdateAsync()
        {
            await Transform.UpdateAsync();
            if (Model != null)
            {
                await Model.UpdateAsync();
            }
        }

        public virtual void Render(Shader shader)
        {
            EngineGL.Instance.UseShader(shader).SetShaderData("lightColor", LightData.Color);
            Transform.Render(shader);
            Model.Render(shader);
        }

        public void Dispose()
        {
            Model.Dispose();
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// Метод для подачи в шейдер данных о свете
        /// </summary>
        /// <param name="shader">в какой шейдер отправляем данные</param>
        public virtual void RenderLight(Shader shader)
        {
            throw new NotImplementedException();
        }
    }
}
