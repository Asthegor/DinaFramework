using DinaFramework.Internal;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

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
        public static void DrawRectangle(this SpriteBatch sb, Texture2D pixel, Rectangle rect, Color color, int thickness = 1)
        {
            ArgumentNullException.ThrowIfNull(sb);
            ArgumentNullException.ThrowIfNull(pixel);

            sb.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color); // top
            sb.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color); // left
            sb.Draw(pixel, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height), color); // right
            sb.Draw(pixel, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness), color); // bottom
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
        /// 
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="pos"></param>
        /// <param name="dim"></param>
        /// <param name="radius"></param>
        /// <param name="color"></param>
        public static void MaskCorners(this SpriteBatch sb, Vector2 pos, Vector2 dim, int radius, Color color)
        {
            ArgumentNullException.ThrowIfNull(sb);



            Texture2D circle = InternalAssets.Circle(sb.GraphicsDevice); // 512x512
            if (circle == null)
                return;

            int srcSize = circle.Width / 2; // 256

            Rectangle src = new Rectangle(0, 0, srcSize, srcSize);
            Rectangle dst = new Rectangle((int)pos.X, (int)pos.Y, radius, radius);

            // Top-Left
            src.X = 0;
            src.Y = 0;
            dst.X = (int)pos.X;
            dst.Y = (int)pos.Y;
            sb.Draw(circle, dst, src, color);

            // Top-Right
            src.X = srcSize;
            src.Y = 0;
            dst.X = (int)(pos.X + dim.X - radius);
            dst.Y = (int)(pos.Y);
            sb.Draw(circle, dst, src, color);

            // Bottom-Right
            src.X = srcSize;
            src.Y = srcSize;
            dst.X = (int)(pos.X + dim.X - radius);
            dst.Y = (int)(pos.Y + dim.Y - radius);
            sb.Draw(circle, dst, src, color);

            // Bottom-Left
            src.X = 0;
            src.Y = srcSize;
            dst.X = (int)(pos.X);
            dst.Y = (int)(pos.Y + dim.Y - radius);
            sb.Draw(circle, dst, src, color);
        }

        public static void DrawArc(this SpriteBatch sb, Color color, Rectangle rect, float radius, float startAngle, float endAngle)
        {
            // Créer un bitmap pour l'arc
            Texture2D arcBitmap = sb.CreateArcBitmap(color, new Rectangle((int)rect.X, (int)rect.Y, (int)radius * 2, (int)radius * 2), radius, startAngle, endAngle);

            // Dessiner le bitmap sur le spriteBatch
            sb.Draw(arcBitmap, new Rectangle(rect.X, rect.Y, (int)radius * 2, (int)radius * 2), color);

            //arcBitmap.Dispose();
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
        public static List<string> GetModifiedKeys(this Dictionary<string, object> sourceDictionary, Dictionary<string, object> otherDictionary)
        {
            ArgumentNullException.ThrowIfNull(otherDictionary);
            if (otherDictionary.Count == 0)
                return new List<string>();

            List<string> modifiedKeys = new List<string>();

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
            return modifiedKeys;
        }
    }
}
