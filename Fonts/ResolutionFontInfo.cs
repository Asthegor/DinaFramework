using DinaFramework.Services;

using Microsoft.Xna.Framework;

using System;

namespace DinaFramework.Fonts
{
    /// <summary>
    /// Associe une résolution à un chemin de contenu et une clé.
    /// </summary>
    /// <param name="key">Clé associé à cette résolution.</param>
    /// <param name="resolution">Résolution d'écran associée.</param>
    /// <param name="contentPath">Chemin du contenu associé.</param>
    /// <exception cref="ArgumentNullException"></exception>
    public sealed class ResolutionFontInfo(Key<ResolutionTag> key, Point resolution, string contentPath)
    {
        /// <summary>
        /// ClÉ associée à cette résolution.
        /// </summary>
        public Key<ResolutionTag> Key { get; } = key;
        /// <summary>
        /// Résolution d'écran associée.
        /// </summary>
        public Point Resolution { get; } = resolution;
        /// <summary>
        /// Chemin du contenu associé.
        /// </summary>
        public string ContentPath { get; } = contentPath ?? throw new ArgumentNullException(nameof(contentPath));
    }
}
