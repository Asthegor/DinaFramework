using System;

namespace DinaFramework.Exceptions
{
    /// <summary>
    /// Exception levée lorsqu'un type de scène invalide est fourni.
    /// </summary>
    public class InvalidSceneTypeException(Type type) : Exception($"The type '{type.Name}' must be a class that inherits from 'Scene' and implements 'ILoadingScreen'.") { }
    /// <summary>
    /// Exception levée lorsqu'une clé en double est ajoutée à un dictionnaire.
    /// </summary>
    public class DuplicateDictionaryKeyException(string key) : Exception($"The key '{key}' is already in the Dictionary") { }
    /// <summary>
    /// Exception levée lorsqu'une ressource n'est pas trouvée.
    /// </summary>
    public class SpriteBatchNotBeginException() : Exception($"You must launch BeginSpritebatch in order to launch EndSpritebatch.");
}
