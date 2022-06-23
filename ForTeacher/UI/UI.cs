using SFML.System;
using SFML.Graphics;

namespace ForTeacher.UI
{
    public class UIManager : Drawable
    {
        private GameText title;

        private GameText phase;

        private GameText playerLabel;
        private GameText playerShipsLabel;

        private GameText opponentLabel;
        private GameText opponentShipsLabel;

        private Button gameOverButton;

        public UIManager()
        {
            title = new GameText
            {
                DisplayedText = "Battleship",
                CharacterSize = 48,
                HorizontalAlign = HorizontalAlign.Center,
                Position = new Vector2f(960, 0)
            };

            phase = new GameText
            {
                CharacterSize = 30,
                HorizontalAlign = HorizontalAlign.Center,
                Position = new(960, 60)
            };

            playerLabel = new GameText
            {
                DisplayedText = "Player",
                CharacterSize = 30,
                Position = new Vector2f(960, 66 * 11),
                Margin = new(20, 0)
            };

            opponentLabel = new GameText
            {
                DisplayedText = "Opponent",
                CharacterSize = 30,
                HorizontalAlign = HorizontalAlign.Right,
                Position = new Vector2f(960, 66 * 11),
                Margin = new(20, 0)
            };
             
            playerShipsLabel = new GameText
            {
                DisplayedText = "Sunken Ships - X Left",
                CharacterSize = 22,
                Position = new Vector2f(960, 66 * 11 + 40),
                Margin = new(20, 0)
            };

            opponentShipsLabel = new GameText
            {
                DisplayedText = "Sunken Ships - X Left",
                CharacterSize = 22,
                HorizontalAlign = HorizontalAlign.Right,
                Position = new Vector2f(960, 66 * 11 + 40),
                Margin = new(20, 0)
            };

            gameOverButton = new Button
            {
                Text = "Return to Menu",
                Size = new(250, 60),
                TextSize = 32,
                Position = new Vector2f(960, 200),
                Visible = false
            };

            gameOverButton.LMBClicked += (s, e) =>
            {
                Console.WriteLine("Returning to menu...");
            };
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            DrawBorders(target, states);
            DrawShips(target, states);
            DrawText(target, states);
        }

        private void DrawBorders(RenderTarget target, RenderStates states)
        {
            VertexArray vertices = new(PrimitiveType.Lines);

            // Player board
            vertices.Append(new(
                new Vector2f(1920 - 66 * 11, 0),
                Program.BORDER_COLOR
            ));

            vertices.Append(new(
                new Vector2f(1920 - 66 * 11, 66 * 11),
                Program.BORDER_COLOR
            ));

            // Opponent board
            vertices.Append(new(
                new Vector2f(66 * 11, 0),
                Program.BORDER_COLOR
            ));

            vertices.Append(new(
                new Vector2f(66 * 11, 66 * 11),
                Program.BORDER_COLOR
            ));

            // Both boards and bottom
            vertices.Append(new(
                new Vector2f(0, 66 * 11),
                Program.BORDER_COLOR
            ));

            vertices.Append(new(
                new Vector2f(1920, 66 * 11),
                Program.BORDER_COLOR
            ));

            // Divide player and opponent sides
            vertices.Append(new(
                new Vector2f(1920 / 2, 66 * 11),
                Program.BORDER_COLOR
            ));

            vertices.Append(new(
                new Vector2f(1920 / 2, 1080),
                Program.BORDER_COLOR
            ));

            target.Draw(vertices, states);
        }

        private void DrawText(RenderTarget target, RenderStates states)
        {
            title.Draw(target, states);
            playerLabel.Draw(target, states);
            opponentLabel.Draw(target, states);

            playerShipsLabel.DisplayedText = "Sunken Ships - " + Program.PlayerBoard.TotalSunkShips();
            playerShipsLabel.DisplayedText += " / " + Board.MAX_SHIPS;
            playerShipsLabel.Draw(target, states);

            opponentShipsLabel.DisplayedText = "Sunken Ships - " + Program.OpponentBoard.TotalSunkShips();
            opponentShipsLabel.DisplayedText += " / " + Board.MAX_SHIPS;
            opponentShipsLabel.Draw(target, states);

            if (Program.PlanningPhase)
            {
                phase.DisplayedText = "Player, place your ships.";
            }
            else
            {
                phase.DisplayedText = "Game is ongoing.";
            }

            if (Program.GameOver != 0)
            {
                phase.DisplayedText = "Game Over. ";
                phase.DisplayedText += Program.GameOver switch
                {
                    1 => "Player wins!",
                    2 => "AI wins!",
                    _ => "ERROR W/ GameOver"
                };
                gameOverButton.Visible = true;
            }

            phase.Draw(target, states);

            gameOverButton.Draw(target, states);
        }

        private void DrawShips(RenderTarget target, RenderStates states)
        {
            // We don't need to draw any sunken ships when planning
            if (Program.PlanningPhase)
                return;

            int i = 0;

            // Player's sunken ships
            foreach (Ship ship in Program.PlayerBoard.Ships)
            {
                if (ship.IsSunk() == false)
                    continue;

                RectangleShape rect = new RectangleShape
                {
                    FillColor = Program.SHIP_COLOR,
                    Size = new(50, 50 * Ship.FindLength(ship.Type)),
                    Position = new(i * 55 + 1920 / 2, 66 * 11 + 60)
                };

                rect.Texture = Ship.GetTexture(ship.Type);

                target.Draw(rect, states);

                i++;
            }

            i = 0;

            // Opponent's sunken ships
            int opponentTotal = 0;
            foreach (Ship ship in Program.OpponentBoard.Ships)
                if (ship.IsSunk())
                    opponentTotal++;

            foreach (Ship ship in Program.OpponentBoard.Ships)
            {
                if (ship.IsSunk() == false)
                    continue;

                RectangleShape rect = new RectangleShape
                {
                    FillColor = Program.SHIP_COLOR,
                    Size = new(50, 50 * Ship.FindLength(ship.Type)),
                    Position = new(-(opponentTotal - i) * 55 + 960, 66 * 11 + 60)
                };

                rect.Texture = new Texture(ship.Type switch
                {
                    ShipType.Carrier => ResourceLoader.Get<Image>("Carrier"),
                    ShipType.Battleship => ResourceLoader.Get<Image>("Battleship"),
                    ShipType.Destroyer => ResourceLoader.Get<Image>("Destroyer"),
                    ShipType.Submarine => ResourceLoader.Get<Image>("Submarine"),
                    ShipType.PatrolBoat => ResourceLoader.Get<Image>("PatrolBoat"),
                    _ => throw new Exception("ShipType is out of bounds")
                });

                target.Draw(rect, states);
                i++;
            }
        }
    }
}
