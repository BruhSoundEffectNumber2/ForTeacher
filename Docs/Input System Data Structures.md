# Input System Data Structures

## Requirements

- Subscribe to any key or mouse button
- Bind to either pressed or released
- Multiple event listeners for any above event
- Unsubscribe from any above event

## C# Event System

- Publishers (events) and Subscribers (listeners)
- Easy syntax for adding/removing listeners (+=/-=)
- Multiple listeners per-event
- Delegate system for sending arguments
- Easy to invoke event for all listeners

## Input System

- Create C# events lazily
- Need to differentiate between pressed and released
  - ~~Two events for pressed and released~~
    - Increases boilerplate within Input
    - Likely to result in the same issues with enumerating over changed lists
  - One event for each key, listener chooses to ignore
    - Less events need to be created
    - Less needs to be changed for adding more options (double press, hold, etc.)
    - More complexity for subscribers to handle, more functions for each case
- Keep track of key states as normal and still decoupled from events

---

4 functions

- `BindKeyEvent(Keyboard.Key key, InputKeyEventHandler handler)`
- `UnbindKeyEvent(Keyboard.Key key, InputKeyEventHandler handler)`
- `BindMouseEvent(Mouse.Button button, InputKeyEventHandler handler)`
- `BindKeyEvent(Mouse.Button button, InputKeyEventHandler handler)`

A struct, `InputEventArgs`, containing:

- What triggered this event (key/button was pressed/released)
- Any other relevant info

2 delegate's, `InputKeyEventHandler`, declared as

```
public delegate void InputKeyEventHandler(
  InputEventArgs inputArgs,
  KeyEventArgs eventArgs);
```

and `InputMouseEventHandler`, declared as

```
public delegate void InputMouseEventHandler(
  InputEventArgs inputArgs,
  KeyEventArgs eventArgs);
```

## Creating and using events internally

2 Dictionaries for keyboard and mouse events

- `Dictionary<Keyboard.Key, InputKeyEventHandler> _keyEvents`
- `Dictionary<Mouse.Button, InputMouseEventHandler> _mouseEvents`
