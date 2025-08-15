using System;

namespace DinaFramework.Services
{
    /// <summary>
    /// 
    /// </summary>
    public readonly struct ServiceKey : IEquatable<ServiceKey>
    {
        private readonly string _key;
        private ServiceKey(string key)
        {
            _key = key;
        }
        /// <summary>
        /// 
        /// </summary>
        public static readonly ServiceKey SceneManager = new("SceneManager");

        /// <summary>
        /// 
        /// </summary>
        public static readonly ServiceKey Texture1px = new("Texture1px");

        /// <summary>
        /// 
        /// </summary>
        public static readonly ServiceKey ScreenManager = new ServiceKey("ScreenManager");


        /// <summary>
        /// 
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static ServiceKey FromString(string key) => new ServiceKey(key);
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
        public bool Equals(ServiceKey other) => _key == other._key;
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj) => obj is ServiceKey other && Equals(other);
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
        public static bool operator ==(ServiceKey left, ServiceKey right) => left.Equals(right);
        /// <summary>
        /// 
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(ServiceKey left, ServiceKey right) => !left.Equals(right);
    }
}
