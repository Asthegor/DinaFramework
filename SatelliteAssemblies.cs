using Microsoft.Xna.Framework;

using System;
using System.IO;
using System.Reflection;

namespace DinaFramework.Assemblies
{
    public static class SatelliteAssemblies
    {
        public static void Load(string resourceFolder, Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            if (!Directory.Exists(resourceFolder))
                throw new DirectoryNotFoundException(resourceFolder);

            foreach (var dir in Directory.GetDirectories(resourceFolder))
            {
                string culture = Path.GetFileName(dir);
                string dllPath = Path.Combine(dir, $"{game.GetType().Assembly.GetName().Name}.resources.dll");

                if (File.Exists(dllPath))
                    Assembly.LoadFrom(dllPath);
            }
        }
    }
}