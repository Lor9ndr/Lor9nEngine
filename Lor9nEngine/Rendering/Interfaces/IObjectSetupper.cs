using Lor9nEngine.Rendering.Base;
using Lor9nEngine.Rendering.Base.Buffers;

namespace Lor9nEngine.Rendering.Interfaces
{
    public interface IObjectSetupper : IRenderable
    {
        public VAO Vao { get; init; }
        public VBO Vbo { get; init; }
        public EBO Ebo { get; init; }
        public Vertex[] Vertices { get; set; }
        public int IndicesCount { get; init; }
        public int VerticesCount { get; init; }
        public bool HasIndices => IndicesCount > 0;

    }
}
