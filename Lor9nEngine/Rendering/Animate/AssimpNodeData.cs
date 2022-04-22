﻿using OpenTK.Mathematics;

namespace Lor9nEngine.Rendering.Animate
{

    public struct AssimpNodeData
    {
        public Matrix4 Transformation;
        public string Name;
        public int ChildrenCount;
        public List<AssimpNodeData> children;
    }
}
