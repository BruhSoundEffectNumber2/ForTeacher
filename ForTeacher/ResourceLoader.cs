using FStudio = FMOD.Studio;

namespace ForTeacher
{
    public static class ResourceLoader
    {
        private static readonly Dictionary<string, object> _resources = new();

        private static readonly Dictionary<string, 
            (FStudio.Bank, Dictionary<string, FStudio.EventDescription>)> _banks = new();

        private static FStudio.Bank _stringsBank;

        public static bool Load<T>(string path, string name)
        {
            path = "Resources/" + path;

            if (_resources.ContainsKey(name))
            {
                return false;
            }

            try
            {
                var res = Activator.CreateInstance(typeof(T), new object[] { path });

                if (res == null)
                    return false;

                _resources.Add(name, res);
            }
            catch (Exception e) { Console.WriteLine(e); return false; }

            return true;
        }

        public static bool LoadFMODBank(FStudio.System system, string fileName, string name)
        {
            if (LoadStringsBank(system) == false)
                return false;

            // Load bank
            if (_banks.ContainsKey(name))
                return false;

            string path = "Resources/Audio/FMOD/Desktop/" + fileName;

            var result = system.loadBankFile(path + ".bank", FStudio.LOAD_BANK_FLAGS.NORMAL, out var bank);

            if (result != FMOD.RESULT.OK)
            {
                Console.WriteLine("FMOD - Cannot load bank: {0}. Error: {1}", fileName, result);
                return false;
            }

            _banks.Add(name, (bank, new()));

            // Load events from bank
            bank.getEventList(out var bankEvents);

            foreach (var bankEvent in bankEvents)
            {
                bankEvent.getPath(out string p);

                // For now, we only consider the actual name of the event, that being the last portion
                // of the path. This is not a good solution, but it works for now.

                string eventName = p.Split('/').Last();

                // For now, we will require event names to be unique, even if they are in different directories

                if (_banks[name].Item2.ContainsKey(eventName))
                {
                    Console.WriteLine("FMOD - Cannot load event: {0}. Event of the same name already exists.", p);
                    continue;
                }

                _banks[name].Item2.Add(eventName, bankEvent);
            }

            Console.WriteLine($"FMOD - Loaded bank: {name}. {_banks[name].Item2.Count} events loaded.");

            return true;
        }

        public static T Get<T>(string name)
        {
            if (_resources.ContainsKey(name) == false)
            {
                throw new Exception("Could not find a resource named: " + name);
            }

            return (T)_resources[name];
        }

        public static FStudio.Bank GetFMODBank(string name)
        {
            if (_banks.ContainsKey(name) == false)
            {
                throw new Exception("Could not find a FMOD bank named: " + name);
            }

            return _banks[name].Item1;
        }

        public static FStudio.Bank[] GetAllFMODBanks()
        {
            return _banks.Values.Select(x => x.Item1).ToArray();
        }

        public static FStudio.EventDescription GetFMODEvent(string bank, string name)
        {
            if (_banks.ContainsKey(bank) == false)
            {
                throw new Exception("Could not find a FMOD bank named: " + bank);
            }

            if (_banks[bank].Item2.ContainsKey(name) == false)
            {
                throw new Exception("Could not find a FMOD event named: " + name);
            }

            return _banks[bank].Item2[name];
        }

        private static bool LoadStringsBank(FStudio.System system)
        {
            if (_stringsBank.isValid())
                return true;

            var result = system.loadBankFile(
                "Resources/Audio/FMOD/Desktop/Master.strings.bank", 
                FStudio.LOAD_BANK_FLAGS.NORMAL, 
                out var _strings);
            
            if (result != FMOD.RESULT.OK)
            {
                Console.WriteLine("FMOD - Cannot Master.strings.bank. Error: {0}", result);
                return false;
            }

            _stringsBank = _strings;
            return true;
        }
    }
}
