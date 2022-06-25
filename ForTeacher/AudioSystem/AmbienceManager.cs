using SFML.System;
using SFML.Audio;

namespace ForTeacher.AudioSystem
{
    public class AmbienceManager
    {
        private Music _shipInside;

        public AmbienceManager()
        {
            _shipInside = ResourceLoader.Get<Music>("ShipInside");
        }

        public void Game()
        {
            _shipInside.Loop = true;
            _shipInside.Play();
        }

        public void MainMenu()
        {
            // Stop all game ambience
            _shipInside.Stop();
        }
    }
}
