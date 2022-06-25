using SFML.System;
using SFML.Audio;
using System.Timers;
using FStudio = FMOD.Studio;

namespace ForTeacher.AudioSystem
{
    
    public class AudioManager
    {
        public MusicManager MusicManager { get; private set; }
        public AmbienceManager AmbienceManager { get; private set; }

        // FMOD
        private FStudio.System _fSystem;
        private FStudio.Bank _bMaster;

        private Levels.GameLevel _gameLevel;

        // Ship firing effect
        private System.Timers.Timer _missileTimer;
        private Sound _missileLaunch;
        private Sound _impactMiss;
        private Sound _impactHit;
        private Sound _impactSink;
        private int _missilePhase;
        private (bool, bool, bool) _fireInfo;

        public AudioManager()
        {
            // Initialize FMOD
            // The FMOD Core runtime needs to be loaded before the Studio
            // runtime, so we call a innocuous Core function before anything else
            FMOD.Memory.GetStats(out _, out _);
            FStudio.System.create(out _fSystem);
            _fSystem.initialize(64, FStudio.INITFLAGS.NORMAL, FMOD.INITFLAGS.NORMAL, IntPtr.Zero);

            _fSystem.loadBankFile("Resources/Audio/FMOD/Desktop/Master.bank", FStudio.LOAD_BANK_FLAGS.NORMAL, out _bMaster);

            _bMaster.getEventList(out var descriptions);
            descriptions[0].createInstance(out var instance);
            instance.start();
            instance.release();
            
            MusicManager = new();
            AmbienceManager = new();

            Program.LevelChanged += OnLevelChanged;

            _missileLaunch = new(ResourceLoader.Get<SoundBuffer>("MissileLaunch"));
            _impactMiss = new(ResourceLoader.Get<SoundBuffer>("ImpactMiss"));
            _impactHit = new(ResourceLoader.Get<SoundBuffer>("ImpactHit"));
            _impactSink = new(ResourceLoader.Get<SoundBuffer>("ImpactSink"));

            _missileTimer = new();
            _missileTimer.Elapsed += OnShipFiredTimerComplete;
        }

        public void Update()
        {
            _fSystem.update();
        }

        public void Shutdown()
        {
            _bMaster.unload();
            _fSystem.release();
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

        private void OnShipFired(object? sender, (bool, bool, bool) e)
        {
            _fireInfo = e;

            // If the player fired, play at high volume, for AI ships we play at
            // lower volume (to simulate attenuation)
            _missileLaunch.Volume = _fireInfo.Item1 ? 100 : 25;

            Console.WriteLine("Missile launched");
            _missileLaunch.Play();

            _missileTimer.Interval = 3000;
            _missileTimer.Enabled = true;
        }

        private void OnShipFiredTimerComplete(object? sender, ElapsedEventArgs e)
        {
            _missilePhase++;
            
            switch (_missilePhase)
            {
                case 1:
                    // Missile has launched, play impact (water or explosion sfx)
                    _missileTimer.Interval = 2000;
                    
                    if (_fireInfo.Item2 && _fireInfo.Item3 == false)
                    {
                        Console.WriteLine("Missile hit");
                        _impactHit.Volume = _fireInfo.Item1 ? 25 : 100;
                        _impactHit.Play();
                    } else if (_fireInfo.Item3 == false)
                    {
                        Console.WriteLine("Missile missed");
                        _impactMiss.Volume = _fireInfo.Item1 ? 25 : 100;
                        _impactMiss.Play();
                    } else
                    {
                        Console.WriteLine("Mssile sank a ship");
                        _impactSink.Volume = _fireInfo.Item1 ? 25 : 100;
                        _impactSink.Play();
                        _missileTimer.Interval = 4000;
                    }

                    _missileTimer.Enabled = true;
                    break;
                case 2:
                    // Missile impact is done, can continue game
                    Console.WriteLine("Continuing game");
                    _gameLevel.BlockAction = false;
                    _missilePhase = 0;
                    _missileTimer.Enabled = false;
                    break;
            }
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
