using SFML.System;
using SFML.Window;
using SFML.Graphics;

namespace ForTeacher
{
    public class Program
    {
        public static readonly Color BACKGROUND_COLOR = new Color(10, 10, 10);
        public static readonly Color BORDER_COLOR = new Color(27, 171, 67);
        public static readonly Color GRID_COLOR = new Color(27, 200, 67);

        public static readonly uint WINDOW_WIDTH = 1920;
        public static readonly uint WINDOW_HEIGHT = 1080;

        public static Logic.Board Board;
        public static Graphics.GraphicsManager GraphicsManager;

        public static void Main()
        {
            ContextSettings settings = new();
            settings.AntialiasingLevel = 8;

            RenderWindow window = new RenderWindow(
                new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), 
                "Stuff", 
                Styles.Close,
                settings
            );

            GraphicsManager = new Graphics.GraphicsManager(Board);
            
            while (window.IsOpen)
            {
                window.WaitAndDispatchEvents();

                window.Closed += (s, e) =>
                {
                    window.Close();
                };

                window.Clear(BACKGROUND_COLOR);
                window.Draw(GraphicsManager);
                window.Display();
            }
        }
    }
}