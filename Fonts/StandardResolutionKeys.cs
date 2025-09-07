using DinaFramework.Services;

using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinaFramework.Fonts
{
    /// <summary>
    /// Tag utilisé pour les résolutions d'écran.
    /// </summary>
    public sealed class ResolutionTag { }
    /// <summary>
    /// Liste des clés standard pour les résolutions d'écran.
    /// </summary>
    public static class StandardResolutionKeys
    {
        /// <summary>
        /// Résolution 1280x720 (HD).
        /// </summary>
        public static readonly Key<ResolutionTag> R720p = Key<ResolutionTag>.FromString("R720p");
        /// <summary>
        /// Résolution 1600x900 (HD+).
        /// </summary>
        public static readonly Key<ResolutionTag> R900p = Key<ResolutionTag>.FromString("R900p");
        /// <summary>
        /// Résolution 1920x1080 (Full HD).
        /// </summary>
        public static readonly Key<ResolutionTag> R1080p = Key<ResolutionTag>.FromString("R1080p");
        /// <summary>
        /// Résolution 2560x1440 (2K).
        /// </summary>
        public static readonly Key<ResolutionTag> R1440p = Key<ResolutionTag>.FromString("R1440p");
        /// <summary>
        /// Résolution 3840x2160 (4K).
        /// </summary>
        public static readonly Key<ResolutionTag> R2160p = Key<ResolutionTag>.FromString("R2160p");
    }
}
