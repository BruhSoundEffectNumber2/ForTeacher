using Studio = FMOD.Studio;

namespace ForTeacher.AudioSystem
{
    public class AudioManager
    {
        public MusicManager MusicManager { get; private set; }
        public AmbienceManager AmbienceManager { get; private set; }

        private Levels.GameLevel _gameLevel;
        private (bool player, bool hit, bool sink) _shipFireArgs;

        public AudioManager()
        {
            FMODInterop.Init();

            MusicManager = new();
            AmbienceManager = new();

            Program.LevelChanged += OnLevelChanged;
        }

        public void Update()
        {
            FMODInterop.Update();
        }

        public void Shutdown()
        {
            FMODInterop.Shutdown();
        }

        private void OnLevelChanged(object? sender, EventArgs e)
        {
            if (Program.CurrentLevel is Levels.GameLevel)
            {
                _gameLevel = Program.CurrentLevel as Levels.GameLevel;

                _gameLevel.ShipFired += OnShipFired;

                Game();
            } else if (Program.CurrentLevel is Levels.MainMenuLevel)
            {
                MainMenu();
            }
        }

        private void OnShipFired(object? sender, (bool player, bool hit, bool sink) args)
        {
            _shipFireArgs = args;
            Debug.Log("Firing", "Game");
            FMODInterop.PlayEventOneShot(
                "MissileLaunch", 
                new[] { ("Turn", _shipFireArgs.player ? 0f : 1f) }, 
                OnMissileLaunchDone);
        }

        private void OnMissileLaunchDone(object? sender, Studio.EVENT_CALLBACK_TYPE type)
        {
            if (type != Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
                return;
            
            string impact = "Missile";
            impact += _shipFireArgs.sink ? "Sink" : _shipFireArgs.hit ? "Hit" : "Miss";

            Debug.Log("Impact: " + impact, "Game");

            FMODInterop.PlayEventOneShot(
                impact,
                new[] { ("Turn", _shipFireArgs.player ? 0f : 1f) },
                OnMissileImpactDone);
        }

        private void OnMissileImpactDone(object? sender, Studio.EVENT_CALLBACK_TYPE type)
        {
            if (type != Studio.EVENT_CALLBACK_TYPE.TIMELINE_MARKER)
                return;
            
            Debug.Log("Continuing", "Game");
            Debug.Break();
            _gameLevel.BlockAction = false;
        }

        private void Game()
        {
            AmbienceManager.Game();
            MusicManager.Game();
        }

        private void MainMenu()
        {
            AmbienceManager.MainMenu();
            MusicManager.MainMenu();
        }
    }
}
