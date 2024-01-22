#pragma warning disable CA1854 // Préférer la méthode 'IDictionary.TryGetValue(TKey, out TValue)'

using DinaFramework.Enums;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace DinaFramework.Controls
{
    public class PlayerController
    {
        public ControllerType Controller { get; private set; }
        public PlayerIndex Index { get; private set; }
        private readonly Dictionary<string, ControllerKey> _keys;
        public PlayerController(ControllerType controllerType, PlayerIndex playerIndex, params ControllerKey[] controllerKeys)
        {
            ArgumentNullException.ThrowIfNull(controllerKeys);

            Controller = controllerType;
            Index = playerIndex;
            _keys = new Dictionary<string, ControllerKey>();
            foreach (ControllerKey key in controllerKeys)
                _keys.Add(key.ToString(), key);
        }
        public ControllerKey this[string alias]
        {
            get
            {
                ArgumentNullException.ThrowIfNull(alias);

                alias = char.ToUpper(alias[0]) + alias[1..];
                foreach (var kvp in _keys)
                {
                    if (_keys.ContainsKey(alias))
                        return _keys[alias];

                    switch (kvp.Value)
                    {
                        case KeyboardKey:
                            if (_keys.ContainsKey("Key_" + alias))
                                return _keys["Key_" + alias];
                            break;
                        case GamepadButton:
                            if (_keys.ContainsKey("Button_" + alias))
                                return _keys["Button_" + alias];
                            break;
                        default:
                            break;
                    }
                }
                return null;
            }
        }
        public ControllerKey GetKey(string name)
        {
            if (_keys.ContainsKey(name))
                return _keys[name];
            return null;
        }
    }
}
#pragma warning restore CA1854 // Préférer la méthode 'IDictionary.TryGetValue(TKey, out TValue)'
