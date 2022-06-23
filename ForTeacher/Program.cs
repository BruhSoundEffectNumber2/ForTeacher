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

        public static RenderWindow Window;
        public static Board PlayerBoard;
        public static Board OpponentBoard;
        public static GraphicsManager GraphicsManager;
        public static AI AI;

        // To ensure that input events are only called once
        private static bool lmbReleased = false;
        private static bool keyPressed = false;

        // Is the player currently placing their ships?
        public static bool PlanningPhase = true;
        public static ShipType CurrentlyPlacing = ShipType.Carrier;
        public static bool PlacingVertically = true;

        public static int GameOver = 0;

        public static float DeltaTime { get; private set; }
        public static float TotalTimeElapsed { get; private set; }

        private static GameText frameTimeText;

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

            // Create Boards
            PlayerBoard = new Board(true);
            OpponentBoard = new Board(false);

            // Create AI
            AI = new(PlayerBoard, OpponentBoard);

            // Create Graphics
            GraphicsManager = new(PlayerBoard, OpponentBoard);

            frameTimeText = new GameText
            {
                VerticalAlign = VerticalAlign.Bottom,
                Margin = new(5, 5),
                Position = new(0, 1080)
            };

            // Temporary, place ships
            //PlayerBoard.AddShip(0, 0, true, ShipType.Carrier);

            //PlayerBoard.AddShip(6, 6, true, ShipType.Battleship);

            //PlayerBoard.AddShip(9, 1, true, ShipType.Destroyer);
            //PlayerBoard.AddShip(0, 9, false, ShipType.Destroyer);

            //PlayerBoard.AddShip(3, 1, true, ShipType.Submarine);
            //PlayerBoard.AddShip(3, 5, false, ShipType.Submarine);

            //PlayerBoard.AddShip(4, 7, false, ShipType.PatrolBoat);
            //PlayerBoard.AddShip(6, 1, true, ShipType.PatrolBoat);
            //PlayerBoard.AddShip(9, 8, true, ShipType.PatrolBoat);

            //PlaceAIShips();

            // Events
            Window.Closed += (s, e) =>
            {
                Window.Close();
            };

            Window.MouseButtonReleased += MouseButtonReleased;
            Window.KeyPressed += KeyPressed;

            // Main Game Loop
            double totalTimeBeforeUpdate = 0;
            double previousTimeElapsed = 0;
            double deltaTime = 0;
            double totalTimeElapsed = 0;

            Clock clock = new();

            Input.BindKeyEvent(Keyboard.Key.Space, InputEventType.Pressed, (s, e) =>
            {
                Console.WriteLine("Space pressed.");
            });

            Input.BindKeyEvent(Keyboard.Key.Space, InputEventType.Released, (s, e) =>
            {
                Console.WriteLine("Space released.");
            });

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

                    // Graphics
                    Window.Clear(BACKGROUND_COLOR);
                    Window.Draw(GraphicsManager);
                    frameTimeText.DisplayedText = "FPS: ";
                    frameTimeText.DisplayedText += (1f / DeltaTime).ToString("F2");
                    Window.Draw(frameTimeText);
                    Window.Display();

                    // Clear input flags
                    // TODO: This setup only allows one event call per frame, which might cause issues
                    lmbReleased = false;
                    keyPressed = false;
                }
            }
        }

        private static void KeyPressed(object? sender, KeyEventArgs e)
        {
            if (keyPressed)
                return;

            keyPressed = true;

            if (e.Code == Keyboard.Key.R)
            {
                if (PlanningPhase)
                {
                    PlacingVertically = !PlacingVertically;
                }
            }
        }

        private static void MouseButtonReleased(object? sender, MouseButtonEventArgs e)
        {
            if (lmbReleased)
                return;

            if (e.Button != Mouse.Button.Left)
                return;

            lmbReleased = true;

            if (PlanningPhase == false && GameOver == 0)
            {
                if (FireAtEnemy(e))
                {
                    if (IsGameOver())
                    {
                        EndGame();
                        return;
                    }

                    AITurn();

                    if (IsGameOver())
                        EndGame();
                }
            } else if (GameOver == 0)
            {
                if (PlacePlayerShip())
                {
                    PlaceAIShips();
                }
            }
        }

        private static bool IsGameOver()
        {
            if (PlayerBoard.TotalSunkShips() == Board.MAX_SHIPS)
            {
                GameOver = 2;
                return true;
            }

            if (OpponentBoard.TotalSunkShips() == Board.MAX_SHIPS)
            {
                GameOver = 1;
                return true;
            }

            GameOver = 0;
            return false;
        }

        private static void EndGame()
        {
            Console.WriteLine("Game Over");
        }

        private static bool PlacePlayerShip()
        {
            Vector2i? nullablePos = GraphicsManager.PlayerBoard.GetPlayerBoardMousePos();

            if (nullablePos != null)
            {
                Vector2i pos = (Vector2i)nullablePos;

                if (PlayerBoard.AddShip(pos.X, pos.Y, PlacingVertically, CurrentlyPlacing))
                {
                    if (PlayerBoard.TotalOfType(CurrentlyPlacing) >= Board.MaxOfType(CurrentlyPlacing))
                    {
                        // After adding the ship correctly, go to the next ship if we have placed all of the current type
                        CurrentlyPlacing++;

                        // If we have placed all 5 types, we are done planning and can start the game
                        if (CurrentlyPlacing > ShipType.PatrolBoat)
                        {
                            return true;
                        }
                    }
                }
            }

            return false;
        }

        private static void PlaceAIShips()
        {
            // Properties for what ship the AI is currently placing
            ShipType type = ShipType.Carrier; // Always start with the carrier
            Vector2i pos;
            bool vertical;

            while (OpponentBoard.TotalPlacedShips() < Board.MAX_SHIPS)
            {
                // Inclusive bounds for where a ship can be placed
                int placementMaxX;
                int placementMaxY;

                // Choose horizontal or vertical
                vertical = Random.Shared.NextDouble() >= 0.5;

                // Restrict the area that the ship can be placed on
                if (vertical)
                {
                    placementMaxX = 9;
                    placementMaxY = Board.SIZE - Ship.FindLength(type);
                } else
                {
                    placementMaxX = Board.SIZE - Ship.FindLength(type);
                    placementMaxY = 9;
                }

                // Place the ship at a random spot on the board
                pos.X = Random.Shared.Next(placementMaxX + 1);
                pos.Y = Random.Shared.Next(placementMaxY + 1);

                /*
                 * If the ship placement was invalid, it will not be added to the board
                 * and the system will try and place the ship again.
                 */
                OpponentBoard.AddShip(pos.X, pos.Y, vertical, type);

                // Determine which ship we are placing next
                if (OpponentBoard.TotalOfType(type) >= Board.MaxOfType(type))
                {
                    // Done with all ships of the current type, so move on to the next
                    type++;

                    // If we have gone through all 5 types of ships we can place, we are done
                    if (type > ShipType.PatrolBoat)
                    {
                        PlanningPhase = false;
                        return;
                    }
                }
            }
        }

        private static bool FireAtEnemy(MouseButtonEventArgs e)
        {
            if (GraphicsManager.OpponentBoard.Bounds.Contains(e.X, e.Y))
            {
                Vector2i pos = new((int)MathF.Floor(e.X / 66f) - 1, 9 - (int)MathF.Floor(e.Y / 66f));
                if (OpponentBoard.Markers[pos.X, pos.Y] == MarkerType.None)
                {
                    // Determine if we are over a ship
                    foreach (Ship ship in OpponentBoard.Ships)
                    {
                        if (ship.IsOn(pos.X, pos.Y))
                        {
                            ship.Hit(pos.X, pos.Y);
                            OpponentBoard.Markers[pos.X, pos.Y] = MarkerType.Hit;
                            return true;
                        }
                    }

                    OpponentBoard.Markers[pos.X, pos.Y] = MarkerType.Miss;
                    return true;
                }
            }

            return false;
        }

        private static void AITurn()
        {
            Vector2i result = AI.DoTurn();

            // Determine if we are over a ship
            foreach (Ship ship in PlayerBoard.Ships)
            {
                if (ship.IsOn(result.X, result.Y))
                {
                    ship.Hit(result.X, result.Y);
                    PlayerBoard.Markers[result.X, result.Y] = MarkerType.Hit;
                    AI.GiveResults(true, ship.IsSunk());
                    return;
                }
            }

            PlayerBoard.Markers[result.X, result.Y] = MarkerType.Miss;
            AI.GiveResults(false, false);
        }
    }
}