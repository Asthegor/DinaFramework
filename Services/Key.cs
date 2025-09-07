#nullable enable
using System;

namespace DinaFramework.Services
{
    /// <summary>
    /// Représente une clé fortement typée pour identifier de manière unique des ressources,
    /// services ou zones dans le projet. Le type générique <typeparamref name="T"/> sert uniquement
    /// à distinguer les familles de clés et n'a pas d'autre rôle fonctionnel.
    /// </summary>
    /// <typeparam name="T">Un tag servant à différencier les types de clés.</typeparam>
    public readonly struct Key<T> : IKey, IEquatable<Key<T>>
    {
        private readonly string _value;
        private Key(string value) => _value = value;
        /// <summary>
        /// Retourne la valeur de la clé sous forme de chaîne.
        /// </summary>
        public string Value => _value;
        /// <summary>
        /// Crée une nouvelle clé à partir d'une chaîne.
        /// </summary>
        /// <param name="value">La valeur de la clé.</param>
        /// <returns>Une instance de <see cref="Key{T}"/> correspondant à la valeur donnée.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Design", "CA1000:Do not declare static members on generic types",
            Justification = "Méthode nécessaire pour créer une clé typée")]
        public static Key<T> FromString(string value) => new Key<T>(value);
        /// <summary>
        /// Compare cette clé à une autre clé du même type.
        /// </summary>
        /// <param name="other">La clé à comparer.</param>
        /// <returns>True si les valeurs sont identiques ; sinon false.</returns>
        public bool Equals(Key<T> other) => _value == other._value;
        /// <summary>
        /// Compare cette clé à un objet.
        /// </summary>
        /// <param name="obj">L'objet à comparer.</param>
        /// <returns>True si l'objet est une clé du même type et que les valeurs sont identiques ; sinon false.</returns>
        public override bool Equals(object? obj) => obj is Key<T> other && Equals(other);
        /// <summary>
        /// Compare cette clé à une autre clé.
        /// </summary>
        /// <param name="other">La IKey à comparer.</param>
        /// <returns>True si l'objet est une clé du même type et que les valeurs sont identiques ; sinon false.</returns>
        public bool Equals(IKey? other) => other != null && _value == other.Value;
        /// <summary>
        /// Retourne le code de hachage de la clé.
        /// </summary>
        /// <returns>Un entier représentant le code de hachage.</returns>
        public override int GetHashCode() => _value.GetHashCode(StringComparison.CurrentCulture);
        /// <summary>
        /// Compare deux clés du même type pour vérifier l'égalité.
        /// </summary>
        public static bool operator ==(Key<T> left, Key<T> right) => left.Equals(right);
        /// <summary>
        /// Compare deux clés du même type pour vérifier l'inégalité.
        /// </summary>
        public static bool operator !=(Key<T> left, Key<T> right) => !left.Equals(right);
    }
    /// <summary>
    /// Interface non générique pour représenter une clé.
    /// </summary>
    public interface IKey : IEquatable<IKey>
    {
        /// <summary>
        /// Retourne la valeur de la clé sous forme de chaîne.
        /// </summary>
        string Value { get; }
    }
}
