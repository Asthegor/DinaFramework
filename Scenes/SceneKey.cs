using System;

namespace DinaFramework.Scenes
{
    /// <summary>
    /// Représente une clé unique et immuable pour identifier une scène dans le framework.
    /// Fournit des méthodes utilitaires pour la création, la comparaison et la conversion de clés de scène.
    /// </summary>
    public readonly struct SceneKey : IEquatable<SceneKey>
    {
        private readonly string _key;
        private SceneKey(string key)
        {
            _key = key;
        }

        /// <summary>
        /// Crée une nouvelle instance de <see cref="SceneKey"/> à partir d'une chaîne de caractères.
        /// </summary>
        /// <param name="key">La chaîne à utiliser comme clé de scène.</param>
        /// <returns>Une nouvelle clé de scène.</returns>
        public static SceneKey FromString(string key) => new SceneKey(key);
        /// <summary>
        /// Retourne la représentation sous forme de chaîne de la clé de scène.
        /// </summary>
        /// <returns>La chaîne représentant la clé de scène.</returns>
        public override string ToString() => _key;
        /// <summary>
        /// Détermine si la clé de scène actuelle est égale à une autre clé de scène.
        /// </summary>
        /// <param name="other">La clé de scène à comparer.</param>
        /// <returns>True si les clés sont identiques, sinon false.</returns>
        public bool Equals(SceneKey other) => _key == other._key;
        /// <summary>
        /// Détermine si la clé de scène actuelle est égale à un objet donné.
        /// </summary>
        /// <param name="obj">L'objet à comparer.</param>
        /// <returns>True si l'objet est une <see cref="SceneKey"/> et que les clés sont identiques, sinon false.</returns>
        public override bool Equals(object obj) => obj is SceneKey other && Equals(other);
        /// <summary>
        /// Retourne le code de hachage de la clé de scène.
        /// </summary>
        /// <returns>Un entier représentant le code de hachage.</returns>
        public override int GetHashCode() => _key.GetHashCode(StringComparison.CurrentCulture);
        /// <summary>
        /// Vérifie si deux clés de scène sont identiques.
        /// </summary>
        /// <param name="left">La première clé.</param>
        /// <param name="right">La seconde clé.</param>
        /// <returns>True si les clés sont égales, sinon false.</returns>
        public static bool operator ==(SceneKey left, SceneKey right) => left.Equals(right);
        /// <summary>
        /// Vérifie si deux clés de scène sont différentes.
        /// </summary>
        /// <param name="left">La première clé.</param>
        /// <param name="right">La seconde clé.</param>
        /// <returns>True si les clés sont différentes, sinon false.</returns>
        public static bool operator !=(SceneKey left, SceneKey right) => !left.Equals(right);
    }

}
