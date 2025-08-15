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
        /// 
        /// </summary>
        public static readonly SceneKey FrameworkLogo = new("__FrameworkLogo__");

        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static SceneKey FromString(string key) => new SceneKey(key);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() => _key;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="other"></param>
        /// <returns></returns>
        public bool Equals(SceneKey other) => _key == other._key;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is SceneKey other && Equals(other);
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode() => _key.GetHashCode(StringComparison.CurrentCulture);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(SceneKey left, SceneKey right) => left.Equals(right);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(SceneKey left, SceneKey right) => !left.Equals(right);
    }

}
