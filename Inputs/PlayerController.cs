using DinaFramework.Services;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DinaFramework.Inputs
{
    public class PlayerController
    {
        private readonly Dictionary<IKey, List<ControllerKey>> _bindings = [];

        public PlayerController(params (IKey action, ControllerKey[] keys)[] bindings)
        {
            ArgumentNullException.ThrowIfNull(bindings);
            foreach (var (action, list) in bindings)
            {
                if (!_bindings.ContainsKey(action))
                    _bindings[action] = [];
                _bindings[action].AddRange(list);
            }
        }
        public void Bind(IKey action, ControllerKey key)
        {
            if (!_bindings.ContainsKey(action))
                _bindings[action] = new List<ControllerKey>();
            _bindings[action].Add(key);
        }

        public void Update()
        {
            foreach (var keys in _bindings.Values)
            {
                foreach (var key in keys)
                    key.Update();
            }
        }

        public bool IsDown(IKey action) => _bindings.TryGetValue(action, out var keys) && keys.Any(k => k.IsDown);
        public bool IsPressed(IKey action) => _bindings.TryGetValue(action, out var keys) && keys.Any(k => k.IsPressed);
        public bool IsReleased(IKey action) => _bindings.TryGetValue(action, out var keys) && keys.Any(k => k.IsReleased);
    }
}
