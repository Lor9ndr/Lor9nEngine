using Lor9nEngine.GameObjects;
using Lor9nEngine.Rendering;
using Lor9nEngine.Rendering.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Lor9nEngine.GameObjects.Lights
{

    internal interface ILight : IGameObject
    {
        public void RenderLight(Shader shader);
    }
}
