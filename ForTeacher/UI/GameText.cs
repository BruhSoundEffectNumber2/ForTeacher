using SFML.System;
using SFML.Graphics;

namespace ForTeacher.UI
{
    public enum HorizontalAlign
    {
        Left,
        Center,
        Right,
    }

    public enum VerticalAlign
    {
        Top,
        Center,
        Bottom,
    }

    public class GameText : Drawable
    {
        public Text Internal { get; private set; }
        public string Font
        {
            get { return fontName; }
            set
            {
                fontName = value;
                Internal.Font = ResourceLoader.Get<Font>(fontName);
            }
        }

        public Vector2f Position { get; set; }
        public Vector2f Margin { get; set; }
        public string DisplayedText
        {
            get { return Internal.DisplayedString; } set { Internal.DisplayedString = value; }
        }

        public uint CharacterSize
        {
            get { return Internal.CharacterSize; } set { Internal.CharacterSize = value; }
        }

        public Color FillColor
        {
            get { return Internal.FillColor; } set { Internal.FillColor = value; }
        }

        public HorizontalAlign HorizontalAlign { get; set; }
        public VerticalAlign VerticalAlign { get; set; }
        public FloatRect Bounds
        {
            get
            {
                FloatRect bounds = Internal.GetLocalBounds();
                bounds.Width += Margin.X;
                bounds.Height += Margin.Y;

                return bounds;
            }
        }

        private string fontName;

        public GameText()
        {
            Internal = new Text();

            // Default values
            fontName = "OpenSans";
            DisplayedText = "Text";
            FillColor = Color.White;
            CharacterSize = 18;
            Font = "OpenSans";
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            FloatRect localBounds = Internal.GetLocalBounds();

            // Set the internal position based on text alignment
            float horizontal = HorizontalAlign switch
            {
                HorizontalAlign.Left => 0 + Margin.X,
                HorizontalAlign.Center => -localBounds.Left - localBounds.Width / 2,
                HorizontalAlign.Right => -localBounds.Width - Margin.X,
                _ => 0
            };

            float vertical = VerticalAlign switch
            {
                VerticalAlign.Top => Margin.Y,
                VerticalAlign.Center => -localBounds.Top - localBounds.Height / 2,
                VerticalAlign.Bottom => -localBounds.Height - Margin.Y,
                _ => 0
            };

            Internal.Position = Position + new Vector2f(horizontal, vertical);
            
            target.Draw(Internal, states);
        }
    }
}
