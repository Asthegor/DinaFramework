using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Resources;

namespace DinaFramework.Localization
{
    /// <summary>
    /// Gère la traduction des chaînes de texte dans l'application, permettant de récupérer des traductions pour différentes langues.
    /// </summary>
    public static class LocalizationManager
    {
        private static readonly Dictionary<string, string> _cache = new();
        private static readonly List<ResourceManager> _resourceManagers = new();
        private static readonly List<Assembly> _assemblies = [];
        private static string[] _availableLanguages;
        private static bool _loaded;
        private static bool _updated;

        //private static readonly List<Type> _listStrings = [];

        private static CultureInfo _currentCulture = CultureInfo.CurrentUICulture;

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
            get => _currentCulture.TwoLetterISOLanguageName;
            set
            {
                _currentCulture = new CultureInfo(value);
                _updated = true;
                _cache.Clear();
            }
        }

        /// <summary>
        /// Ajoute une classe contenant des traductions.
        /// </summary>
        /// <param name="resourceClass">La classe contenant les traductions à ajouter.</param>
        public static void Register(Type resourceClass)
        {
            ArgumentNullException.ThrowIfNull(resourceClass);

            var prop = resourceClass.GetProperty("ResourceManager", BindingFlags.Static | BindingFlags.NonPublic | BindingFlags.Public);
            if (prop == null)
                throw new InvalidOperationException($"La classe {resourceClass.Name} ne contient pas de ResourceManager.");
            var rm = prop.GetValue(null) as ResourceManager;
            _resourceManagers.Add(rm);

            _assemblies.Add(resourceClass.Assembly);
            _loaded = true;
            _availableLanguages = SearchAvailableLanguages();
        }

        /// <summary>
        /// Récupère la traduction pour une clé donnée dans la langue actuelle.
        /// </summary>
        /// <param name="key">La clé de la traduction.</param>
        /// <returns>La traduction correspondant à la clé, ou la clé elle-même si non trouvée.</returns>
        public static string GetTranslation(string key)
        {
            if (_cache.TryGetValue(key, out var cached))
                return cached;

            foreach (var rm in _resourceManagers)
            {
                try
                {
                    var translation = rm.GetString(key, _currentCulture);
                    if (!string.IsNullOrEmpty(translation))
                    {
                        _cache[key] = translation;
                        return translation;
                    }
                }
                catch (MissingManifestResourceException)
                {
                    // Ignore cette exception si la ressource n'est pas trouvée
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
            var previous = _currentCulture;
            _currentCulture = new CultureInfo(culture);
            var translation = GetTranslation(key);
            _currentCulture = previous;
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
        public static string[] GetAvailableLanguages() => _availableLanguages ?? Array.Empty<string>();

        private static string[] SearchAvailableLanguages()
        {
            var cultures = new HashSet<string>();
            foreach (var assembly in _assemblies)
            {
                var dir = Path.GetDirectoryName(assembly.Location);
                if (dir != null)
                    AddCulturesFromSubdirectories(dir, cultures, assembly);
            }
            return new List<string>(cultures).ToArray();

        }

        private static void AddCulturesFromSubdirectories(string directoryPath, HashSet<string> cultures, Assembly assembly)
        {
            foreach (var directory in Directory.GetDirectories(directoryPath))
            {
                var cultureName = Path.GetFileName(directory);
                try
                {
                    var culture = new CultureInfo(cultureName);
                    var satellitePath = Path.Combine(directory, $"{assembly.GetName().Name}.resources.dll");
                    if (File.Exists(satellitePath))
                        cultures.Add(cultureName);
                }
                catch (CultureNotFoundException)
                {
                    // Ignorer les dossiers non valides, mais ne pas bloquer la recherche des autres sous-répertoires
                }
                AddCulturesFromSubdirectories(directory, cultures, assembly);
            }
        }

    }
}
