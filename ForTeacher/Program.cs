using SFML.System;
using SFML.Window;
using SFML.Graphics;

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
        public static AudioSystem.AudioManager AudioManager;
        public static Levels.Level CurrentLevel;

        // Events
        public static event EventHandler LevelChanged;

        public static void Main()
        {
            // Load Resources - Fonts
            ResourceLoader.Load<Font>("Graphics/OpenSans.ttf", "OpenSans");

            // Load Resources - Ships
            ResourceLoader.Load<Image>("Graphics/Ships/Carrier.png", "Carrier");
            ResourceLoader.Load<Image>("Graphics/Ships/Battleship.png", "Battleship");
            ResourceLoader.Load<Image>("Graphics/Ships/Destroyer.png", "Destroyer");
            ResourceLoader.Load<Image>("Graphics/Ships/Submarine.png", "Submarine");
            ResourceLoader.Load<Image>("Graphics/Ships/PatrolBoat.png", "PatrolBoat");

            // Load Resources - Markers
            ResourceLoader.Load<Image>("Graphics/Board/Hit.png", "HitMarker");
            ResourceLoader.Load<Image>("Graphics/Board/Miss.png", "MissMarker");

            // Load Resources - Ambience
            ResourceLoader.Load<SFML.Audio.Music>("Audio/Ambience/ShipInside.wav", "ShipInside");

            // Load Resources - Effects
            ResourceLoader.Load<SFML.Audio.SoundBuffer>("Audio/Effects/MissileLaunch.wav", "MissileLaunch");
            ResourceLoader.Load<SFML.Audio.SoundBuffer>("Audio/Effects/ImpactMiss.wav", "ImpactMiss");
            ResourceLoader.Load<SFML.Audio.SoundBuffer>("Audio/Effects/ImpactHit.wav", "ImpactHit");
            ResourceLoader.Load<SFML.Audio.SoundBuffer>("Audio/Effects/ImpactSink.wav", "ImpactSink");

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
                // FMOD needs to be shut down before the window is closed
                AudioManager.Shutdown();

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

            AudioManager = new();

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
                    AudioManager.Update();

                    // Graphics
                    Window.Clear(BACKGROUND_COLOR);
                    Window.Draw(CurrentLevel);
                    Window.Display();
                }
            }
        }

        public static void ChangeLevel(Levels.Level level)
        {
            CurrentLevel.Terminate();
            CurrentLevel = level;
            CurrentLevel.Initialize();
            LevelChanged?.Invoke(null, EventArgs.Empty);
        }
    }
}