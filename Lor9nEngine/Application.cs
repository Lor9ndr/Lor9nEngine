using Lor9nEngine.Scenes;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.Desktop;

namespace Lor9nEngine
{
    internal class Application
    {
        public event Action Load;
        public event Action Unload;
        public event Action<FrameEventArgs> UpdateFrame;
        public event Action RenderThreadStarted;
        public event Action<FrameEventArgs> RenderFrame;
        public Game Game { get;private set;}

        public void Run()
        {

            Game =  new Game(GameWindowSettings.Default, NativeWindowSettings.Default);
            Game.Load += OnLoad;
            Game.Unload += OnUnload;
            Game.UpdateFrame += OnUpdateFrame;
            Game.RenderFrame += OnRenderFrame;
            Game.Run();
  
        }
        public virtual void OnLoad()
        {
            Scene? scene = null;
            foreach (var item in Directory.GetFiles("Scenes/"))
            {
                if (item.EndsWith(SceneLoader.FILE_EXTENSION))
                {
                    scene = SceneLoader.LoadSceneByPath(item);
                    break;
                }
            }
            this.Load?.Invoke();
            Game.LoadScene(scene);
        }

        public virtual void OnUnload()
        {
            this.Unload?.Invoke();
        }

        public virtual void OnUpdateFrame(FrameEventArgs args)
        {
            this.UpdateFrame?.Invoke(args);
        }

        public virtual void OnRenderFrame(FrameEventArgs args)
        {
            this.RenderFrame?.Invoke(args);
        }
    }
}
