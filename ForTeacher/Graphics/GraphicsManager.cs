using SFML.System;
using SFML.Graphics;
using ForTeacher.UI;

namespace ForTeacher.Graphics
{
    public class GraphicsManager : Drawable
    {
        public BoardGraphics PlayerBoard { get; init; }
        public BoardGraphics OpponentBoard { get; init; }
        public UIManager UI { get; init; }

        public GraphicsManager(Board playerBoard, Board opponentBoard)
        {
            PlayerBoard = new BoardGraphics(playerBoard, new Vector2f(1920 - 66 * 11, 0));
            OpponentBoard = new BoardGraphics(opponentBoard, new Vector2f());
            UI = new UIManager();
        }

        public void Draw(RenderTarget target, RenderStates states)
        {
            PlayerBoard.Draw(target, states);
            OpponentBoard.Draw(target, states);
            UI.Draw(target, states);
        }
    }
}
