using DinaFramework.Services;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DinaFramework.Inputs
{
    /// <summary>
    /// Représente le contrôleur d'un joueur, permettant de gérer les liaisons 
    /// entre des actions logiques (<see cref="IKey"/>) et des touches ou boutons physiques (<see cref="ControllerKey"/>).
    /// </summary>
    public sealed class PlayerController
    {
        private readonly Dictionary<IKey, List<ControllerKey>> _bindings = [];

        /// <summary>
        /// Initialise un nouveau contrôleur joueur avec un ensemble de liaisons action → touches/boutons.
        /// </summary>
        /// <param name="bindings">Tableau de tuples associant une action à un ou plusieurs <see cref="ControllerKey"/>.</param>
        /// <exception cref="ArgumentNullException">Levée si <paramref name="bindings"/> est null.</exception>
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
        /// <summary>
        /// Ajoute une nouvelle liaison entre une action et une touche/bouton.
        /// </summary>
        /// <param name="action">L'action logique à associer.</param>
        /// <param name="key">La touche ou le bouton physique.</param>
        public void Bind(IKey action, ControllerKey key)
        {
            if (!_bindings.ContainsKey(action))
                _bindings[action] = [];
            _bindings[action].Add(key);
        }

        /// <summary>
        /// Met à jour l'état de toutes les touches/boutons liés à ce joueur.
        /// </summary>
        public void Update()
        {
            foreach (var keys in _bindings.Values)
            {
                foreach (var key in keys)
                    key.Update();
            }
        }

        /// <summary>
        /// Indique si une action est actuellement maintenue.
        /// </summary>
        /// <param name="action">L'action à tester.</param>
        /// <returns><c>true</c> si au moins une touche liée est enfoncée, sinon <c>false</c>.</returns>
        public bool IsDown(IKey action) => _bindings.TryGetValue(action, out var keys) && keys.Any(k => k.IsDown);
        /// <summary>
        /// Indique si une action vient d’être pressée pendant cette frame.
        /// </summary>
        /// <param name="action">L'action à tester.</param>
        /// <returns><c>true</c> si au moins une touche liée a été pressée, sinon <c>false</c>.</returns>
        public bool IsPressed(IKey action) => _bindings.TryGetValue(action, out var keys) && keys.Any(k => k.IsPressed);
        /// <summary>
        /// Indique si une action vient d’être relâchée pendant cette frame.
        /// </summary>
        /// <param name="action">L'action à tester.</param>
        /// <returns><c>true</c> si au moins une touche liée a été relâchée, sinon <c>false</c>.</returns>
        public bool IsReleased(IKey action) => _bindings.TryGetValue(action, out var keys) && keys.Any(k => k.IsReleased);
    }
}
