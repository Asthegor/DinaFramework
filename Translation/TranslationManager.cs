using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;

namespace DinaFramework.Translation
{
    /// <summary>
    /// Gère la traduction des chaînes de texte dans l'application, permettant de récupérer des traductions pour différentes langues.
    /// </summary>
    public static class TranslationManager
    {
        private static readonly List<Type> _listStrings = [];
        private static readonly List<Assembly> _assemblies = [];
        private static string[] _availableLanguages;
        private static bool _loaded;
        private static bool _updated;

        /// <summary>
        /// Vérifie si les valeurs de traduction ont été chargées.
        /// </summary>
        public static bool IsLoaded => _loaded;

        /// <summary>
        /// Vérifie si les traductions ont été mises à jour.
        /// </summary>
        public static bool IsUpdated => _updated;

        /// <summary>
        /// Obtient ou définit la langue actuelle.
        /// </summary>
        public static string CurrentLanguage
        {
            get => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            set
            {
                var culture = new CultureInfo(value);
                CultureInfo.CurrentCulture = culture;
                foreach (var strings in _listStrings)
                {
                    foreach (var method in strings.GetRuntimeMethods())
                    {
                        if (method.Name == "set_Culture")
                        {
                            method.Invoke(null, new[] { culture });
                            break;
                        }
                    }
                }
                _updated = true;
            }
        }

        /// <summary>
        /// Ajoute une classe contenant des traductions.
        /// </summary>
        /// <param name="valueClass">La classe contenant les traductions à ajouter.</param>
        public static void SetValues(Type valueClass)
        {
            ArgumentNullException.ThrowIfNull(valueClass);
            _listStrings.Add(valueClass);
            _assemblies.Add(valueClass.Assembly);
            _loaded = true;
            SearchAvailableLanguages();
        }

        /// <summary>
        /// Récupère la traduction pour une clé donnée dans la langue actuelle.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <returns>La traduction correspondant à la clé, ou la clé elle-même si non trouvée.</returns>
        public static string GetTranslation(string key)
        {
            if (_listStrings.Count == 0)
                return key;

            foreach (var strings in _listStrings)
            {
                foreach (var method in strings.GetRuntimeMethods())
                {
                    if (method.Name == $"get_{key}")
                        return (string)method.Invoke(null, null);
                }
            }
            return key;
        }

        /// <summary>
        /// Récupère la traduction pour une clé donnée dans une culture spécifique.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <param name="culture">Le code de la culture.</param>
        /// <returns>La traduction pour la culture spécifiée.</returns>
        public static string GetTranslation(string key, string culture)
        {
            var previousLanguage = CurrentLanguage;
            CurrentLanguage = culture;

            var translation = GetTranslation(key);
            CurrentLanguage = previousLanguage;

            return translation;
        }

        /// <summary>
        /// Récupère la longueur maximale d'une traduction pour une clé donnée en utilisant une police.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <param name="font">La police utilisée pour mesurer la longueur.</param>
        /// <returns>La longueur maximale sous forme de Vector2.</returns>
        public static Vector2 GetTranslationMaxLength(string key, SpriteFont font)
        {
            ArgumentNullException.ThrowIfNull(font);

            var maxLength = Vector2.Zero;
            foreach (var language in GetAvailableLanguages())
            {
                var translation = GetTranslation(key, language);
                var size = font.MeasureString(translation);

                maxLength = new Vector2(
                    Math.Max(maxLength.X, size.X),
                    Math.Max(maxLength.Y, size.Y)
                );
            }

            return maxLength;
        }

        /// <summary>
        /// Récupère la liste des cultures disponibles via les assemblies satellites.
        /// </summary>
        /// <returns>Un tableau des langues disponibles.</returns>
        public static string[] GetAvailableLanguages() => _availableLanguages;

        private static string[] SearchAvailableLanguages()
        {
            var cultures = new List<string>();

            foreach (var assembly in _assemblies)
            {
                var assemblyPath = Path.GetDirectoryName(assembly.Location);
                AddCulturesFromSubdirectories(assemblyPath, cultures, assembly);
            }

            _availableLanguages = cultures.ToArray();
            return _availableLanguages;
        }

        private static void AddCulturesFromSubdirectories(string directoryPath, List<string> cultures, Assembly assembly)
        {
            foreach (var directory in Directory.GetDirectories(directoryPath))
            {
                var cultureName = Path.GetFileName(directory);

                try
                {
                    // Tenter de créer la culture sans provoquer d'erreur si le nom est invalide
                    var culture = new CultureInfo(cultureName);
                    var satellitePath = Path.Combine(directory, $"{assembly.GetName().Name}.resources.dll");
                    if (File.Exists(satellitePath))
                    {
                        cultures.Add(cultureName);
                    }
                }
                catch (CultureNotFoundException)
                {
                    // Ignorer les dossiers non valides, mais ne bloquer pas la recherche des autres sous-répertoires
                }

                // Appel récursif pour les sous-répertoires
                AddCulturesFromSubdirectories(directory, cultures, assembly);
            }
        }

    }
}
