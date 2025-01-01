using DinaFramework.Enums;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

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

        private readonly Dictionary<string, ControllerKey> _keys;

        /// <summary>
        /// Initialise une nouvelle instance de la classe PlayerController.
        /// </summary>
        /// <param name="controllerType">Le type de contrôleur.</param>
        /// <param name="playerIndex">L'index du joueur.</param>
        /// <param name="controllerKeys">Les touches de contrôleur associées au joueur.</param>
        public PlayerController(ControllerType controllerType, PlayerIndex playerIndex, params ControllerKey[] controllerKeys)
        {
            ArgumentNullException.ThrowIfNull(controllerKeys);

            Controller = controllerType;
            Index = playerIndex;
            _keys = new Dictionary<string, ControllerKey>();
            foreach (ControllerKey key in controllerKeys)
                _keys.Add(key.ToString(), key);
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
                ArgumentNullException.ThrowIfNull(alias);

                alias = char.ToUpper(alias[0]) + alias[1..];
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
            }
            set
            {
                ArgumentNullException.ThrowIfNull(alias);

                alias = char.ToUpper(alias[0]) + alias[1..];

                if (!_keys.ContainsKey(alias))
                    _keys[alias] = value;
                else if (value == null)
                    _keys.Remove(alias);
            }
        }
        /// <summary>
        /// Récupère une touche de contrôleur par son nom.
        /// </summary>
        /// <param name="name">Le nom de la touche de contrôleur.</param>
        /// <returns>La touche de contrôleur correspondante, ou null si elle n'est pas trouvée.</returns>
        public ControllerKey GetKey(string name)
        {
            if (_keys.TryGetValue(name, out ControllerKey value))
                return value;
            return null;
        }
    }
}
