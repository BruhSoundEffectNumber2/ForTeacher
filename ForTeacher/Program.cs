using SFML.System;
using SFML.Window;
using SFML.Graphics;
using ForTeacher.UI;
using ForTeacher.Graphics;

namespace ForTeacher
{
    public class Program
    {
        public static readonly Color BACKGROUND_COLOR = new Color(25, 25, 25);
        public static readonly Color BORDER_COLOR = new Color(27, 171, 67);
        public static readonly Color GRID_COLOR = new Color(27, 200, 67);
        public static readonly Color SHIP_COLOR = new Color(46, 181, 42);

        public static readonly uint WINDOW_WIDTH = 1920;
        public static readonly uint WINDOW_HEIGHT = 1080;

        public static int TargetFPS { get; private set; } = 144;
        public static float TimeUntilUpdate { get; private set; } = 1f / TargetFPS;

        public static float DeltaTime { get; private set; }
        public static float TotalTimeElapsed { get; private set; }

        public static RenderWindow Window;
        public static Levels.Level CurrentLevel;

        public static void Main()
        {
            // Load Resources - Ships
            ResourceLoader.Load(ResourceType.Font, "Graphics/OpenSans.ttf", "OpenSans");
            ResourceLoader.Load(ResourceType.Image, "Graphics/Ships/Carrier.png", "Carrier");
            ResourceLoader.Load(ResourceType.Image, "Graphics/Ships/Battleship.png", "Battleship");
            ResourceLoader.Load(ResourceType.Image, "Graphics/Ships/Destroyer.png", "Destroyer");
            ResourceLoader.Load(ResourceType.Image, "Graphics/Ships/Submarine.png", "Submarine");
            ResourceLoader.Load(ResourceType.Image, "Graphics/Ships/PatrolBoat.png", "PatrolBoat");

            // Load Resources - Markers
            ResourceLoader.Load(ResourceType.Image, "Graphics/Board/Hit.png", "HitMarker");
            ResourceLoader.Load(ResourceType.Image, "Graphics/Board/Miss.png", "MissMarker");

            // Setup Window
            ContextSettings settings = new();
            settings.AntialiasingLevel = 8;

            Window = new RenderWindow(
                new VideoMode(WINDOW_WIDTH, WINDOW_HEIGHT), 
                "Stuff", 
                Styles.Close,
                settings
            );

            // Events
            Window.Closed += (s, e) =>
            {
                Window.Close();
            };

            // Main Game Loop
            double totalTimeBeforeUpdate = 0;
            double previousTimeElapsed = 0;
            double deltaTime = 0;
            double totalTimeElapsed = 0;

            Clock clock = new();

            CurrentLevel = new Levels.MainMenuLevel();
            CurrentLevel.Initialize();

            while (Window.IsOpen)
            {
                Window.DispatchEvents();

                // Timing
                totalTimeElapsed = clock.ElapsedTime.AsSeconds();
                deltaTime = totalTimeElapsed - previousTimeElapsed;
                previousTimeElapsed = totalTimeElapsed;

                totalTimeBeforeUpdate += deltaTime;

                if (totalTimeBeforeUpdate >= TimeUntilUpdate)
                {
                    // DeltaTime
                    DeltaTime = (float)totalTimeBeforeUpdate;
                    TotalTimeElapsed = (float)totalTimeElapsed;
                    totalTimeBeforeUpdate = 0;

                    // Update
                    CurrentLevel.Update(DeltaTime);

                    // Graphics
                    Window.Clear(BACKGROUND_COLOR);
                    Window.Draw(CurrentLevel);
                    Window.Display();
                }
            }
        }
    }
}