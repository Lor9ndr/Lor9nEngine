using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface IUpdatable
    {
        public void Update();
        public Task UpdateAsync();
    }
}
