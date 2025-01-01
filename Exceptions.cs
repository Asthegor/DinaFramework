using System;

#pragma warning disable CA1032 // Implémenter des constructeurs d'exception standard
#pragma warning disable CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement

namespace DinaFramework.Exceptions
{

    public class InvalidSceneTypeException(Type type) : Exception($"The type '{type.Name}' must be a class that inherits from 'Scene' and implements 'ILoadingScreen'.") { }
    public class DuplicateDictionaryKeyException(string key) : Exception($"The key '{key}' is already in the Dictionary") { }


}
#pragma warning restore CA1032 // Implémenter des constructeurs d'exception standard
#pragma warning restore CS1591 // Commentaire XML manquant pour le type ou le membre visible publiquement
