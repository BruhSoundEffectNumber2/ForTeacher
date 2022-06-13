using SFML.Graphics;
using SFML.System;

namespace ForTeacher.Graphics
{
    public class Board : Drawable
    {
        private Logic.Board LogicBoard;
        private Font GridFont;

        public Board(Logic.Board logicBoard)
        {
            GridFont = new Font("Resources/Graphics/OpenSans.ttf");
            LogicBoard = logicBoard;
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            // Draw the background grid
            target.Draw(GridVertices(900, new Vector2f()), states);

            // Draw the grid designations
            foreach (Text text in GridDesignations(900, new Vector2f()))
            {
                if (text != null)
                    target.Draw(text, states);
            }
        }

        private VertexArray GridVertices(uint size, Vector2f offset)
        {
            float cellSize = size / (Logic.Board.SIZE + 1f);
            VertexArray vertices = new VertexArray(PrimitiveType.Lines);

            // Rows
            for (int i = 0; i <= Logic.Board.SIZE + 1; i++)
            {
                vertices.Append(
                    new Vertex(offset + new Vector2f(0, cellSize * i))
                );

                vertices.Append(
                    new Vertex(offset + new Vector2f(size, cellSize * i))
                );
            }

            // Columns
            for (int i = 0; i <= Logic.Board.SIZE + 1; i++)
            {
                vertices.Append(
                    new Vertex(offset + new Vector2f(cellSize * i, 0))
                );

                vertices.Append(
                    new Vertex(offset + new Vector2f(cellSize * i, size))
                );
            }

            return vertices;
        }

        private Text[] GridDesignations(uint size, Vector2f offset)
        {
            float cellSize = size / (Logic.Board.SIZE + 1f);
            Text[] designations = new Text[20];

            designations[0] = new Text 
            { 
                CharacterSize = (uint)(cellSize * (2f/3f)), 
                Font = GridFont, 
                DisplayedString = "1",
                Position = offset + new Vector2f(cellSize * 1, size),
                Origin = new Vector2f(-12, cellSize * (2f / 3f) + 12),
            };

            return designations;
        }
    }

    public class GraphicsManager : Drawable
    {
        public Board Board { get; init; }

        public GraphicsManager(Logic.Board logicBoard)
        {
            Board = new Board(logicBoard);
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            // Draw the borders between parts of the UI
            //DrawBorders(target, states);

            Board.Draw(target, states);
        }

        private void DrawBorders(RenderTarget target, RenderStates states)
        {
            // Divider between the board and info sections
            target.Draw(
                new RectangleShape
                {
                    Size = new Vector2f(Program.WINDOW_WIDTH, 4),
                    FillColor = Program.BORDER_COLOR,
                    Origin = new Vector2f(0, 2),
                    Position = new Vector2f(0, 900)
                },
                states
            );

            // Divider between the player and opponent boards
            target.Draw(
                new RectangleShape
                {
                    Size = new Vector2f(4, Program.WINDOW_HEIGHT),
                    FillColor = Program.BORDER_COLOR,
                    Origin = new Vector2f(2, 0),
                    Position = new Vector2f(900, 0)
                },
                states
            );

            target.Draw(
                new RectangleShape
                {
                    Size = new Vector2f(4, Program.WINDOW_HEIGHT),
                    FillColor = Program.BORDER_COLOR,
                    Origin = new Vector2f(2, 0),
                    Position = new Vector2f(Program.WINDOW_WIDTH - 900, 0)
                },
                states
            );
        }
    }
}
