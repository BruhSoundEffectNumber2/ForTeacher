using SFML.Window;
using SFML.System;

namespace ForTeacher
{
    public enum InputEventType
    {
        Pressed,
        Released
    }

    public struct InputEventArgs
    {
        public InputEventType type;
    }
    
    public class Input
    {
        public static Input Instance
        {
            get 
            {
                if (_instance == null)
                {
                    _instance = new Input();
                }

                return _instance;
            }
        }
        private static Input? _instance;

        public delegate void InputKeyEventHandler(
            InputEventArgs inputArgs,
            KeyEventArgs eventArgs);

        public delegate void InputMouseEventHandler(
            InputEventArgs inputArgs,
            MouseButtonEventArgs eventArgs);

        private Dictionary<Keyboard.Key, bool> _keyStates;
        private Dictionary<Mouse.Button, bool> _mouseStates;

        private Dictionary<Keyboard.Key, InputKeyEventHandler> _keyEvents;
        private Dictionary<Mouse.Button, InputMouseEventHandler> _mouseEvents;

        public Input()
        {
            _keyStates = new();
            _mouseStates = new();
            _keyEvents = new();
            _mouseEvents = new();
            
            Program.Window.KeyPressed += OnKeyPressed;
            Program.Window.KeyReleased += OnKeyReleased;
            Program.Window.MouseButtonPressed += OnMouseButtonPressed;
            Program.Window.MouseButtonReleased += OnMouseButtonReleased;
        }

        public static void BindKeyEvent(Keyboard.Key key, InputKeyEventHandler handler)
        {
            if (Instance._keyEvents.ContainsKey(key) == false)
            {
                Instance._keyEvents[key] = handler;
            } else
            {
                Instance._keyEvents[key] += handler;
            }

            if (Instance._keyStates.ContainsKey(key) == false)
            {
                Instance._keyStates[key] = false;
            }
        }

        public static void UnbindKeyEvent(Keyboard.Key key, InputKeyEventHandler handler)
        {
            if (Instance._keyEvents.ContainsKey(key) == false)
            {
                return;
            }

            if (Instance._keyEvents[key].GetInvocationList().Contains(handler))
            {
                Instance._keyEvents[key] -= handler;
            }
        }

        public static void BindMouseEvent(Mouse.Button button, InputMouseEventHandler handler)
        {
            if (Instance._mouseEvents.ContainsKey(button) == false)
            {
                Instance._mouseEvents[button] = handler;
            }
            else
            {
                Instance._mouseEvents[button] += handler;
            }

            if (Instance._mouseStates.ContainsKey(button) == false)
            {
                Instance._mouseStates[button] = false;
            }
        }

        public static void UnbindMouseEvent(Mouse.Button button, InputMouseEventHandler handler)
        {
            if (Instance._mouseEvents.ContainsKey(button) == false)
            {
                return;
            }

            if (Instance._mouseEvents[button].GetInvocationList().Contains(handler))
            {
                Instance._mouseEvents[button] -= handler;
            }
        }

        public static bool IsKeyDown(Keyboard.Key key)
        {
            return Keyboard.IsKeyPressed(key);
        }

        public static bool IsMouseButtonDown(Mouse.Button button)
        {
            return Mouse.IsButtonPressed(button);
        }

        private void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            if (_keyStates.ContainsKey(e.Code))
            {
                // Ensure event is only fired once, but this only matters if we have seen it before
                if (_keyStates[e.Code] == true)
                    return;
            }

            _keyStates[e.Code] = true;

            if (_keyEvents.ContainsKey(e.Code))
            {
                _keyEvents[e.Code]?.Invoke(new InputEventArgs() { type = InputEventType.Pressed }, e);
            }
        }

        private void OnKeyReleased(object? sender, KeyEventArgs e)
        {
            if (_keyStates.ContainsKey(e.Code))
            {
                // Ensure event is only fired once, but this only matters if we have seen it before
                if (_keyStates[e.Code] == false)
                    return;
            }

            _keyStates[e.Code] = false;

            if (_keyEvents.ContainsKey(e.Code))
            {
                _keyEvents[e.Code]?.Invoke(new InputEventArgs() { type = InputEventType.Released }, e);
            }
        }

        private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            if (_mouseStates.ContainsKey(e.Button))
            {
                // Ensure event is only fired once, but this only matters if we have seen it before
                if (_mouseStates[e.Button] == true)
                    return;
            }

            _mouseStates[e.Button] = true;

            if (_mouseEvents.ContainsKey(e.Button))
            {
                _mouseEvents[e.Button]?.Invoke(new InputEventArgs() { type = InputEventType.Pressed }, e);
            }
        }

        private void OnMouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            if (_mouseStates.ContainsKey(e.Button))
            {
                // Ensure event is only fired once, but this only matters if we have seen it before
                if (_mouseStates[e.Button] == false)
                    return;
            }

            _mouseStates[e.Button] = false;

            if (_mouseEvents.ContainsKey(e.Button))
            {
                _mouseEvents[e.Button]?.Invoke(new InputEventArgs() { type = InputEventType.Released }, e);
            }
        }
    }
}
