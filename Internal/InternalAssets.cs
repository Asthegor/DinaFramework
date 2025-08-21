using Microsoft.Xna.Framework.Graphics;

using System.IO;
using System.Reflection;

namespace DinaFramework.Internal
{
    internal static class InternalAssets
    {
        private static Texture2D _circle;
        private static Texture2D _logo;
        public static Texture2D Circle(GraphicsDevice device)
        {
            if (_circle == null)
            {
                Assembly assembly = typeof(InternalAssets).Assembly;
                using Stream stream = assembly.GetManifestResourceStream("DinaFramework.Resources.CircleMask.png");
                if (stream == null)
                    throw new FileNotFoundException("CircleMask.png introuvable dans les ressources embarquées.");
                _circle = Texture2D.FromStream(device, stream);
            }
            return _circle;
        }
        public static Texture2D Logo(GraphicsDevice device)
        {
            if (_logo == null)
            {
                Assembly assembly = typeof(InternalAssets).Assembly;
                using Stream stream = assembly.GetManifestResourceStream("DinaFramework.Resources.Logo.png");
                if (stream == null)
                    throw new FileNotFoundException("Logo.png introuvable dans les ressources embarquées.");
                _logo = Texture2D.FromStream(device, stream);
            }
            return _logo;
        }
    }
}
