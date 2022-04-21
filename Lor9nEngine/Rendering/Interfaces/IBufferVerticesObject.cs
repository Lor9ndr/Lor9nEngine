using Lor9nEngine.Rendering.Base;
using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface IBufferVerticesObject : IBufferObject
    {
        public IBufferVerticesObject Setup(Vertex[] vertices);
        public IBufferVerticesObject Setup(Vertex[] vertices, int[] indices);
        public IBufferVerticesObject Setup(int[] indices);
        public void Bind(BufferTarget target);
        public void Unbind(BufferTarget target);

    }
}
