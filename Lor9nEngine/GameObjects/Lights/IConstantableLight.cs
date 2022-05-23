
using Lor9nEngine.Components.Light;

namespace Lor9nEngine.GameObjects.Lights
{
    public interface IConstantableLight
    {
        public LightConstants LightConstants { get; set; }
    }
}
