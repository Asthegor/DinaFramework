using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;

namespace DinaFramework.Translation
{
    public static class TranslationManager
    {
        private readonly static List<Type> _listStrings = new List<Type>();
        private static bool _loaded;
        private static bool _updated;
        public static bool IsLoaded() => _loaded;
        public static bool IsUpdated() => _updated;
        public static void SetValues(Type valueClass)
        {
            _listStrings.Add(valueClass);

            _loaded = true;
        }
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
        public static string GetTranslation(string key, string culture)
        {
            if (_listStrings.Count == 0)
                return key;

            bool found = false;
            string res = key;
            
            CultureInfo ciCurrentCulture = CultureInfo.CurrentCulture;

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
            CultureInfo.CurrentCulture = ciCurrentCulture;
            return res;
        }

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

    }
}
