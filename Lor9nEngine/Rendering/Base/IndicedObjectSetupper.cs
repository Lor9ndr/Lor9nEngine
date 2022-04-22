using System.Runtime.InteropServices;

using Lor9nEngine.Rendering.Base.Buffers;
using Lor9nEngine.Rendering.Interfaces;

using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Base
{
    internal class IndicedObjectSetupper : IObjectSetupper
    {
        private bool _disposedValue = false;
        private Vertex[] _vertices;
        private int[] _indices;

        public VAO Vao { get; init; }

        public VBO Vbo { get; init; }

        public EBO Ebo { get; init; }

        public int IndicesCount { get; init; }
        public int VerticesCount { get; init; }
        public bool HasIndices => IndicesCount > 0;
        public Vertex[] Vertices { get => _vertices; set => _vertices = value; }
        public int[] Indices { get => _indices; set => _indices = value; }

        public IndicedObjectSetupper(Vertex[] vertices, int[] indices)
        {
            VerticesCount = vertices.Length;
            IndicesCount = indices.Length;
            _vertices = vertices;
            _indices = indices;

            Vao = (VAO)new VAO().Setup();
            Vbo = (VBO)new VBO().Setup(vertices);
            Ebo = (EBO)new EBO().Setup(indices);
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
        public IndicedObjectSetupper(IndicedObjectSetupper baseSetupper)
        {
            Vao = baseSetupper.Vao;
            Vbo = baseSetupper.Vbo;
            Ebo = baseSetupper.Ebo;
            _vertices = baseSetupper.Vertices;
            _indices = baseSetupper.Indices;
            VerticesCount = _vertices.Length;
            IndicesCount = baseSetupper.IndicesCount;
        }

        public void Render()
        {
            Vao.Bind();
            EngineGL.Instance.BindBuffer(BufferTarget.ElementArrayBuffer, Ebo)
                            .DrawElements(PrimitiveType.Triangles, IndicesCount, DrawElementsType.UnsignedInt, 0);
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
                    Ebo.Dispose();
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
