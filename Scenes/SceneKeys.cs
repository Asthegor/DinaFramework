using DinaFramework.Services;

namespace DinaFramework.Scenes
{

    /// <summary>
    /// Représente un tag vide utilisé pour typer les clés de scène.
    /// </summary>
    public sealed class SceneTag { }
    internal static class SceneKeys
    {
        public static readonly Key<SceneTag> FrameworkLogo = Key<SceneTag>.FromString("__FrameworkLogo__");
    }
}
