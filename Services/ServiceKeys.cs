using System;

namespace DinaFramework.Services
{
    /// <summary>
    /// Représente un tag vide utilisé pour typer les clés de service.
    /// </summary>
    public sealed class ServiceTag { }
    /// <summary>
    /// Représente une clé unique et immuable pour identifier un service dans le framework.
    /// Fournit des méthodes utilitaires pour la création, la comparaison et la conversion de clés de service.
    /// </summary>
    public static class ServiceKeys
    {
        /// <summary>
        /// Clé représentant le gestionnaire de scènes.
        /// </summary>
        public static readonly Key<ServiceTag> SceneManager = Key<ServiceTag>.FromString("SceneManager");

        /// <summary>
        /// Clé représentant la texture 1 pixel utilisée par défaut.
        /// </summary>
        public static readonly Key<ServiceTag> Texture1px = Key<ServiceTag>.FromString("Texture1px");

        /// <summary>
        /// Clé représentant le gestionnaire d'écran.
        /// </summary>
        public static readonly Key<ServiceTag> ScreenManager = Key<ServiceTag>.FromString("ScreenManager");

        /// <summary>
        /// Clé représentant la texture de la flèche d'une liste déroulante.
        /// </summary>
        public static readonly Key<ServiceTag> DropDownArrow = Key<ServiceTag>.FromString("DropDownArrow");

        /// <summary>
        /// Clé représentant le profil de polices.
        /// </summary>
        public static readonly Key<ServiceTag> FontProfile = Key<ServiceTag>.FromString("FontProfile");
        /// <summary>
        /// Clé représentant le gestionnaire de polices.
        /// </summary>
        public static readonly Key<ServiceTag> FontManager = Key<ServiceTag>.FromString("FontManager");
    }
}
