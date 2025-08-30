using Microsoft.Xna.Framework.Graphics;

using System.IO;
using System.Reflection;

namespace DinaFramework.Internal
{
    internal static class InternalAssets
    {
        private static Texture2D? _circle;
        private static Texture2D? _logo;
        private static Texture2D? _dropDownArrow;
        public static Texture2D Circle(GraphicsDevice device)
        {
            _circle ??= GetInternalResource(device, "CircleMask.png");
            return _circle;
        }
        public static Texture2D Logo(GraphicsDevice device)
        {
            _logo ??= GetInternalResource(device, "Logo.png");
            return _logo;
        }
        public static Texture2D DropDownArrow(GraphicsDevice device)
        {
            _dropDownArrow ??= GetInternalResource(device, "DropDownArrow.png");
            return _dropDownArrow;
        }
        private static Texture2D GetInternalResource(GraphicsDevice device, string filename)
        {
            Assembly assembly = typeof(InternalAssets).Assembly;
            using Stream? stream = assembly.GetManifestResourceStream($"DinaFramework.Resources.{filename}");
            if (stream == null)
                throw new FileNotFoundException($"{filename} introuvable dans les ressources embarquées.");
            return Texture2D.FromStream(device, stream);
        }
    }
}
