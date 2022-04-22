namespace Lor9nEngine.Rendering.Interfaces
{
    internal interface IGLObject : IDisposable
    {
        public int Handle { get; set; }
        public void Bind();
        public void Unbind();
    }
}
