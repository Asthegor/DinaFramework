using DinaFramework.Interfaces;

using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Reflection;

namespace DinaFramework.Translation
{
    public static class TranslationManager
    {
        private static readonly List<string> _translations = new List<string>();
        private static Type _values;
        private static bool _loaded;
        private static bool _updated;

        public static bool IsLoaded() => _loaded;
        public static bool IsUpdated() => _updated;
        public static void SetValues(Type valueClass)
        {
            _values = valueClass ?? throw new ArgumentNullException(nameof(valueClass));

            _translations.Clear();
            foreach (var method in valueClass.GetRuntimeMethods())
            {
                if (method.Name.StartsWith("get_", StringComparison.CurrentCulture))
                {
                    if (method.Invoke(null, null) is string)
                        _translations.Add(method.Name[4..]);
                }
            }
            _loaded = true;
        }
        public static string CurrentLanguage
        {
            get => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
            set

            {
                CultureInfo.CurrentCulture = new CultureInfo(value);
                foreach (var method in _values.GetRuntimeMethods())
                {
                    if (method.Name == "set_Culture")
                    {
                        method.Invoke(null, new[] { CultureInfo.CurrentCulture });
                        break;
                    }
                }
                _updated = true;
            }
        }
        public static string GetTranslation(string key)
        {
            if (_values == null)
                return key;
            foreach (var method in _values.GetRuntimeMethods())
            {
                if (method.Name == "get_" + key)
                    return (string)method.Invoke(null, null);
            }
            return key;
        }
    }
}
