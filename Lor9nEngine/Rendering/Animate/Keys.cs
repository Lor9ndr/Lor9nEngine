using OpenTK.Mathematics;

namespace Lor9nEngine.Rendering.Animate
{
    public struct KeyPosition
    {
        public Vector3 Position;
        public float TimeStamp;
    }
    public struct KeyRotation
    {
        public Quaternion Orientation;
        public float TimeStamp;
    }
    public struct KeyScale
    {
        public Vector3 Scale;
        public float TimeStamp;
    }
}
