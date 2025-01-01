using System;

namespace DinaFramework.Functions
{
    /// <summary>
    /// Fournit des fonctions utilitaires globales pour la manipulation des chaînes de caractères.
    /// </summary>
    public static class DinaFunctions
    {
        /// <summary>
        /// Extrait une valeur depuis une chaîne en fonction d'un séparateur, tout en modifiant la chaîne d'entrée pour supprimer la partie extraite.
        /// </summary>
        /// <param name="value">La chaîne de caractères à traiter. La chaîne sera modifiée pour exclure la partie extraite.</param>
        /// <param name="sep">Le séparateur utilisé pour identifier la partie à extraire.</param>
        /// <returns>La partie extraite de la chaîne. Si le séparateur n'est pas trouvé, retourne la chaîne complète.</returns>
        /// <exception cref="ArgumentNullException">Si le paramètre <paramref name="value"/> ou <paramref name="sep"/> est <c>null</c>.</exception>
        public static string ExtractValue(ref string value, string sep)
        {
            if (string.IsNullOrEmpty(value) || string.IsNullOrEmpty(sep))
                return value;
            string res;
            if (!value.Contains(sep, StringComparison.CurrentCulture))
            {
                res = value;
                value = "";
            }
            else
            {
                int posSep = value.IndexOf(sep, StringComparison.CurrentCulture);
                res = value[..posSep];
                value = value[(posSep + sep.Length)..];
            }
            return res;
        }
    }
}
