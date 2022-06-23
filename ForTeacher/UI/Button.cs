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

        public event EventHandler? LMBClicked;

        private GameText text;
        private RectangleShape rect;
        private FloatRect bounds;

        private bool lmbPressed = false;

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

            Program.Window.MouseButtonPressed += MouseButtonPressed;
            Program.Window.MouseButtonReleased += MouseButtonReleased;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            Update();

            if (Visible == false)
                return;

            target.Draw(rect, states);
            target.Draw(text, states);
        }

        private void MouseButtonPressed(object? sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button != SFML.Window.Mouse.Button.Left || Visible == false)
                return;

            if (lmbPressed)
                return;

            Vector2i mousePos = Mouse.GetPosition(Program.Window);

            if (bounds.Contains(mousePos.X, mousePos.Y) == false)
                return;

            lmbPressed = true;
        }

        private void MouseButtonReleased(object? sender, SFML.Window.MouseButtonEventArgs e)
        {
            if (e.Button != SFML.Window.Mouse.Button.Left || Visible == false)
                return;

            if (lmbPressed == false)
                return;

            Vector2i mousePos = Mouse.GetPosition(Program.Window);

            if (bounds.Contains(mousePos.X, mousePos.Y) == false)
                return;

            lmbPressed = false;

            LMBClicked?.Invoke(this, new());
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
