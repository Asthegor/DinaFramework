using DinaFramework.Enums;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DinaFramework.Controls
{
    /// <summary>
    /// Classe représentant un contrôleur de joueur.
    /// </summary>
    public class PlayerController
    {
        /// <summary>
        /// Type de contrôleur utilisé par le joueur.
        /// </summary>
        public ControllerType Controller { get; private set; }
        /// <summary>
        /// Index du joueur.
        /// </summary>
        public PlayerIndex Index { get; private set; }

        private readonly Dictionary<string, ControllerKey> _keys = [];

        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerController.
        /// </summary>
        /// <param name="controllerType">Le type de contrôleur.</param>
        /// <param name="playerIndex">L'index du joueur.</param>
        /// <param name="controllerKeys">Les touches de contrôleur associées au joueur.</param>
        public PlayerController(ControllerType controllerType, PlayerIndex playerIndex, params ControllerKey[] controllerKeys)
        {
            ArgumentNullException.ThrowIfNull(controllerKeys);

            if (controllerKeys.Length == 0)
                throw new ArgumentException("At least one controller key must be provided.", nameof(controllerKeys));

            if (controllerKeys.Any(k => k == null || string.IsNullOrWhiteSpace(k.ToString())))
                throw new ArgumentException("Controller keys cannot be null or have empty names.", nameof(controllerKeys));

            Controller = controllerType;
            Index = playerIndex;

            foreach (ControllerKey key in controllerKeys)
            {
                string alias = NormalizeAlias(key.Alias);

                
                if (_keys.ContainsKey(alias))
                    throw new ArgumentException($"Duplicate controller key name detected: '{alias}'", nameof(controllerKeys));

                _keys.Add(alias, key);
            }
        }

        /// <summary>
        /// Accesseur pour obtenir ou définir une touche de contrôleur par son alias.
        /// </summary>
        /// <param name="alias">L'alias de la touche de contrôleur.</param>
        /// <returns>La touche de contrôleur.</returns>
        public ControllerKey this[string alias]
        {
            get
            {
                ArgumentException.ThrowIfNullOrWhiteSpace(alias);
                //if (alias.Trim().Length < 2)
                //    throw new ArgumentException("Alias must be longer than 2 characters.", nameof(alias));

                alias = NormalizeAlias(alias);

                _keys.TryGetValue(alias, out ControllerKey value);
                return value;

                /*
                foreach (var kvp in _keys)
                {
                    if (_keys.TryGetValue(alias, out ControllerKey value))
                        return value;

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
                */
            }
            set
            {
                ArgumentNullException.ThrowIfNull(alias);

                alias = NormalizeAlias(alias);

                if (value == null)
                    _keys.Remove(alias);
                else 
                    _keys[alias] = value;
            }
        }
        /// <summary>
        /// Récupère une touche de contrôleur par son nom.
        /// </summary>
        /// <param name="alias">Alias de la touche de contrôleur.</param>
        /// <returns>La touche de contrôleur correspondante, ou null si elle n'est pas trouvée.</returns>
        public ControllerKey GetKey(string alias) => this[alias];

        private static string NormalizeAlias(string alias)
        {
            if (string.IsNullOrEmpty(alias))
                return alias;
            return char.ToUpper(alias[0]) + alias[1..];
        }
    }
}
