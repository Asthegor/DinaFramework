using DinaFramework.Services;

using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;

namespace DinaFramework.Fonts
{
    /// <summary>
    /// Contient toutes les résolutions disponibles et leurs chemins associés.
    /// Singleton.
    /// </summary>
    public sealed class FontProfile
    {
        private static FontProfile? _instance;
        /// <summary>
        /// Singleton de la classe <see cref="FontProfile"/>.
        /// </summary>
        public static FontProfile Instance => _instance ?? throw new InvalidOperationException("FontProfile n’a pas été initialisé.");
        /// <summary>
        /// Indique si le singleton a été initialisé.
        /// </summary>
        public static bool IsInitialized => _instance != null;


        private readonly Dictionary<Key<ResolutionTag>, ResolutionFontInfo> _byKey = [];
        private readonly Dictionary<Point, ResolutionFontInfo> _byResolution = [];
        private readonly Key<ResolutionTag> _defaultResolution;

        private FontProfile(IEnumerable<ResolutionFontInfo> infos, Key<ResolutionTag> defaultResolution)
        {
            foreach (var info in infos)
            {
                _byKey[info.Key] = info;
                _byResolution[info.Resolution] = info;
            }
            _defaultResolution = defaultResolution;
        }

        /// <summary>
        /// Initialise le singleton avec une liste de résolutions.
        /// </summary>
        public static void Initialize(IEnumerable<ResolutionFontInfo> infos, Key<ResolutionTag> defaultResolution)
        {
            ArgumentNullException.ThrowIfNull(infos);

            if (_instance != null)
                throw new InvalidOperationException("FontProfile est déjà initialisé.");
            _instance = new FontProfile(infos, defaultResolution);
            ServiceLocator.Register(ServiceKeys.FontProfile, _instance);
        }

        /// <summary>
        /// Permet de récupérer les informations associées à une clé de résolution.
        /// </summary>
        /// <param name="key">Clé de résolution.</param>
        /// <returns></returns>
        public ResolutionFontInfo GetInfo(Key<ResolutionTag> key) => _byKey.TryGetValue(key, out var info) ? info : _byKey[_defaultResolution];

        /// <summary>
        /// Permet de récupérer les informations associées à une résolution.
        /// </summary>
        /// <param name="resolution">Résolution d'écran.</param>
        /// <returns></returns>
        public ResolutionFontInfo GetInfo(Point resolution) => _byResolution.TryGetValue(resolution, out var info) ? info : _byKey[_defaultResolution];
    }
}
