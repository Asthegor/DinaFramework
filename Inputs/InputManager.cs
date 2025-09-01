using DinaFramework.Services;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace DinaFramework.Inputs
{
    /// <summary>
    /// Fournit un gestionnaire global pour l'enregistrement et la mise à jour des contrôleurs joueurs.
    /// Permet de vérifier l'état des touches pour un joueur spécifique ou pour tous les joueurs.
    /// </summary>
    public static class InputManager
    {
        private static readonly Dictionary<PlayerIndex, PlayerController> _players = new();
        /// <summary>
        /// Retourne l'ensemble des contrôleurs enregistrés.
        /// </summary>
        public static IEnumerable<PlayerController> Players => _players.Values;
        /// <summary>
        /// Enregistre un nouveau joueur avec ses touches associées.
        /// </summary>
        /// <param name="playerIndex">Index du joueur à enregistrer.</param>
        /// <param name="bindings">Actions et touches à associer au joueur.</param>
        /// <returns>Le contrôleur du joueur nouvellement enregistré.</returns>
        /// <exception cref="InvalidOperationException">Si le joueur est déjà enregistré.</exception>
        public static PlayerController RegisterPlayer(PlayerIndex playerIndex, params (IKey action, ControllerKey[] key)[] bindings)
        {
            if (_players.ContainsKey(playerIndex))
                throw new InvalidOperationException($"Player {playerIndex} already registered.");

            var controller = new PlayerController(bindings);
            _players[playerIndex] = controller;
            return controller;
        }

        /// <summary>
        /// Récupère le contrôleur associé à un joueur.
        /// </summary>
        /// <param name="playerId">Index du joueur.</param>
        /// <returns>Le contrôleur du joueur demandé.</returns>
        public static PlayerController GetPlayer(PlayerIndex playerId) => _players[playerId];

        /// <summary>
        /// Met à jour l'état de tous les contrôleurs enregistrés.
        /// </summary>
        public static void Update()
        {
            foreach (var controller in _players.Values)
                controller.Update();
        }
        /// <summary>
        /// Vérifie si une action est actuellement pressée par au moins un joueur.
        /// </summary>
        /// <param name="key">L'action à vérifier.</param>
        /// <returns><c>true</c> si l'action est pressée, sinon <c>false</c>.</returns>
        public static bool IsPressedByAny(IKey key)
        {
            foreach (var player in Players)
                if (player.IsPressed(key))
                    return true;
            return false;
        }
        /// <summary>
        /// Vérifie si une action a été relâchée par au moins un joueur.
        /// </summary>
        /// <param name="key">L'action à vérifier.</param>
        /// <returns><c>true</c> si l'action a été relâchée, sinon <c>false</c>.</returns>
        public static bool IsReleasedByAny(IKey key)
        {
            foreach (var player in Players)
                if (player.IsReleased(key))
                    return true;
            return false;
        }
    }
}
