using FMOD;
using Studio = FMOD.Studio;

namespace ForTeacher.AudioSystem
{
    public struct FMODEventData
    {
        public string bank;
        public string name;
        public Studio.EventDescription description;

        public FMOD.RESULT CreateInstance(out FMODInstanceData instance)
        {
            var result = description.createInstance(out var tempEvent);

            if (result != FMOD.RESULT.OK)
            {
                instance = default;
                return result;
            }

            instance = new(tempEvent);

            return FMOD.RESULT.OK;
        }
    }

    public class FMODInstanceData
    {
        public Studio.EventInstance Instance { get; init; }
        public event EventHandler<Studio.EVENT_CALLBACK_TYPE> Callbacks
        {
            add
            {
                FMODInterop.AddCallback(this, value);
            }
            remove
            {
                FMODInterop.RemoveCallback(this, value);
            }
        }

        private bool _released;
        private Studio.EVENT_CALLBACK? _fCallback;

        public FMODInstanceData(Studio.EventInstance instance)
        {
            Instance = instance;
            _released = false;
            
            SetupCallback();
        }

        public bool Play()
        {
            if (_released)
                return false;

            Instance.start();

            return true;
        }

        public bool Release()
        {
            if (_released)
                return false;

            Instance.release();

            _released = true;

            return true;
        }

        private void SetupCallback()
        {
            _fCallback = new Studio.EVENT_CALLBACK(FMODCallback);
            Instance.setCallback(_fCallback);
        }

        private RESULT FMODCallback(Studio.EVENT_CALLBACK_TYPE t, IntPtr e, IntPtr p)
        {
            FMODInterop.AddCallbackEvent(this, t);

            return RESULT.OK;
        }
    }

    public static class FMODInterop
    {
        private static readonly string[] BANK_NAMES = { "Master", "Ambience", "Music", "Effects" };
        private static readonly Dictionary<string, Studio.Bank> s_banks = new();
        private static readonly List<FMODEventData> s_events = new();

        private static Studio.System s_system;
        private static List<(FMODInstanceData instance, Studio.EVENT_CALLBACK_TYPE type)> s_callbackEvents = new();
        private static Dictionary<FMODInstanceData, EventHandler<Studio.EVENT_CALLBACK_TYPE>> s_callbacks = new();

        public static void Init()
        {
            Debug.Log("Starting", "FMOD");
            
            /* 
             * The FMOD Core runtime needs to be loaded before the FMOD Studio
             * runtime, so we call a innocuous FMOD Core function before anything else.
             */
            Memory.GetStats(out _, out _);

            // Initialize FMOD
            Studio.System.create(out s_system);
            s_system.initialize(64, Studio.INITFLAGS.NORMAL, INITFLAGS.NORMAL, IntPtr.Zero);
            FMOD.Debug.Initialize(DEBUG_FLAGS.LOG);
            
            LoadBanks();

            foreach (string bank in BANK_NAMES)
            {
                LoadEvents(bank);
            }

            Debug.Log("Ready", "FMOD");
            Debug.Break();
        }

        public static void Update()
        {
            UpdateCallbacks();
            s_system.update();
        }

        public static void Shutdown()
        {
            Debug.Log("Shutting down", "FMOD");
            
            foreach (var bank in s_banks.Values)
            {
                bank.unload();
            }

            s_system.release();
        }

        public static void PlayEventOneShot(
            string name,
            (string name, float value)[]? parameters = default,
            EventHandler<Studio.EVENT_CALLBACK_TYPE>? callback = default)
        {
            if (GetEvent(name, out var data) == false)
                return;

            if (data.CreateInstance(out var instance) == RESULT.OK)
            {
                // Setup callback
                if (callback != null)
                {
                    AddCallback(instance, callback);
                }

                // Set parameters
                if (parameters != null)
                {
                    foreach (var param in parameters)
                    {
                        // Check to see if the parameter exists
                        if (instance.Instance.getParameterByName(param.name, out _) == RESULT.OK)
                        {
                            instance.Instance.setParameterByName(param.name, param.value);
                        } else
                        {
                            Debug.Warning("Parameter does not exist on instance", "FMOD");
                        }
                    }
                }
                
                instance.Play();
                instance.Release();
            }
        }

        public static bool GetEvent(string name, out FMODEventData data)
        {
            foreach (var eventData in s_events)
            {
                if (eventData.name == name)
                {
                    data = eventData;
                    return true;
                }
            }

            data = default;
            return false;
        }

        public static void AddCallbackEvent(FMODInstanceData instance, Studio.EVENT_CALLBACK_TYPE type)
        {
            s_callbackEvents.Add((instance, type));
        }

        public static void AddCallback(FMODInstanceData instance, EventHandler<Studio.EVENT_CALLBACK_TYPE> callback)
        {
            if (s_callbacks.ContainsKey(instance))
            {
                s_callbacks[instance] += callback;
            }
            else
            {
                s_callbacks[instance] = callback;
            }
        }

        public static void RemoveCallback(FMODInstanceData instance, EventHandler<Studio.EVENT_CALLBACK_TYPE> callback)
        {
            s_callbacks[instance] -= callback;
        }

        private static void UpdateCallbacks()
        {
            var copy = s_callbackEvents.ToList();
            List<(FMODInstanceData instance, Studio.EVENT_CALLBACK_TYPE type)> toRemove = new();

            foreach (var (instance, type) in copy)
            {
                s_callbacks[instance]?.Invoke(instance, type);

                toRemove.Add((instance, type));
            }

            foreach (var item in toRemove)
            {
                s_callbackEvents.RemoveAll(x => x.instance.Equals(item.instance) && x.type == item.type);
            }
        }

        private static void LoadBanks()
        {
            string stringsPath = "Resources/Audio/FMOD/Desktop/Master.strings.bank";
            var stringResult = s_system.loadBankFile(stringsPath, Studio.LOAD_BANK_FLAGS.NORMAL, out _);

            if (stringResult != FMOD.RESULT.OK)
            {
                Debug.Error("Fatal Error while loading the strings bank. " + FMOD.Error.String(stringResult), "FMOD");
                return;
            }

            foreach (string name in BANK_NAMES)
            {
                string path = "Resources/Audio/FMOD/Desktop/" + name + ".bank";

                var result = s_system.loadBankFile(path, Studio.LOAD_BANK_FLAGS.NORMAL, out var bank);

                if (result != FMOD.RESULT.OK)
                {
                    Debug.Error("Failed to load bank: " + name + ". " + FMOD.Error.String(result), "FMOD");
                    continue;
                }

                if (s_banks.ContainsKey(name))
                {
                    Debug.Error("Bank with the same name already loaded. " + name, "FMOD");
                    continue;
                }

                s_banks.Add(name, bank);
                Debug.Log("Loaded bank: " + name, "FMOD");
            }

            Debug.Log("Finished loading " + s_banks.Count + " banks.", "FMOD");
        }

        private static void LoadEvents(string bank)
        {
            if (s_banks.ContainsKey(bank) == false)
            {
                Debug.Error("Bank not loaded: " + bank, "FMOD");
                return;
            }

            s_banks[bank].getEventList(out var eventList);

            Debug.Log("Loading " + eventList.Length + " events from " + bank, "FMOD");

            foreach (var description in eventList)
            {
                var a = description.getPath(out string path);

                // TODO: allow multiple events of the same name
                string name = path.Split("/").Last();

                if (s_events.Any(x => x.name == name))
                {
                    Debug.Error("Event with the same name already loaded. " + name, "FMOD");
                    continue;
                }

                s_events.Add(new FMODEventData
                {
                    bank = bank,
                    name = name,
                    description = description,
                });
            }
        }
    }
}
