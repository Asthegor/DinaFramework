using System;
using System.Collections.Generic;
using System.Globalization;
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
            ArgumentNullException.ThrowIfNull(nameof(valueClass));

            _listStrings.Add(valueClass);

            _loaded = true;
        }
        public static string CurrentLanguage
        {
            get => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
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
    }
}
