using OpenTK.Mathematics;

namespace Lor9nEngine.Rendering.Animate
{
    [Serializable]
    public struct BoneInfo
    {
        public int ID;
        public Matrix4 offset;
    }
}
