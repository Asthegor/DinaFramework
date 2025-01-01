using System.Collections.Generic;

namespace DinaFramework.Core
{
    /// <summary>
    /// Fournit une implémentation simple pour la localisation de services, permettant d'enregistrer, de récupérer et de supprimer des services à l'aide de clés uniques.
    /// </summary>
    public static class ServiceLocator
    {
        // Stockage interne des services enregistrés, associant une clé unique à chaque service.
        private static readonly Dictionary<string, object> _dictionary = new Dictionary<string, object>();
        /// <summary>
        /// Récupère un service enregistré à l'aide de sa clé.
        /// </summary>
        /// <typeparam name="T">Type attendu du service.</typeparam>
        /// <param name="key">Clé unique associée au service.</param>
        /// <returns>Le service correspondant à la clé spécifiée, casté en type T.</returns>
        /// <exception cref="KeyNotFoundException">Si la clé spécifiée n'existe pas dans le dictionnaire.</exception>
        /// <exception cref="InvalidCastException">Si le service enregistré ne correspond pas au type T attendu.</exception>
        public static T Retreive<T>(string key)
        {
            return (T)_dictionary[key];
        }
        /// <summary>
        /// Enregistre ou met à jour un service associé à une clé spécifique.
        /// </summary>
        /// <param name="key">Clé unique pour identifier le service.</param>
        /// <param name="value">Instance du service à enregistrer.</param>
        public static void Register(string key, object value)
        {
            _dictionary[key] = value;
        }
        /// <summary>
        /// Supprime un service enregistré à l'aide de sa clé.
        /// </summary>
        /// <param name="key">Clé unique associée au service à supprimer.</param>
        /// <returns>Retourne <c>true</c> si le service a été supprimé avec succès, sinon <c>false</c>.</returns>
        public static void Unregister(string key)
        {
            _dictionary.Remove(key);
        }
    }
}
