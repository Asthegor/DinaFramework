#pragma warning disable CA1002 // Ne pas exposer de listes génériques
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
namespace DinaFramework.Functions
{
    using Microsoft.Xna.Framework.Graphics;

    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.Resources;

    public static class SpriteFontValidator
    {
        public static List<(string Key, string Value, char InvalidChar)> CheckResourceValuesWithSpriteFont(SpriteFont font, Type resourceType, string cultureCode)
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

#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
#pragma warning restore CA1002 // Ne pas exposer de listes génériques