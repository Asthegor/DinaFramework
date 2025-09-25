using DinaFramework.Internal;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace DinaFramework.Extensions
{
    /// <summary>
    /// Fournit des méthodes d'extension pour convertir entre différents types de données.
    /// </summary>
    public static class ConvertExtensions
    {
        #region Vector2
        /// <summary>
        /// Convertit un vecteur 2D en un point entier.
        /// </summary>
        /// <param name="vector">Le vecteur à convertir.</param>
        /// <returns>Un Point représentant les coordonnées entières du vecteur.</returns>
        public static Point ToPoint(this Vector2 vector) => new Point(Convert.ToInt32(vector.X), Convert.ToInt32(vector.Y));
        #endregion

        #region Point
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
        #endregion

        #region SpriteBatch
        /// <summary>
        /// Dessine les contours d’un rectangle à l’aide d’une texture, d’une couleur et d’une épaisseur.
        /// Le contour est composé de 4 segments : haut, bas, gauche et droite.
        /// Utile pour encadrer des éléments UI ou des blocs de jeu.
        /// </summary>
        /// <param name="sb">SpriteBatch utilisé pour le rendu.</param>
        /// <param name="pixel">Texture utilisée pour dessiner les bords. Doit être une texture pleine, comme un pixel blanc.</param>
        /// <param name="rect">Rectangle cible à entourer.</param>
        /// <param name="color">Couleur du contour.</param>
        /// <param name="thickness">Épaisseur du contour en pixels (par défaut : 1).</param>
        /// <param name="isFilled">Indique si le rectangle doit être plein.</param>
        public static void DrawRectangle(this SpriteBatch sb, Texture2D pixel, Rectangle rect, Color color, int thickness = 1, bool isFilled = false)
        {
            ArgumentNullException.ThrowIfNull(sb);
            ArgumentNullException.ThrowIfNull(pixel);

            if (isFilled)
                sb.Draw(pixel, rect, color);
            else
            {
                sb.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color); // top
                sb.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color); // left
                sb.Draw(pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color); // right
                sb.Draw(pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color); // bottom
            }
        }
        /// <summary>
        /// Permet de dessiner une ligne entre deux points en utilisant une texture pixel.
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pixel"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="color"></param>
        /// <param name="thickness"></param>
        public static void DrawLine(this SpriteBatch sb, Texture2D pixel, Color color, Vector2 start, Vector2 end, int thickness = 1)
        {
            ArgumentNullException.ThrowIfNull(sb);
            ArgumentNullException.ThrowIfNull(pixel);
            Vector2 direction = end - start;
            float length = direction.Length();
            if (length < 1f)
                return;
            direction.Normalize();
            // Calculer l'angle de rotation
            float angle = (float)Math.Atan2(direction.Y, direction.X);
            // Dessiner la ligne
            sb.Draw(pixel, start, null, color, angle, Vector2.Zero, new Vector2(length, thickness), SpriteEffects.None, 0f);
        }
        /// <summary>
        /// Dessine des masques arrondis sur les quatre coins d’un rectangle
        /// en utilisant une texture de cercle prédéfinie.
        /// </summary>
        /// <param name="sb">Le <see cref="SpriteBatch"/> utilisé pour le rendu.</param>
        /// <param name="pos">Position (coin supérieur gauche) du rectangle.</param>
        /// <param name="dim">Dimensions du rectangle.</param>
        /// <param name="radius">Rayon des coins arrondis.</param>
        /// <param name="color">Couleur appliquée au masque.</param>
        public static void MaskCorners(this SpriteBatch sb, Vector2 pos, Vector2 dim, int radius, Color color)
        {
            ArgumentNullException.ThrowIfNull(sb);
            Texture2D circle = InternalAssets.Circle(sb.GraphicsDevice); // 512x512
            if (circle == null)
                return;
            int srcSize = circle.Width / 2; // 256

            Rectangle src = new Rectangle(0, 0, srcSize, srcSize);
            Rectangle dst = new Rectangle((int)pos.X, (int)pos.Y, radius, radius);

            (int srcX, int srcY, int dstX, int dstY)[] corners =
            [
                (0, 0, (int)pos.X, (int)pos.Y),                                                     // Top-Left
                (srcSize, 0, (int)(pos.X + dim.X - radius), (int)pos.Y),                            // Top-Right
                (0, srcSize, (int)pos.X, (int)(pos.Y + dim.Y - radius)),                            // Bottom-Left
                (srcSize, srcSize, (int)(pos.X + dim.X - radius), (int)(pos.Y + dim.Y - radius)),   // Bottom-Right
            ];

            foreach (var (sx, sy, dx, dy) in corners)
            {
                src.X = sx;
                src.Y = sy;
                dst.X = dx;
                dst.Y = dy;
                sb.Draw(circle, dst, src, color);
            }
        }
        /// <summary>
        /// Dessine un arc de cercle dans une zone donnée.
        /// </summary>
        /// <param name="sb">Le <see cref="SpriteBatch"/> utilisé pour le rendu.</param>
        /// <param name="color">Couleur de l’arc.</param>
        /// <param name="rect">Rectangle définissant la zone où l’arc sera placé.</param>
        /// <param name="radius">Rayon de l’arc.</param>
        /// <param name="startAngle">Angle de départ (en radians).</param>
        /// <param name="endAngle">Angle de fin (en radians).</param>
        public static void DrawArc(this SpriteBatch sb, Color color, Rectangle rect, float radius, float startAngle, float endAngle)
        {
            ArgumentNullException.ThrowIfNull(sb, nameof(sb));
            using Texture2D arcBitmap = sb.CreateArcBitmap(color, new Rectangle((int)rect.X, (int)rect.Y, (int)radius * 2, (int)radius * 2), radius, startAngle, endAngle);
            sb.Draw(arcBitmap, new Rectangle(rect.X, rect.Y, (int)radius * 2, (int)radius * 2), color);
        }
        private static Texture2D CreateArcBitmap(this SpriteBatch sb, Color color, Rectangle arcRect, float radius, float startAngle, float endAngle)
        {
            Texture2D bitmap = new Texture2D(sb.GraphicsDevice, arcRect.Width, arcRect.Height);
            Color[] data = new Color[arcRect.Width * arcRect.Height];

            for (int y = 0; y < arcRect.Height; y++)
            {
                for (int x = 0; x < arcRect.Width; x++)
                {
                    float dx = x - radius;
                    float dy = y - radius;
                    float dist = (float)Math.Sqrt(dx * dx + dy * dy);

                    if (dist <= radius)
                    {
                        float angle = (float)Math.Atan2(dy, dx);
                        angle += angle > 0 ? 0 : (float)(Math.PI * 2); // Convertir en radians positifs

                        if (angle >= startAngle && angle <= endAngle)
                        {
                            data[y * arcRect.Width + x] = color;
                        }
                    }
                }
            }

            bitmap.SetData(data);
            return bitmap;
        }
        #endregion

        #region Dictionary
        /// <summary>
        /// Compares two dictionaries and returns a list of keys whose values differ between them.
        /// </summary>
        /// <remarks>This method performs a key-by-key comparison between the two dictionaries. A key is
        /// considered "modified" if it exists in both dictionaries  but the values associated with the key are not
        /// equal. Equality is determined using the <see cref="object.Equals(object, object)"/> method.</remarks>
        /// <param name="sourceDictionary">The source dictionary to compare.</param>
        /// <param name="otherDictionary">The dictionary to compare against the source dictionary.</param>
        /// <returns>A list of keys from the source dictionary whose values differ from the corresponding values in the other
        /// dictionary.  If no values differ, the list will be empty.</returns>
        public static Collection<string> GetModifiedKeys(this Dictionary<string, object> sourceDictionary, Dictionary<string, object> otherDictionary)
        {
            ArgumentNullException.ThrowIfNull(sourceDictionary);
            ArgumentNullException.ThrowIfNull(otherDictionary);
            if (otherDictionary.Count == 0)
                return [];

            List<string> modifiedKeys = [];

            foreach (KeyValuePair<string, object> kvp in sourceDictionary)
            {
                if (otherDictionary.ContainsKey(kvp.Key)) // Vérifie si la clé existe dans l'autre dictionnaire
                {
                    if (!Equals(kvp.Value, otherDictionary[kvp.Key]))
                    {
                        modifiedKeys.Add(kvp.Key);
                    }
                }
            }
            return [..  modifiedKeys];
        }
        #endregion

    }
}
