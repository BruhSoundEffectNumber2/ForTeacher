using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace ForTeacher.UI
{
    public class Button : Drawable
    {
        public bool Visible = true;
        public Vector2f Position;
        public Vector2f Size = new(150, 45);
        public HorizontalAlign HorizontalAlign = HorizontalAlign.Center;
        public VerticalAlign VerticalAlign = VerticalAlign.Center;

        public Color Color = new(255, 255, 255);
        public string Text = "Button";
        public uint TextSize = 24;
        public Color TextColor = new(10, 10, 10);

        public event EventHandler? Clicked;

        private GameText text;
        private RectangleShape rect;
        private FloatRect bounds;

        private bool _wasMouseOver = false;

        public Button()
        {
            text = new GameText()
            {
                HorizontalAlign = HorizontalAlign.Center,
                VerticalAlign = VerticalAlign.Center
            };

            bounds = new();
            rect = new();

            Update();

            Input.BindMouseEvent(Mouse.Button.Left, OnButtonClicked);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Update();

            if (Visible == false)
                return;

            target.Draw(rect, states);
            target.Draw(text, states);
        }

        private void OnButtonClicked(InputEventArgs inputArgs, MouseButtonEventArgs eventArgs)
        {
            if (inputArgs.type == InputEventType.Pressed)
            {
                if (bounds.Contains(eventArgs.X, eventArgs.Y))
                {
                    _wasMouseOver = true;
                }
            } else
            {
                if (bounds.Contains(eventArgs.X, eventArgs.Y))
                {
                    if (_wasMouseOver)
                    {
                        Clicked?.Invoke(this, EventArgs.Empty);
                    }
                }

                _wasMouseOver = false;
            }
        }

        private void Update()
        {
            // Update bounds
            bounds.Left = HorizontalAlign switch
            {
                HorizontalAlign.Left => Position.X,
                HorizontalAlign.Center => Position.X - Size.X / 2,
                HorizontalAlign.Right => Position.X - Size.X,
                _ => throw new ArgumentOutOfRangeException("Horiz alignment enum out of bounds.")
            };

            bounds.Top = VerticalAlign switch
            {
                VerticalAlign.Top => Position.Y,
                VerticalAlign.Center => Position.Y - Size.Y / 2,
                VerticalAlign.Bottom => Position.Y - Size.Y,
                _ => throw new ArgumentOutOfRangeException("Vert alignment enum out of bounds.")
            };

            bounds.Width = Size.X;
            bounds.Height = Size.Y;

            // Update rect
            rect.Position = new(bounds.Left, bounds.Top);
            rect.Size = new(bounds.Width, bounds.Height);
            rect.FillColor = Color;

            // Update text
            text.DisplayedText = Text;
            text.FillColor = TextColor;
            text.CharacterSize = TextSize;
            text.Position = rect.Position + new Vector2f(Size.X / 2, Size.Y / 2);
            text.Margin = new Vector2f(0, 0);
        }
    }
}
