using System.Runtime.InteropServices;

using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Base
{
    internal class ObjectSetupper : IObjectSetupper
    {
        private bool _disposedValue = false;
        private Vertex[] _vertices;
        public VAO Vao { get; init; }

        public VBO Vbo { get; init; }

        public EBO Ebo { get; init; } = new EBO();

        public int IndicesCount { get; init; } = 0;
        public int VerticesCount { get; init; }
        public bool HasIndices => IndicesCount > 0;

        public Vertex[] Vertices { get => _vertices; set => _vertices = value; }

        public ObjectSetupper(Vertex[] vertices)
        {
            VerticesCount = vertices.Length;

            Vao = (VAO)new VAO().Setup();
            Vbo = (VBO)new VBO().Setup(vertices);


            _vertices = vertices;
            EngineGL.Instance.BindVAO(Vao);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, Vertex.Size, Marshal.OffsetOf<Vertex>("Position"));
            GL.EnableVertexAttribArray(0);

            GL.VertexAttribPointer(1, 3, VertexAttribPointerType.Float, false, Vertex.Size, Marshal.OffsetOf<Vertex>("Normal"));
            GL.EnableVertexAttribArray(1);

            GL.VertexAttribPointer(2, 2, VertexAttribPointerType.Float, false, Vertex.Size, Marshal.OffsetOf<Vertex>("TexCoords"));
            GL.EnableVertexAttribArray(2);

            GL.VertexAttribPointer(3, 3, VertexAttribPointerType.Float, false, Vertex.Size, Marshal.OffsetOf<Vertex>("Tangent"));
            GL.EnableVertexAttribArray(3);

            GL.VertexAttribPointer(4, 3, VertexAttribPointerType.Float, false, Vertex.Size, Marshal.OffsetOf<Vertex>("Bitangent"));
            GL.EnableVertexAttribArray(4);

            GL.VertexAttribIPointer(5, 4, VertexAttribIntegerType.Int, Vertex.Size, Marshal.OffsetOf<Vertex>("BoneIDs"));
            GL.EnableVertexAttribArray(5);

            GL.VertexAttribPointer(6, 4, VertexAttribPointerType.Float, true, Vertex.Size, Marshal.OffsetOf<Vertex>("Weights"));
            GL.EnableVertexAttribArray(6);

            EngineGL.Instance.UnbindBuffer(BufferTarget.ArrayBuffer).UnbindVAO();
        }
        public ObjectSetupper(ObjectSetupper baseSetupper)
        {
            Vao = baseSetupper.Vao;
            Vbo = baseSetupper.Vbo;
            Ebo = baseSetupper.Ebo;
            _vertices = baseSetupper.Vertices;
            VerticesCount = _vertices.Length;
        }

        public void Render()
        {

            Vao.Bind();
            EngineGL.Instance.DrawArrays(PrimitiveType.Triangles, 0, VerticesCount);
            Vao.Unbind();
        }

        public void Render(Shader shader)
        {
            EngineGL.Instance.UseShader(shader);
            Render();
        }
        public void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    Vao.Dispose();
                    Vbo.Dispose();
                }

                // TODO: освободить неуправляемые ресурсы (неуправляемые объекты) и переопределить метод завершения
                // TODO: установить значение NULL для больших полей
                _disposedValue = true;
            }
        }

        // // TODO: переопределить метод завершения, только если "Dispose(bool disposing)" содержит код для освобождения неуправляемых ресурсов
        // ~IndicesObjectSetupper()
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
