using OpenTK.Mathematics;
using OpenTK.Windowing.Common;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lor9nEngine.Input
{
    public class MouseManager
    {
        private static MouseState lastMouseState;
        private static MouseState thisMouseState;
        private static Game _window;
        public static void Update()
        {
            lastMouseState = thisMouseState;
            thisMouseState = _window.MouseState;
        }
        public void Enable(Game window)
        {
            _window = window;
            _window.MouseMove += Window_MouseMove;
        }
        public void Disable()
        {
            _window.MouseMove -= Window_MouseMove;
        }

        private void Window_MouseMove(MouseMoveEventArgs obj)
        {

        }

        public static float PositionX => thisMouseState.X;
        public static float PositionY => thisMouseState.Y;
        public static Vector2 DeltaPosition => thisMouseState.Delta;
        public static float DeltaScrollX => thisMouseState.Scroll.X - lastMouseState.Scroll.X;
        public static float DeltaScrollY => thisMouseState.Scroll.Y - lastMouseState.Scroll.Y;


        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if mouseButton is down this update but not last one</returns>
        public static bool IsButtonTouched(MouseButton mouseButton) => thisMouseState.IsButtonDown(mouseButton) && lastMouseState.WasButtonDown(mouseButton);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if mouseButton is down</returns>
        public static bool IsButtonDown(MouseButton mouseButton) => thisMouseState.IsButtonDown(mouseButton);

        /// <summary>
        /// 
        /// </summary>
        /// <returns>True if mouseButton is up this update but not last one</returns>
        public static bool IsButtonUp(MouseButton mouseButton) => thisMouseState.WasButtonDown(mouseButton) && lastMouseState.IsButtonDown(mouseButton);
    }
}
