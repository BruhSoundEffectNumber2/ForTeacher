using SFML.Graphics;
using SFML.Audio;

namespace ForTeacher
{
    public static class ResourceLoader
    {
        static readonly Dictionary<string, object> _resources = new();

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

        public static T Get<T>(string name)
        {
            if (_resources.ContainsKey(name) == false)
            {
                throw new Exception("Could not find a resource named: " + name);
            }

            return (T)_resources[name];
        }
    }
}
