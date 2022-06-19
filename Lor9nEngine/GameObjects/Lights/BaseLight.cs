using Lor9nEngine.Components.Light;
using Lor9nEngine.Components.Transform;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Mathematics;

namespace Lor9nEngine.GameObjects.Lights
{
    public class BaseLight : ILight, ISettupable
    {
        public IGameObject? Parent
        {
            get => _parent;
            set
            {
                if (value is not null)
                {
                    _parent = value;
                    Transform = new ParentedTransform(Transform, Parent!);
                }
            }
        }

        public LightData LightData { get; set; }
        public Model Model { get; set; }
        public List<IGameObject> Childrens { get; set; } = new List<IGameObject>();

        public ITransform Transform { get; set; }
        public string Name => _name;
        public Matrix4 Projection { get => _projection; set => _projection = value; }
        public Shadow Shadow { get; set; }

        protected string _name;
        private IGameObject? _parent;
        private Matrix4 _projection;

        public BaseLight(LightData lightData, Model model)
        {
            Model = model;
            LightData = lightData;
            Transform = new Transform(new Vector3(10), new Vector3(0), new Vector3(5));
            _parent = null;
            Shadow = new DirectShadow(this);
        }

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
            await Task.WhenAll(Shadow.UpdateAsync(), Transform.UpdateAsync());

            if (Model != null)
            {
                await Model.UpdateAsync();
            }

        }

        public virtual void Render(Shader shader)
        {
            EngineGL.Instance.UseShader(shader).SetShaderData("lightColor", LightData.Color);
            Shadow.Render(shader);
            Transform.Render(shader);
            Model.Render(shader);
        }
        public void RenderModel(Shader shader)
        {
            EngineGL.Instance.UseShader(shader).SetShaderData("lightColor", LightData.Color);
            Transform.Render(shader);
            Model.Render(shader);
        }

        public virtual void RecreateProjection()
        {
            throw new NotImplementedException();
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
        public virtual void RenderDepth(IEnumerable<IGameObject> gameObjects)
        {
            Shadow.RenderDepth(gameObjects);
        }

        public virtual void Setup()
        {
            throw new NotImplementedException();
        }

        public void RenderWithOutTextures(Shader shader)
        {
            throw new NotImplementedException();
        }


    }
}
