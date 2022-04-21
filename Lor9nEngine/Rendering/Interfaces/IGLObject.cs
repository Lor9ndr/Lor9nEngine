using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface IGLObject : IDisposable
    {
        public int Handle { get; set; }
        public void Bind();
        public void Unbind();
    }
}
