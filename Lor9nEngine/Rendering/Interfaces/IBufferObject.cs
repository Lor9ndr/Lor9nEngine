using OpenTK.Graphics.OpenGL4;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface IBufferObject:IGLObject
    {
        public IBufferObject Setup();
        public void Bind(BufferTarget target);
        public void Unbind(BufferTarget target);
    }
}
