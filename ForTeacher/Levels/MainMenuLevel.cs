using SFML.Graphics;
using ForTeacher.UI;

namespace ForTeacher.Levels
{
    public class MainMenuLevel : Level
    {
        private GameText _title;
        private Button _startButton;
        
        public MainMenuLevel()
        {
            
        }

        public override void Initialize()
        {
            base.Initialize();

            _title = new GameText()
            {
                DisplayedText = "Battleship",
                Position = new(Program.WINDOW_WIDTH / 2, 0),
                HorizontalAlign = HorizontalAlign.Center,
                Margin = new(0, 10),
                CharacterSize = 48
            };

            _startButton = new Button()
            {
                Text = "Play",
                Position = new(Program.WINDOW_WIDTH / 2, Program.WINDOW_HEIGHT / 2),
                Size = new(250, 70),
                TextSize = 40
            };

            _startButton.Clicked += StartGame;
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);
        }

        public override void Draw(RenderTarget target, RenderStates states)
        {
            base.Draw(target, states);

            target.Draw(_title, states);
            target.Draw(_startButton, states);
        }

        public override void Terminate()
        {
            base.Terminate();
        }

        private void StartGame(object? sender, EventArgs args)
        {
            Program.ChangeLevel(new GameLevel());
        }
    }
}
