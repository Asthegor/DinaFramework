using Microsoft.Xna.Framework;

using System;
using System.IO;
using System.Reflection;

namespace DinaFramework.Assemblies
{
    /// <summary>
    /// Fournit des méthodes pour charger les assemblages satellites basés sur les ressources localisées.
    /// </summary>
    /// <remarks>
    /// La classe SatelliteAssemblies permet de charger dynamiquement des assemblages satellites
    /// à partir d'un dossier spécifique. Ces assemblages contiennent des ressources localisées pour différentes cultures.
    /// </remarks>
    public static class SatelliteAssemblies
    {
        /// <summary>
        /// Charge les assemblages satellites depuis un dossier de ressources spécifié.
        /// </summary>
        /// <param name="resourceFolder">Le chemin du dossier contenant les assemblages satellites.</param>
        /// <param name="game">L'instance du jeu utilisée pour identifier le nom de l'assemblage principal.</param>
        /// <exception cref="ArgumentNullException">Lancé si <paramref name="game"/> est nul.</exception>
        /// <exception cref="DirectoryNotFoundException">Lancé si le dossier spécifié dans <paramref name="resourceFolder"/> n'existe pas.</exception>
        /// <remarks>
        /// Cette méthode parcourt les sous-dossiers du dossier de ressources et tente de charger les fichiers DLL
        /// correspondant aux assemblages satellites pour le jeu. Les DLL doivent suivre le format
        /// "[NomAssemblage].resources.dll" pour être chargées.
        /// </remarks>
        public static void Load(string resourceFolder, Game game)
        {
            ArgumentNullException.ThrowIfNull(game);

            if (!Directory.Exists(resourceFolder))
                throw new DirectoryNotFoundException(resourceFolder);

            foreach (var dir in Directory.GetDirectories(resourceFolder))
            {
                //string culture = Path.GetFileName(dir);
                string dllPath = Path.Combine(dir, $"{game.GetType().Assembly.GetName().Name}.resources.dll");

                if (File.Exists(dllPath))
                    Assembly.LoadFrom(dllPath);
            }
        }
    }
}