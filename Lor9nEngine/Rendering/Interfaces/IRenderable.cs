﻿using OpenTK.Graphics.OpenGL4;

namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface IRenderable : IDisposable
    {
        public void Render(Shader shader);
    }
}
