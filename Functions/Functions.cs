using System;
using System.Collections.Generic;

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
        /// <summary>
        /// Vérifie si une chaîne contient au moins un caractère dont le code ASCII ne correspond à aucune des plages spécifiées.
        /// </summary>
        /// <param name="str">La chaîne à analyser.</param>
        /// <param name="ranges">
        /// Tableau de plages de valeurs ASCII autorisées. Chaque tuple représente une plage inclusive (min, max).
        /// </param>
        /// <returns>
        /// true si un caractère ne correspond à aucune des plages fournies ; false si tous les caractères respectent au moins une plage.
        /// </returns>
        public static bool AsciiCharOutOfrange(string str, params (int min, int max)[] ranges)
        {
            if (string.IsNullOrEmpty(str) || ranges == null || ranges.Length == 0)
                return false;

            foreach (char c in str)
            {
                bool inRange = false;
                foreach (var (min, max) in ranges)
                {
                    if (c >= min && c <= max)
                    {
                        inRange = true;
                        break;
                    }
                }

                if (!inRange)
                    return true;
            }

            return false;
        }
    }
}
