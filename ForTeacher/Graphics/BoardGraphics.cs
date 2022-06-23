using SFML.Graphics;
using SFML.System;
using SFML.Window;

namespace ForTeacher.Graphics
{
    public class BoardGraphics : Drawable
    {
        public FloatRect Bounds { get; private set; }

        private readonly int cellSize = 66;
        private readonly int size = 66 * 11;

        private Board data;
        private Vector2f offset;

        public BoardGraphics(Board data, Vector2f offset)
        {
            this.data = data;
            this.offset = offset;

            if (data.Player)
                Bounds = new(1920 - size + cellSize, 0, size - cellSize, size - cellSize);
            else
                Bounds = new(cellSize, 0, size - cellSize, size - cellSize);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            DrawGrid(target, states);

            DrawShips(target, states);

            DrawMarkers(target, states);

            DrawShipPlacement(target, states);
        }

        private void DrawGrid(RenderTarget target, RenderStates states)
        {
            VertexArray gridLines = new(PrimitiveType.Lines);
            List<Text> designations = new(20);

            // Lines - Columns
            for (int i = 0; i < Board.SIZE; i++)
            {
                gridLines.Append(new(offset + new Vector2f(i * cellSize + cellSize, 0)));
                gridLines.Append(new(offset + new Vector2f(i * cellSize + cellSize, size)));
            }

            // Lines - Rows
            for (int i = 0; i < Board.SIZE; i++)
            {
                gridLines.Append(new(offset + new Vector2f(0, i * cellSize + cellSize)));
                gridLines.Append(new(offset + new Vector2f(size, i * cellSize + cellSize)));
            }

            // Designations - Columns
            for (int i = 0; i < Board.SIZE; i++)
            {
                Text text = new Text
                {
                    CharacterSize = (uint)(cellSize * (2f / 3f)),
                    Font = ResourceLoader.Get<Font>("OpenSans"),
                    DisplayedString = ((char)('A' + i)).ToString(),
                    Position = offset + new Vector2f(cellSize + cellSize * i + cellSize / 2, size - cellSize / 2)
                };

                FloatRect bounds = text.GetLocalBounds();
                text.Origin = new(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);

                designations.Add(text);
            }

            // Designations - Rows
            for (int i = 0; i < Board.SIZE; i++)
            {
                Text text = new Text
                {
                    CharacterSize = (uint)(cellSize * (2f / 3f)),
                    Font = ResourceLoader.Get<Font>("OpenSans"),
                    DisplayedString = (i + 1).ToString(),
                    Position = offset + new Vector2f(cellSize / 2, size - cellSize + cellSize * -i - cellSize / 2)
                };

                FloatRect bounds = text.GetLocalBounds();
                text.Origin = new(bounds.Left + bounds.Width / 2, bounds.Top + bounds.Height / 2);

                designations.Add(text);
            }

            target.Draw(gridLines, states);

            foreach (Text text in designations)
            {
                target.Draw(text, states);
            }
        }

        private void DrawShips(RenderTarget target, RenderStates states)
        {
            foreach (Ship ship in data.Ships)
            {
                if (ship == null)
                    continue;

                //if (data.Player == false && ship.IsSunk() == false)
                //    continue;

                RectangleShape rect = new()
                {
                    FillColor = Program.SHIP_COLOR,
                    Size = new(cellSize, cellSize * Ship.FindLength(ship.Type))
                };

                rect.Origin = new(0, rect.Size.Y);
                rect.Position = offset + new Vector2f((ship.X + 1) * cellSize, size - (ship.Y + 1) * cellSize);

                if (ship.Vertical == false)
                {
                    rect.Rotation = 90;
                    rect.Position -= new Vector2f(0, cellSize);
                }

                rect.Texture = Ship.GetTexture(ship.Type);

                target.Draw(rect, states);
            }
        }

        private void DrawMarkers(RenderTarget target, RenderStates states)
        {
            for (int x = 0; x < Board.SIZE; x++)
            {
                for (int y = 0; y < Board.SIZE; y++)
                {
                    MarkerType marker = data.Markers[x, y];

                    if (marker != MarkerType.None)
                    {
                        RectangleShape rect = new RectangleShape
                        {
                            Origin = new(0, cellSize),
                            Position = offset + new Vector2f(x * cellSize + cellSize, size - cellSize - y * cellSize),
                            Size = new Vector2f(cellSize, cellSize)
                        };

                        if (marker == MarkerType.Miss)
                        {
                            rect.Texture = new Texture(ResourceLoader.Get<Image>("MissMarker"));
                        }
                        else
                        {
                            rect.Texture = new Texture(ResourceLoader.Get<Image>("HitMarker"));
                        }

                        target.Draw(rect, states);
                    }
                }
            }
        }

        private void DrawShipPlacement(RenderTarget target, RenderStates states)
        {
            if (Program.PlanningPhase == false || data.Player == false)
                return;

            Vector2i? nullablePos = GetPlayerBoardMousePos();

            if (nullablePos == null)
                return;

            Vector2i pos = (Vector2i)nullablePos;

            bool valid = data.ShipPlacementValid(pos.X, pos.Y, Program.PlacingVertically, Program.CurrentlyPlacing);

            RectangleShape rect = new RectangleShape
            {
                Position = offset + new Vector2f((pos.X + 1) * cellSize, size - (pos.Y + 1) * cellSize),
                Size = new(cellSize, cellSize * Ship.FindLength(Program.CurrentlyPlacing)),
                Origin = new(0, Ship.FindLength(Program.CurrentlyPlacing) * cellSize),
                FillColor = valid ? Color.Green : Color.Red,
                Rotation = Program.PlacingVertically ? 0 : 90,
            };

            // Adjust origin if rotating
            if (Program.PlacingVertically == false)
                rect.Position -= new Vector2f(0, cellSize);

            rect.Texture = Ship.GetTexture(Program.CurrentlyPlacing);

            target.Draw(rect, states);
        }

        public Vector2i? GetPlayerBoardMousePos()
        {
            Vector2i mousePos = Mouse.GetPosition(Program.Window);

            if (Bounds.Contains(mousePos.X, mousePos.Y) == false)
                return null;

            // Convert mouse position
            mousePos.X -= (1920 - cellSize * 11);

            return new((int)MathF.Floor(mousePos.X / 66f) - 1, 9 - (int)MathF.Floor(mousePos.Y / 66f));
        }
    }
}
