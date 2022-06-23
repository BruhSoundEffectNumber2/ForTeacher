using SFML.Graphics;
using SFML.Window;
using SFML.System;
using ForTeacher.Graphics;

namespace ForTeacher
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

        public PlanningPhaseState()
        {
            CurrentlyPlacing = ShipType.Carrier;
            PlacingVertically = true;
        }
    }

    public class GameLevel : Level
    {
        // Children of Level
        public Board PlayerBoard { get; private set; }
        public Board OpponentBoard { get; private set; }
        public GraphicsManager GraphicsManager { get; private set; }
        public AI AI { get; private set; }

        // State
        public Phase Phase { get; private set; }
        public PlayerOptions Winner { get; private set; }

        private PlanningPhaseState planningState;

        public GameLevel()
        {
            PlayerBoard = new(true);
            OpponentBoard = new(false);
            GraphicsManager = new(PlayerBoard, OpponentBoard);
            AI = new(PlayerBoard, OpponentBoard);

            Phase = Phase.Planning;
            Winner = PlayerOptions.None;
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            Program.Window.Draw(GraphicsManager);
        }

        public override void Initialize()
        {
            base.Initialize();
        }

        public override void Terminate()
        {
            base.Terminate();
        }
    }
}
