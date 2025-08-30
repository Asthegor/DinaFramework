using DinaFramework.Services;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace DinaFramework.Inputs
{
    public static class InputManager
    {
        private static readonly Dictionary<PlayerIndex, PlayerController> _players = new();
        public static IEnumerable<PlayerController> GetPlayers()
        {
            return _players.Values;
        }
        public static PlayerController RegisterPlayer(PlayerIndex playerIndex, params (IKey action, ControllerKey[] key)[] bindings)
        {
            if (_players.ContainsKey(playerIndex))
                throw new InvalidOperationException($"Player {playerIndex} already registered.");

            var controller = new PlayerController(bindings);
            _players[playerIndex] = controller;
            return controller;
        }

        public static PlayerController GetPlayer(PlayerIndex playerId) => _players[playerId];

        public static void Update()
        {
            foreach (var controller in _players.Values)
                controller.Update();
        }
        public static bool IsPressedByAny(IKey key)
        {
            foreach (var player in GetPlayers())
                if (player.IsPressed(key))
                    return true;
            return false;
        }
        public static bool IsReleasedByAny(IKey key)
        {
            foreach (var player in GetPlayers())
                if (player.IsReleased(key))
                    return true;
            return false;
        }
    }
}
