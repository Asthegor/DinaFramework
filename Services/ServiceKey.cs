using System;

namespace DinaFramework.Services
{
    /// <summary>
    /// Représente une clé unique et immuable pour identifier un service dans le framework.
    /// Fournit des méthodes utilitaires pour la création, la comparaison et la conversion de clés de service.
    /// </summary>
    public readonly struct ServiceKey : IEquatable<ServiceKey>
    {
        private readonly string _key;
        private ServiceKey(string key)
        {
            _key = key;
        }
        /// <summary>
        /// Clé représentant le gestionnaire de scènes.
        /// </summary>
        public static readonly ServiceKey SceneManager = new("SceneManager");

        /// <summary>
        /// Clé représentant la texture 1 pixel utilisée par défaut.
        /// </summary>
        public static readonly ServiceKey Texture1px = new("Texture1px");

        /// <summary>
        /// Clé représentant le gestionnaire d'écran.
        /// </summary>
        public static readonly ServiceKey ScreenManager = new ServiceKey("ScreenManager");

        /// <summary>
        /// Clé représentant la texture de la flèche d'une liste déroulante.
        /// </summary>
        public static readonly ServiceKey DropDownArrow = new("DropDownArrow"); 


        /// <summary>
        /// Crée une nouvelle instance de <see cref="ServiceKey"/> à partir d'une chaîne de caractères.
        /// </summary>
        /// <param name="key">La chaîne à utiliser comme clé de service.</param>
        /// <returns>Une nouvelle clé de service.</returns>
        public static ServiceKey FromString(string key) => new ServiceKey(key);
        /// <summary>
        /// Retourne la représentation sous forme de chaîne de la clé de service.
        /// </summary>
        /// <returns>La chaîne représentant la clé de service.</returns>
        public override string ToString() => _key;
        /// <summary>
        /// Détermine si la clé de service actuelle est égale à une autre clé de service.
        /// </summary>
        /// <param name="other">La clé de service à comparer.</param>
        /// <returns>True si les clés sont identiques, sinon false.</returns>
        public bool Equals(ServiceKey other) => _key == other._key;
        /// <summary>
        /// Détermine si la clé de service actuelle est égale à un objet donné.
        /// </summary>
        /// <param name="obj">L'objet à comparer.</param>
        /// <returns>True si l'objet est une <see cref="ServiceKey"/> et que les clés sont identiques, sinon false.</returns>
        public override bool Equals(object obj) => obj is ServiceKey other && Equals(other);
        /// <summary>
        /// Retourne le code de hachage de la clé de service.
        /// </summary>
        /// <returns>Un entier représentant le code de hachage.</returns>
        public override int GetHashCode() => _key.GetHashCode(StringComparison.CurrentCulture);
        /// <summary>
        /// Vérifie si deux clés de service sont identiques.
        /// </summary>
        /// <param name="left">La première clé.</param>
        /// <param name="right">La seconde clé.</param>
        /// <returns>True si les clés sont égales, sinon false.</returns>
        public static bool operator ==(ServiceKey left, ServiceKey right) => left.Equals(right);
        /// <summary>
        /// Vérifie si deux clés de service sont différentes.
        /// </summary>
        /// <param name="left">La première clé.</param>
        /// <param name="right">La seconde clé.</param>
        /// <returns>True si les clés sont différentes, sinon false.</returns>
        public static bool operator !=(ServiceKey left, ServiceKey right) => !left.Equals(right);
    }
}
