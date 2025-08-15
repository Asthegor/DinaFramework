using System;
using System.Collections.Generic;

namespace DinaFramework.Services
{
    /// <summary>
    /// Fournit une implémentation simple pour la localisation de services, permettant d'enregistrer, de récupérer et de supprimer des services à l'aide de clés uniques.
    /// </summary>
    public static class ServiceLocator
    {
        // Stockage interne des services enregistrés, associant une clé unique à chaque service.
        private static readonly Dictionary<ServiceKey, object> _dictionary = [];
        /// <summary>
        /// Récupère un service enregistré à l'aide de sa clé.
        /// </summary>
        /// <typeparam name="T">Type attendu du service.</typeparam>
        /// <param name="key">Clé unique associée au service.</param>
        /// <returns>Le service correspondant à la clé spécifiée, casté en type T.</returns>
        /// <exception cref="KeyNotFoundException">Si la clé spécifiée n'existe pas dans le dictionnaire.</exception>
        public static T Get<T>(ServiceKey key)
        {
            if (_dictionary != null && _dictionary.TryGetValue(key, out object result))
                return (T)result;
            return default;
        }
        /// <summary>
        /// Enregistre ou met à jour un service associé à une clé spécifique.
        /// </summary>
        /// <param name="key">Clé unique pour identifier le service.</param>
        /// <param name="value">Instance du service à enregistrer.</param>
        public static void Register(ServiceKey key, object value)
        {
            _dictionary[key] = value;
        }
        /// <summary>
        /// Supprime un service enregistré à l'aide de sa clé.
        /// </summary>
        /// <param name="key">Clé unique associée au service à supprimer.</param>
        /// <returns>Retourne <c>true</c> si le service a été supprimé avec succès, sinon <c>false</c>.</returns>
        public static void Unregister(ServiceKey key)
        {
            _dictionary.Remove(key);
        }

        internal static bool Exists(ServiceKey v)
        {
            return _dictionary.ContainsKey(v);
        }
    }
}
