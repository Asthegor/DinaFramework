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
    /// <remarks>
    /// La classe TranslationManager permet de gérer les traductions dynamiques dans plusieurs langues.
    /// Elle offre des méthodes pour définir la langue actuelle, récupérer des traductions pour une clé donnée,
    /// et obtenir des informations sur les langues et cultures disponibles.
    /// </remarks>
    public static class TranslationManager
    {
        private readonly static List<Type> _listStrings = new List<Type>();
        private static string[] _availableLanguages;
        private static bool _loaded;
        private static bool _updated;
        /// <summary>
        /// Vérifie si les valeurs de traduction ont été chargées.
        /// </summary>
        /// <returns>Vrai si les valeurs ont été chargées, sinon faux.</returns>
        public static bool IsLoaded() => _loaded;
        /// <summary>
        /// Vérifie si les traductions ont été mises à jour.
        /// </summary>
        /// <returns>Vrai si les traductions ont été mises à jour, sinon faux.</returns>
        public static bool IsUpdated() => _updated;
        /// <summary>
        /// Ajoute une classe contenant des traductions et marque le gestionnaire comme chargé.
        /// </summary>
        /// <param name="valueClass">La classe contenant les traductions à ajouter.</param>
        /// <exception cref="ArgumentNullException">Lancé si <paramref name="valueClass"/> est nul.</exception>
        public static void SetValues(Type valueClass)
        {
            ArgumentNullException.ThrowIfNull(valueClass, nameof(valueClass));

            _listStrings.Add(valueClass);

            _loaded = true;
        }
        /// <summary>
        /// Obtient ou définit la langue actuelle.
        /// </summary>
        public static string CurrentLanguage
        {
            get
            {
                return CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            }

            set
            {
                foreach (var strings in _listStrings)
                {
                    CultureInfo.CurrentCulture = new CultureInfo(value);
                    foreach (var method in strings.GetRuntimeMethods())
                    {
                        if (method.Name == "set_Culture")
                        {
                            method.Invoke(null, new[] { CultureInfo.CurrentCulture });
                            break;
                        }
                    }
                }
                _updated = true;
            }
        }

        /// <summary>
        /// Récupère la liste des langues disponibles pour les traductions.
        /// </summary>
        /// <returns>Un tableau des langues disponibles.</returns>
        public static string[] GetAvailableLanguages() { return _availableLanguages; }

        /// <summary>
        /// Récupère la traduction pour une clé donnée dans la langue actuelle.
        /// </summary>
        /// <param name="key">La clé de la traduction à récupérer.</param>
        /// <returns>La traduction correspondant à la clé.</returns>
        public static string GetTranslation(string key)
        {
            if (_listStrings.Count == 0)
                return key;

            foreach (var strings in _listStrings)
            {
                foreach (var method in strings.GetRuntimeMethods())
                {
                    if (method.Name == "get_" + key)
                        return (string)method.Invoke(null, null);
                }
            }
            return key;
        }
        /// <summary>
        /// Récupère la traduction pour une clé donnée dans une langue spécifique.
        /// </summary>
        /// <param name="key">La clé de la traduction à récupérer.</param>
        /// <param name="culture">Le code de la culture pour laquelle récupérer la traduction.</param>
        /// <returns>La traduction correspondant à la clé dans la culture spécifiée.</returns>
        public static string GetTranslation(string key, string culture)
        {
            if (_listStrings.Count == 0)
                return key;

            bool found = false;
            string res = key;

            string previouslanguage = CurrentLanguage;

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            foreach (var strings in _listStrings)
            {
                foreach (var method in strings.GetRuntimeMethods())
                {
                    if (method.Name == "get_" + key)
                    {
                        res = (string)method.Invoke(null, null);
                        found = true;
                        break;
                    }
                }
                if (found)
                    break;
            }
            CurrentLanguage = previouslanguage;

            return res;
        }

        /// <summary>
        /// Obtient la liste des cultures disponibles dans un dossier de ressources donné.
        /// </summary>
        /// <param name="resourceFolder">Le chemin du dossier contenant les ressources de traduction.</param>
        /// <returns>Une liste des cultures disponibles.</returns>
        /// <exception cref="DirectoryNotFoundException">Lancé si le dossier spécifié n'existe pas.</exception>
        public static List<CultureInfo> GetAvailableCultures(string resourceFolder)
        {
            if (!Directory.Exists(resourceFolder))
                throw new DirectoryNotFoundException($"The folder '{resourceFolder}' does not exist.");

            var resourceDirectories = Directory.GetDirectories(resourceFolder);
            var cultures = new List<CultureInfo>();

            foreach (var dir in resourceDirectories)
            {
                var dirName = Path.GetFileName(dir);

                try
                {
                    var culture = new CultureInfo(dirName);
                    if (!cultures.Contains(culture))
                        cultures.Add(culture);
                }
                catch (CultureNotFoundException)
                {
                    // Ignorer les suffixes non valides
                }
            }
            //// Ajouter la culture par défaut (si un fichier sans suffixe culturel est trouvé)
            //if (!cultures.Contains(CultureInfo.InvariantCulture))
            //    cultures.Add(CultureInfo.InvariantCulture);
            cultures.Add(new CultureInfo("en"));

            return cultures;
        }
        /// <summary>
        /// Obtient la longueur maximale d'une traduction pour une clé donnée en utilisant un font.
        /// </summary>
        /// <param name="key">La clé de la traduction à mesurer.</param>
        /// <param name="font">Le font utilisé pour mesurer la longueur de la chaîne.</param>
        /// <returns>La longueur maximale de la traduction sous forme de Vector2.</returns>
        /// <exception cref="ArgumentException">Lancé si <paramref name="key"/> est nul ou vide.</exception>
        /// <exception cref="ArgumentNullException">Lancé si <paramref name="font"/> est nul.</exception>
        public static Vector2 GetTranslationMaxLength(string key, SpriteFont font)
        {
            ArgumentException.ThrowIfNullOrEmpty(key);
            ArgumentNullException.ThrowIfNull(font);

            CheckAvailableLanguages();

            Vector2 maxlength = Vector2.Zero;

            foreach (string language in _availableLanguages)
            {
                string translation = GetTranslation(key, language);

                Vector2 length = font.MeasureString(translation);

                if (maxlength.X < length.X)
                    maxlength.X = length.X;
                if (maxlength.Y < length.Y)
                    maxlength.Y = length.Y;
            }
            return maxlength;
        }

        private static void CheckAvailableLanguages()
        {
            if (_availableLanguages == null || _availableLanguages.Length == 0)
                _availableLanguages = [""];
        }

        /// <summary>
        /// Définit les langues disponibles pour la traduction.
        /// </summary>
        /// <param name="languages">Les langues disponibles sous forme de chaînes.</param>
        public static void SetAvailableLanguages(params string[] languages)
        {
            if (languages == null || languages.Length == 0)
                languages = [""];

            _availableLanguages = languages;
        }
    }
}
