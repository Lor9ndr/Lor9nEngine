using Lor9nEngine.Components;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Interfaces;
using Lor9nEngine.Rendering.Textures;

using Newtonsoft.Json;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.GameObjects
{
    internal class SkyBox : IGameObject
    {
        private readonly Shader _skyBoxShader = new Shader(Game.SKYBOX_SHADER_PATH + ".vs", Game.SKYBOX_SHADER_PATH + ".fr");

        private readonly Vertex[] skyboxVertices =  {
        new Vertex(-1.0f,  1.0f, -1.0f) ,
        new Vertex(-1.0f, -1.0f, -1.0f),
        new Vertex(1.0f, -1.0f, -1.0f),
        new Vertex(1.0f, -1.0f, -1.0f),
        new Vertex(1.0f,  1.0f, -1.0f),
        new Vertex(-1.0f,  1.0f, -1.0f),

        new Vertex(-1.0f, -1.0f,  1.0f),
        new Vertex(-1.0f, -1.0f, -1.0f),
        new Vertex(-1.0f,  1.0f, -1.0f),
        new Vertex(-1.0f,  1.0f, -1.0f),
        new Vertex(-1.0f,  1.0f,  1.0f),
        new Vertex(-1.0f, -1.0f,  1.0f),

        new Vertex(1.0f, -1.0f, -1.0f),
        new Vertex( 1.0f, -1.0f,  1.0f),
        new Vertex( 1.0f,  1.0f,  1.0f),
        new Vertex( 1.0f,  1.0f,  1.0f),
        new Vertex( 1.0f,  1.0f, -1.0f),
        new Vertex( 1.0f, -1.0f, -1.0f),

        new Vertex(-1.0f, -1.0f,  1.0f),
        new Vertex(-1.0f,  1.0f,  1.0f),
        new Vertex( 1.0f,  1.0f,  1.0f),
        new Vertex( 1.0f,  1.0f,  1.0f),
        new Vertex( 1.0f, -1.0f,  1.0f),
        new Vertex(-1.0f, -1.0f,  1.0f),

        new Vertex(-1.0f,  1.0f, -1.0f),
        new Vertex( 1.0f,  1.0f, -1.0f),
        new Vertex( 1.0f,  1.0f,  1.0f),
        new Vertex( 1.0f,  1.0f,  1.0f),
        new Vertex(-1.0f,  1.0f,  1.0f),
        new Vertex(-1.0f,  1.0f, -1.0f),

        new Vertex(-1.0f, -1.0f, -1.0f),
        new Vertex(-1.0f, -1.0f,  1.0f),
        new Vertex( 1.0f, -1.0f, -1.0f),
        new Vertex( 1.0f, -1.0f, -1.0f),
        new Vertex(-1.0f, -1.0f,  1.0f),
        new Vertex( 1.0f, -1.0f,  1.0f)
    };
        public readonly string[] faces =
        {
            "right.jpg",
            "left.jpg",
            "top.jpg",
            "bottom.jpg",
            "front.jpg",
            "back.jpg"
        };
        private CubeMap _cubemapTexture;
        private bool disposedValue;

        public Model Model { get; set; }
        public List<IGameObject> Childrens { get; set; }
        public IGameObject Parent { get; set; }
        public ITransform Transform { get; set; }

        [JsonConstructor]
        public SkyBox()
        {
            _cubemapTexture = CubeMap.LoadCubeMapFromFile(faces, TextureType.Skybox);
            ResizeVerticesToSkybox();
            Model = new Model(new Mesh(skyboxVertices, "SkyBox", textures: new List<ITexture>() { _cubemapTexture }), "SkyBox");
            EngineGL.Instance.UseShader(_skyBoxShader).SetShaderData("skybox", 0);
        }
        private void ResizeVerticesToSkybox()
        {
            for (int i = 0; i < skyboxVertices.Length; i++)
            {
                skyboxVertices[i].Position *= 100000;
            }
        }

        public void Render()
        {
            GL.DepthFunc(DepthFunction.Lequal);
            var viewproj = Game.Camera.GetViewMatrix() * Game.Camera.GetProjectionMatrix();
            EngineGL.Instance.UseShader(_skyBoxShader).SetShaderData("VP", viewproj);
            Model.Render(_skyBoxShader);
            GL.DepthFunc(DepthFunction.Less);
        }

        public void Render(Shader shader)
        {
            Render();
        }

        public void Render(PrimitiveType type)
        {
            Render();
        }

        public void Update()
        {
        }

        public async Task UpdateAsync()
        {

        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Model.Dispose();
                    _skyBoxShader.Dispose();
                    // TODO: освободить управляемое состояние (управляемые объекты)
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~SkyBox()
        // {
        //     // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Не изменяйте этот код. Разместите код очистки в методе "Dispose(bool disposing)".
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
