using System.Collections.Generic;

namespace DinaFramework.Core
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        public static T Retreive<T>(string key)
        {
            return (T)_dictionary[key];
        }
        public static void Register(string key, object value)
        {
            _dictionary[key] = value;
        }
        public static void Unregister(string key)
        {
            _dictionary.Remove(key);
        }
    }
}
