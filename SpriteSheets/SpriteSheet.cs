using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace DinaFramework.SpriteSheets
{
    /// <summary>
    /// Représente une feuille de sprite (spritesheet) contenant une texture et un ensemble de régions nommées.
    /// Permet d'accéder facilement aux sous-textures par leur nom et de gérer les informations de la spritesheet.
    /// </summary>
    public class SpriteSheet
    {
        /// <summary>
        /// Obtient le nom de la spritesheet.
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// Obtient la texture associée à cette spritesheet.
        /// </summary>
        public Texture2D Texture { get; private set; }

        /// <summary>
        /// Obtient le dictionnaire des régions nommées de la spritesheet.
        /// La clé est le nom de la sous-texture et la valeur est son rectangle dans la texture.
        /// </summary>
        public Dictionary<string, Rectangle> Regions { get; } = new();

        /// <summary>
        /// Obtient la région nommée correspondant à la clé fournie.
        /// </summary>
        /// <param name="name">Le nom de la sous-texture à récupérer.</param>
        /// <returns>Le rectangle correspondant à la sous-texture dans la texture.</returns>
        public Rectangle this[string name] => Regions[name];

        /// <summary>
        /// Définit la texture associée à cette spritesheet.
        /// </summary>
        /// <param name="texture">La texture à utiliser.</param>
        internal void SetTexture(Texture2D texture) => Texture = texture;

        /// <summary>
        /// Définit le nom de cette spritesheet.
        /// </summary>
        /// <param name="name">Le nom à attribuer.</param>
        internal void SetName(string name) => Name = name;
    }
}
