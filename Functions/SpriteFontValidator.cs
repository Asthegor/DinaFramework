namespace DinaFramework.Functions
{
    using Microsoft.Xna.Framework.Graphics;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;
    /// <summary>
    /// Fournit des outils pour valider la compatibilité entre les chaînes d’une ressource localisée
    /// et les caractères pris en charge par une <see cref="SpriteFont"/>.
    /// </summary>
    public static class SpriteFontValidator
    {
        /// <summary>
        /// Vérifie toutes les chaînes d’une ressource localisée pour une culture donnée,
        /// et retourne la liste des entrées contenant du premier caractère non supporté par la <see cref="SpriteFont"/>.
        /// </summary>
        /// <param name="font">Police <see cref="SpriteFont"/> utilisée pour vérifier les caractères disponibles.</param>
        /// <param name="resourceType">Type de la classe ressource (.resx) contenant les traductions.</param>
        /// <param name="cultureCode">Code de culture (ex. "fr-FR", "en-US").</param>
        /// <returns>
        /// Une liste de tuples contenant la clé de ressource, sa valeur et le premier caractère invalide rencontré.
        /// </returns>
        /// <exception cref="ArgumentException">
        /// Lancée si le code culture est vide, ou si la classe ressource ne contient pas de <see cref="ResourceManager"/>.
        /// </exception>
        public static IReadOnlyList<(string Key, string Value, char InvalidChar)> CheckResourceValuesWithSpriteFont(SpriteFont font, Type resourceType, string cultureCode)
        {
            if (string.IsNullOrWhiteSpace(cultureCode))
                throw new ArgumentException("Code culture invalide", nameof(cultureCode));

            // Préparer les caractères supportés
            HashSet<char> supportedChars = [.. font?.Characters];

            // Obtenir le ResourceManager de la classe de ressources
            var property = resourceType?.GetProperty("ResourceManager",
                System.Reflection.BindingFlags.Static |
                System.Reflection.BindingFlags.NonPublic |
                System.Reflection.BindingFlags.Public);

            if (property == null)
                throw new ArgumentException("La classe fournie ne contient pas de ResourceManager.");

            var manager = property.GetValue(null) as ResourceManager;
            if (manager == null)
                throw new ArgumentException("Impossible d’obtenir le ResourceManager à partir de la classe fournie.");

            // Charger la culture (langue)
            CultureInfo culture = new CultureInfo(cultureCode);

            // Résultat des erreurs
            List<(string Key, string Value, char InvalidChar)> errors = [];

            // Obtenir toutes les clés de la ressource
            ResourceSet resourceSet = manager.GetResourceSet(culture, true, true);
            foreach (System.Collections.DictionaryEntry entry in resourceSet)
            {
                string key = entry.Key as string;
                string value = entry.Value as string;

                if (key == null || value == null)
                    continue;

                foreach (char c in value)
                {
                    if (!supportedChars.Contains(c))
                    {
                        errors.Add((key, value, c));
                        break; // On ne signale que le premier caractère invalide pour chaque clé
                    }
                }
            }

            return errors;
        }
    }

}
