using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Extensions
{
    /// <summary>
    /// Fournit des méthodes d'extension pour convertir entre différents types de données.
    /// </summary>
    public static class ConvertExtensions
    {
        /// <summary>
        /// Convertit un vecteur 2D en un point entier.
        /// </summary>
        /// <param name="vector">Le vecteur à convertir.</param>
        /// <returns>Un Point représentant les coordonnées entières du vecteur.</returns>
        public static Point ToPoint(this Vector2 vector) => new Point(Convert.ToInt32(vector.X), Convert.ToInt32(vector.Y));
        /// <summary>
        /// Récupère les dimensions d'une texture sous forme d'un vecteur 2D.
        /// </summary>
        /// <param name="texture">La texture à partir de laquelle extraire les dimensions.</param>
        /// <returns>Un Vector2 contenant la largeur et la hauteur de la texture.</returns>
        /// <exception cref="ArgumentNullException">Si la texture fournie est <c>null</c>.</exception>
        public static Vector2 ToVector2(this Texture2D texture)
        {
            ArgumentNullException.ThrowIfNull(texture, nameof(texture));
            return new Vector2(texture.Width, texture.Height);
        }
    }
}
