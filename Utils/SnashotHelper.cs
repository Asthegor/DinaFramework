using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace DinaFramework.Utils
{
    /// <summary>
    /// Fournit des méthodes utilitaires pour prendre un snapshot des propriétés publiques d'un objet
    /// et pour restaurer ces valeurs plus tard. Utile pour les éléments graphiques qui doivent
    /// sauvegarder/restaurer leur état.
    /// </summary>
    public static class SnapshotHelper
    {
        /// <summary>
        /// Crée un snapshot de toutes les propriétés publiques en lecture/écriture de l'objet fourni.
        /// </summary>
        /// <param name="obj">L'objet dont les propriétés doivent être sauvegardées.</param>
        /// <returns>
        /// Un dictionnaire dont les clés sont les noms des propriétés et les valeurs sont les valeurs actuelles.
        /// </returns>
        public static Dictionary<string, object?> Save(object obj)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));
            return obj.GetType()
                      .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                      .Where(p => p.CanRead && p.CanWrite)
                      .ToDictionary(p => p.Name, p => p.GetValue(obj));
        }

        /// <summary>
        /// Restaure les propriétés d'un objet à partir d'un snapshot précédemment pris.
        /// </summary>
        /// <param name="obj">L'objet dont les propriétés doivent être restaurées.</param>
        /// <param name="snapshot">Le snapshot contenant les valeurs à restaurer.</param>
        public static void Restore(object obj, Dictionary<string, object?> snapshot)
        {
            ArgumentNullException.ThrowIfNull(obj, nameof(obj));
            ArgumentNullException.ThrowIfNull(snapshot, nameof(snapshot));
            foreach (var pair in snapshot)
            {
                var prop = obj.GetType().GetProperty(pair.Key);
                if (prop != null && prop.CanWrite)
                    prop.SetValue(obj, pair.Value);
            }
        }
    }


}
