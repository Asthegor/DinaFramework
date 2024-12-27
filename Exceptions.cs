using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinaFramework.Exceptions
{
#pragma warning disable CA1032 // Implémenter des constructeurs d'exception standard
    public class InvalidSceneTypeException(Type type) : Exception($"The type '{type.Name}' must be a class that inherits from 'Scene' and implements 'ILoadingScreen'.") { }
    public class DuplicateDictionaryKeyException(string key) : Exception($"The key '{key}' is already in the Dictionary") { }
#pragma warning restore CA1032 // Implémenter des constructeurs d'exception standard
}
