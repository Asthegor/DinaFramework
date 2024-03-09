using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinaFramework.Scenes
{
    sealed class ResourceManager
    {
        private readonly Dictionary<string, object> _resources = new Dictionary<string, object>();
        public void AddResource<T>(string resourceName, T resource)
        {
            _resources[resourceName] = resource;
        }

        public T GetResource<T>(string resourceName)
        {
            if (_resources.TryGetValue(resourceName, out object value))
            {
                return (T)value;
            }
            else
            {
                throw new KeyNotFoundException($"Resource '{resourceName}' not found in the resource manager.");
            }
        }
    }
}
