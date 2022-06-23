using SFML.Window;
using SFML.System;

namespace ForTeacher
{
    public enum InputEventType
    {
        Pressed,
        Released
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

        private Dictionary<Keyboard.Key, bool> _keyStates;
        private Dictionary<Mouse.Button, bool> _mouseStates;

        private Dictionary<Keyboard.Key, List<(InputEventType, Action<object, KeyEventArgs>)>> _keyEvents;
        private Dictionary<Mouse.Button, (InputEventType, Action<object, MouseButtonEventArgs>)> _mouseEvents;

        public Input()
        {
            _keyStates = new();
            _mouseStates = new();
            _keyEvents = new();
            _mouseEvents = new();
            
            Program.Window.KeyPressed += OnKeyPressed;
            Program.Window.KeyReleased += OnKeyReleased;
            //Program.Window.MouseButtonPressed += OnMouseButtonPressed;
            //Program.Window.MouseButtonReleased += OnMouseButtonReleased;
        }

        public static void BindKeyEvent(
            Keyboard.Key key, 
            InputEventType type, 
            Action<object, KeyEventArgs> callback)
        {
            // Create the list if needed
            if (Instance._keyEvents.ContainsKey(key) == false)
            {
                Instance._keyEvents[key] = new();
            }
            
            Instance._keyEvents[key].Add((type, callback));
            Instance._keyStates[key] = false;
        }

        public static void ResetStates()
        {
        }

        private void OnKeyPressed(object? sender, KeyEventArgs e)
        {
            // We only care about events looking for this key
            if (_keyEvents.ContainsKey(e.Code) == false)
                return;

            // Ensure event is only fired once
            if (_keyStates[e.Code] == true)
                return;

            _keyStates[e.Code] = true;

            foreach (var keyEvent in _keyEvents[e.Code])
            {
                if (keyEvent.Item1 == InputEventType.Pressed)
                {
                    keyEvent.Item2(this, e);
                }
            }
        }

        private void OnKeyReleased(object? sender, KeyEventArgs e)
        {
            // We only care about events looking for this key
            if (_keyEvents.ContainsKey(e.Code) == false)
                return;

            // Ensure event is only fired once
            if (_keyStates[e.Code] == false)
                return;

            _keyStates[e.Code] = false;

            foreach (var keyEvent in _keyEvents[e.Code])
            {
                if (keyEvent.Item1 == InputEventType.Released)
                {
                    keyEvent.Item2(this, e);
                }
            }
        }

        private void OnMouseButtonPressed(object? sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }

        private void OnMouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
