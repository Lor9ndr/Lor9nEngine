using OpenTK.Windowing.Desktop;
using OpenTK.Windowing.GraphicsLibraryFramework;

namespace Lor9nEngine
{
    /// <summary>
    /// Класс отвечающий за клавиатуру
    /// </summary>
    public class Keyboard
    {
        /// <summary>
        /// Словарь, хранящий список действий при нажатии на определенную клавишу
        /// </summary>
        private Dictionary<Keys, List<Action>> _bindsKeyDown = new Dictionary<Keys, List<Action>>();

        /// <summary>
        /// Словарь, хранящий список действий при отжатии клавиши
        /// </summary>
        private Dictionary<Keys, List<Action>> _bindsKeyUp = new Dictionary<Keys, List<Action>>();
        /// <summary>
        /// Словарь, хранящий список действий при зажатии клавиши
        /// </summary>
        private Dictionary<Keys, List<Action>> _bindsKeyPressing = new Dictionary<Keys, List<Action>>();

        private GameWindow _window;

        /// <summary>
        /// Конструктор, принимающий окно, у которого есть события нажатия на клавишу
        /// </summary>
        /// <param name="window">окно</param>
        public Keyboard(GameWindow window)
        {
            _window = window;
            _window.KeyDown += Window_KeyDown;
            _window.KeyUp += Window_KeyUp;
            _window.UpdateFrame += _window_UpdateFrame;
        }

        private void _window_UpdateFrame(OpenTK.Windowing.Common.FrameEventArgs obj)
        {
            foreach (var keyAction in _bindsKeyPressing)
            {
                if (_window.KeyboardState.IsKeyDown(keyAction.Key) && _window.KeyboardState.WasKeyDown(keyAction.Key))
                {
                    foreach (var action in keyAction.Value)
                    {
                        action.Invoke();
                    }
                }
            }
        }

        /// <summary>
        /// Добавляем действие
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="action">Действие</param>
        /// <param name="type">Тип нажатия</param>
        public void BindKey(Keys key, Action action, PressType type)
        {
            if (type == PressType.Down)
            {
                BindKeyOnKeyDown(key, action);
            }
            else if (type == PressType.Up)
            {
                BindOnKeyUp(key, action);
            }
            else
            {
                BindKeyOnPressing(key, action);
            }
        }

        /// <summary>
        /// Добавляем действие при нажатии клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="action">Действие</param>
        private void BindKeyOnKeyDown(Keys key, Action action)
        {
            if (_bindsKeyDown.TryGetValue(key, out List<Action> actions))
            {
                actions.Add(action);
            }
            else
            {
                _bindsKeyDown.Add(key, new List<Action>() { action });
            }

        }

        /// <summary>
        /// Добавляем действие при отжатии клавиши
        /// </summary>
        /// <param name="key">Клавиша</param>
        /// <param name="action">Действие</param>
        private void BindOnKeyUp(Keys key, Action action)
        {
            if (_bindsKeyUp.TryGetValue(key, out List<Action> actions))
            {
                actions.Add(action);
            }
            else
            {
                _bindsKeyUp.Add(key, new List<Action>() { action });
            }
        }

        private void BindKeyOnPressing(Keys key, Action action)
        {
            if (_bindsKeyPressing.TryGetValue(key, out List<Action> actions))
            {
                actions.Add(action);
            }
            else
            {
                _bindsKeyPressing.Add(key, new List<Action>() { action });
            }
        }

        private void Window_KeyUp(OpenTK.Windowing.Common.KeyboardKeyEventArgs obj)
        {
            if (_bindsKeyUp.TryGetValue(obj.Key, out List<Action> actions))
            {
                foreach (var action in actions)
                {
                    action.Invoke();
                }
            }
        }

        private void Window_KeyDown(OpenTK.Windowing.Common.KeyboardKeyEventArgs obj)
        {
            if (_bindsKeyDown.TryGetValue(obj.Key, out List<Action> actions))
            {
                foreach (var action in actions)
                {
                    action.Invoke();
                }
            }
        }
    }
}
