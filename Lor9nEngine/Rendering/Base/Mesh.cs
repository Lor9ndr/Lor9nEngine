
using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Base
{
    public class Mesh : BaseGLObject
    {
        private static int id;
        private int _currentId;
        private string _name = "UNKNOWN";

        /// <summary>
        /// Список используемых текстур
        /// </summary>
        public List<ITexture> Textures = new List<ITexture>();
        public string Name { get => _name; set => _name = value; }


        public Mesh(Vertex[] vertices, string name, List<ITexture>? textures = null, int[]? indices = default)
            : base(vertices, indices)
        {
            id++;
            _currentId = id;
            if (textures != null)
            {
                Textures = textures;
            }
            Name = name;
        }
        public Mesh(string name, IObjectSetupper baseSetupper, List<ITexture>? textures = null)
            : base(baseSetupper)
        {
            id++;
            _currentId = id;
            if (textures != null)
            {
                Textures = textures;
            }
            Name = name;
        }

        public Mesh(Mesh item)
            : base(item.ObjectSetupper)
        {
            id++;
            _currentId = id;
            _name = $"{item.Name}({_currentId})";
        }


        public override void Render(Shader shader)
        {
            //Console.WriteLine($"Rendering MESH : {_name}");
            ObjectSetupper.Vao.Bind();
            for (int i = 0; i < Textures?.Count; i++)
            {
                var tex = Textures[i];

                string name = tex.Type.ToString();
                tex.Use(TextureUnit.Texture0 + i);

                //ProcessWithPBO(tex);


                EngineGL.Instance
                    .SetShaderData($"material.{name}", i)
                    .SetShaderData(name, i);
            }
            base.Render(shader);
            for (int i = 0; i < Textures?.Count; i++)
            {
                EngineGL.Instance.BindTexture(Textures[i].Target, 0);
            }
        }

        public override void RenderWithOutTextures(Shader shader)
        {
            ObjectSetupper.Vao.Bind();
            base.Render(shader);
            ObjectSetupper.Vao.Unbind();

        }

        /// <summary>
        /// NOT OPTIMIZED
        /// </summary>
        /// <param name="tex"></param>
        [Obsolete]
        private static void ProcessWithPBO(ITexture tex)
        {
            if (tex.Pbos is null)
            {
                return;
            }
            PBO pbo = tex.Pbos[tex.CurrentPboIndex];
            pbo.Process(tex);

        }

        public override void Dispose()
        {
            foreach (var item in Textures)
            {
                item.Dispose();
            }
            base.Dispose();
        }


    }
}
