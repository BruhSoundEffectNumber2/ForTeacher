using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SFML.System;
using SFML.Graphics;

namespace ForTeacher
{
    public enum ResourceType
    {
        Image,
        Font
    }

    public static class ResourceLoader
    {
        static readonly Dictionary<string, object> _resources = new();

        public static bool Load(ResourceType type, string path, string name)
        {
            path = "Resources/" + path;

            if (_resources.ContainsKey(name))
            {
                return false;
            }

            switch (type)
            {
                case ResourceType.Image:
                    try
                    {
                        _resources[name] = new Image(path);
                        break;
                    } catch { return false; }

                case ResourceType.Font:
                    try
                    {
                        _resources[name] = new Font(path);
                        break;
                    }
                    catch { return false; }
            }

            return true;
        }

        public static T Get<T>(string name)
        {
            if (_resources.ContainsKey(name) == false)
            {
                throw new Exception("Could not find a resource named: {name}");
            }

            return (T)_resources[name];
        }
    }
}
