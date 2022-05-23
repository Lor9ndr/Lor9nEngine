using Lor9nEngine.Components.Transform;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;

namespace Lor9nEngine.GameObjects
{

    internal class GameObject : IGameObject
    {
        private IGameObject? _parent;
        public ITransform Transform { get; set; }
        public List<IGameObject> Childrens { get; set; } = new List<IGameObject>();
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
        public Model Model { get; set; }

        public GameObject(Model model, Transform transform, IGameObject? parent = null)
        {
            Model = model;
            Transform = transform;
            _parent = parent;
        }

        public void Render(Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            Transform.Render(shader);
            Model.Render(shader);
        }

        public void Update()
        {
            Transform.Update();
            Model.Update();
            foreach (var item in Childrens)
            {
                item.Update();
            }
        }

        public async Task UpdateAsync()
        {
            await Transform.UpdateAsync();
            await Model.UpdateAsync();
        }

        public void Dispose()
        {
            Model.Dispose();
        }

        public void RenderWithOutTextures(Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            Transform.Render(shader);
            Model.RenderWithOutTextures(shader);
        }

        public void RenderModel(Shader shader)
        {
            Render(shader);
        }
    }
}
