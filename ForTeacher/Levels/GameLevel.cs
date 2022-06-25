using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ForTeacher.Graphics;

namespace ForTeacher.Levels
{
    public enum PlayerOptions
    {
        None,
        Player,
        AI,
        Both
    }

    public enum Phase
    {
        Planning,
        Fighting,
        Over
    }

    public struct PlanningPhaseState
    {
        public ShipType CurrentlyPlacing;
        public bool PlacingVertically;
    }

    public class GameLevel : Level
    {
        // Children of the Level
        public Board PlayerBoard { get; private set; }
        public Board OpponentBoard { get; private set; }
        public GraphicsManager GraphicsManager { get; private set; }
        public AI AI { get; private set; }

        // State
        public bool BlockAction { get; set; } = false;
        public Phase Phase { get; private set; }
        public PlayerOptions Turn { get; private set; }
        public PlayerOptions Winner { get; private set; }
        public PlanningPhaseState PlanningState { get { return _planningState; } }

        // Events
        public event EventHandler<Phase> PhaseChanged;
        /// <summary>
        /// Event for a ship firing. <br/>
        /// Tuple: Player fired, Hit ship, sank ship.
        /// </summary>
        public event EventHandler<(bool, bool, bool)> ShipFired;

        private PlanningPhaseState _planningState;

        public GameLevel()
        {

        }

        public override void Initialize()
        {
            base.Initialize();

            PlayerBoard = new(true);
            OpponentBoard = new(false);
            GraphicsManager = new(PlayerBoard, OpponentBoard);
            AI = new(PlayerBoard, OpponentBoard);

            Phase = Phase.Planning;
            Winner = PlayerOptions.None;

            _planningState.CurrentlyPlacing = ShipType.Carrier;
            _planningState.PlacingVertically = true;

            // Bind planning input
            Input.BindMouseEvent(Mouse.Button.Left, PlacePlayerShip);
            Input.BindKeyEvent(Keyboard.Key.R, RotatePlacingPlayerShip);

            PhaseChanged?.Invoke(this, Phase);
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            if (Winner != PlayerOptions.None)
            {
                if (Phase != Phase.Over)
                {
                    Phase = Phase.Over;
                    PhaseChanged?.Invoke(this, Phase);
                }

                return;
            }

            if (Turn == PlayerOptions.AI && BlockAction == false)
                AITurn();
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            Program.Window.Draw(GraphicsManager);
        }

        public override void Terminate()
        {
            base.Terminate();
        }

        private void PlacePlayerShip(InputEventArgs inputArgs, MouseButtonEventArgs eventArgs)
        {
            if (inputArgs.type != InputEventType.Released)
                return;

            Vector2i? pos = GraphicsManager.PlayerBoard.GetPlayerBoardMousePos();

            if (pos == null)
                return;

            if (PlayerBoard.AddShip(
                pos.Value.X, pos.Value.Y,
                PlanningState.PlacingVertically,
                PlanningState.CurrentlyPlacing))
            {
                if (PlayerBoard.TotalOfType(PlanningState.CurrentlyPlacing) >=
                    Board.MaxOfType(PlanningState.CurrentlyPlacing))
                {
                    if (PlanningState.CurrentlyPlacing < (ShipType)4)
                    {
                        _planningState.CurrentlyPlacing++;
                    }
                    else
                    {
                        DonePlacingPlayerShips();
                    }
                }
            }
        }

        private void RotatePlacingPlayerShip(InputEventArgs inputArgs, KeyEventArgs eventArgs)
        {
            if (inputArgs.type != InputEventType.Pressed)
                return;

            _planningState.PlacingVertically = !PlanningState.PlacingVertically;
        }

        private void DonePlacingPlayerShips()
        {
            PlaceAIShips();

            Phase = Phase.Fighting;
            Turn = PlayerOptions.Player;

            PhaseChanged?.Invoke(this, Phase);

            // Change input bindings for fighting
            Input.UnbindMouseEvent(Mouse.Button.Left, PlacePlayerShip);
            Input.UnbindKeyEvent(Keyboard.Key.R, RotatePlacingPlayerShip);

            Input.BindMouseEvent(Mouse.Button.Left, FireAtOpponent);
        }

        private void PlaceAIShips()
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
                }
                else
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
                        return;
                    }
                }
            }
        }

        private void FireAtOpponent(InputEventArgs inputArgs, MouseButtonEventArgs eventArgs)
        {
            if (inputArgs.type != InputEventType.Released)
                return;

            if (Turn != PlayerOptions.Player || BlockAction)
                return;

            if (GraphicsManager.OpponentBoard.Bounds.Contains(eventArgs.X, eventArgs.Y))
            {
                Vector2i pos = new((int)MathF.Floor(eventArgs.X / 66f) - 1, 9 - (int)MathF.Floor(eventArgs.Y / 66f));
                if (OpponentBoard.Markers[pos.X, pos.Y] == MarkerType.None)
                {
                    // Determine if we are over a ship
                    bool hitShip = false;
                    bool sankShip = false;
                    
                    foreach (Ship ship in OpponentBoard.Ships)
                    {
                        if (ship.IsOn(pos.X, pos.Y))
                        {
                            ship.Hit(pos.X, pos.Y);
                            hitShip = true;
                            sankShip = ship.IsSunk();

                            break;
                        }
                    }

                    OpponentBoard.Markers[pos.X, pos.Y] = hitShip ? MarkerType.Hit : MarkerType.Miss;
                    Turn = PlayerOptions.AI;
                    BlockAction = true;

                    ShipFired?.Invoke(this, (true, hitShip, sankShip));

                    CheckForWinner();
                }
            }
        }

        private void AITurn()
        {
            Vector2i result = AI.DoTurn();

            // Determine if we are over a ship
            bool hitShip = false;
            bool sankShip = false;
            
            foreach (Ship ship in PlayerBoard.Ships)
            {
                if (ship.IsOn(result.X, result.Y))
                {
                    ship.Hit(result.X, result.Y);
                    hitShip = true;
                    sankShip = ship.IsSunk();

                    break;
                }
            }

            PlayerBoard.Markers[result.X, result.Y] = hitShip ? MarkerType.Hit : MarkerType.Miss;
            AI.GiveResults(hitShip, sankShip);
            Turn = PlayerOptions.Player;
            BlockAction = true;

            ShipFired?.Invoke(this, (false, hitShip, sankShip));
            
            CheckForWinner();
        }

        private void CheckForWinner()
        {
            if (PlayerBoard.TotalSunkShips() == Board.MAX_SHIPS)
            {
                Winner = PlayerOptions.AI;
                Console.WriteLine("AI wins!");
            }
            else if (OpponentBoard.TotalSunkShips() == Board.MAX_SHIPS)
            {
                Winner = PlayerOptions.Player;
                Console.WriteLine("Player wins!");
            }
        }
    }
}
