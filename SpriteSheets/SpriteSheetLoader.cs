using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.Linq;

namespace DinaFramework.SpriteSheets
{

    /// <summary>
    /// Classe permettant de charger un SpriteSheet avec son fichier de position (xml)
    /// </summary>
    public static class SpriteSheetLoader
    {
        private sealed class ParsedSheet
        {
            public string Name;
            public Texture2D FullTexture;
            public Dictionary<string, Rectangle> Regions = new();
        }

        private static readonly Dictionary<string, Texture2D> textureCache = new();

        private static ParsedSheet LoadSpriteSheet(ContentManager content, string xmlPath, string texturePath)
        {

            // Charger la texture avec cache
            if (!textureCache.TryGetValue(texturePath, out var texture))
            {
                texture = content.Load<Texture2D>(texturePath);
                textureCache[texturePath] = texture;
            }

            var parsedSheet = new ParsedSheet { Name = texturePath };
            parsedSheet.FullTexture = texture;

            // Charger le XML
            XDocument doc;
            using (var stream = TitleContainer.OpenStream(xmlPath))
                doc = XDocument.Load(stream);

            foreach (var elem in doc.Descendants("SubTexture"))
            {
                string name = elem.Attribute("name").Value;
                int x = int.Parse(elem.Attribute("x").Value, CultureInfo.InvariantCulture);
                int y = int.Parse(elem.Attribute("y").Value, CultureInfo.InvariantCulture);
                int w = int.Parse(elem.Attribute("width").Value, CultureInfo.InvariantCulture);
                int h = int.Parse(elem.Attribute("height").Value, CultureInfo.InvariantCulture);

                parsedSheet.Regions[name] = new Rectangle(x, y, w, h);
            }
            return parsedSheet;
        }
        /// <summary>
        /// Charge une spritesheet à partir d’un fichier XML et d’une texture associée.
        /// </summary>
        /// <param name="content">Le ContentManager utilisé pour charger la texture.</param>
        /// <param name="xmlPath">Chemin du fichier XML décrivant les régions de la spritesheet.</param>
        /// <param name="texturePath">Chemin de la texture associée à la spritesheet.</param>
        /// <returns>
        /// Une instance de <see cref="SpriteSheet"/> contenant la texture et toutes les régions définies.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Lancée si <paramref name="content"/> est null.
        /// </exception>
        public static SpriteSheet Load(ContentManager content, string xmlPath, string texturePath)
        {
            ArgumentNullException.ThrowIfNull(content, nameof(content));
            var parsedSheet = LoadSpriteSheet(content, xmlPath, texturePath);
            var spriteSheet = new SpriteSheet();
            spriteSheet.SetTexture(parsedSheet.FullTexture);
            foreach(var kvp in parsedSheet.Regions)
                spriteSheet.Regions[kvp.Key] = kvp.Value;
            return spriteSheet;
            
        }
        /// <summary>
        /// Vide le cache de textures (utile si on change de contenu en runtime)
        /// </summary>
        public static void ClearCache()
        {
            textureCache.Clear();
            subTextureCache.Clear();
        }

        private static Dictionary<string, Texture2D> subTextureCache = new Dictionary<string, Texture2D>();
        /// <summary>
        /// Extrait une sous-texture à partir d’une spritesheet et la met en cache.
        /// </summary>
        /// <param name="graphics">Le périphérique graphique (<see cref="GraphicsDevice"/>) utilisé pour créer la texture.</param>
        /// <param name="spriteSheet">La spritesheet contenant la texture et ses régions.</param>
        /// <param name="textureName">Le nom de la sous-texture à extraire.</param>
        /// <returns>
        /// Une nouvelle <see cref="Texture2D"/> correspondant à la sous-texture demandée.
        /// Si elle existe déjà dans le cache, la version en cache est retournée.
        /// </returns>
        /// <exception cref="ArgumentNullException">
        /// Lancée si <paramref name="spriteSheet"/> est null.
        /// </exception>
        /// <exception cref="KeyNotFoundException">
        /// Lancée si <paramref name="textureName"/> n’existe pas dans la spritesheet.
        /// </exception>
        public static Texture2D LoadSubTexture(GraphicsDevice graphics, SpriteSheet spriteSheet, string textureName)
        {
            ArgumentNullException.ThrowIfNull(spriteSheet);

            // Clé unique combinant le nom de la spritesheet et le nom du sprite
            string cacheKey = $"{spriteSheet.Name}/{textureName}";

            if (subTextureCache.TryGetValue(cacheKey, out var cachedTexture))
                return cachedTexture;

            if (!spriteSheet.Regions.TryGetValue(textureName, out Rectangle rect))
                throw new KeyNotFoundException($"Le sprite '{textureName}' n'existe pas dans la spritesheet '{spriteSheet.Name}'.");

            // Extraire les pixels
            Color[] pixels = new Color[rect.Width * rect.Height];
            spriteSheet.Texture.GetData(0, rect, pixels, 0, pixels.Length);

            // Créer la texture
            Texture2D texture = new Texture2D(graphics, rect.Width, rect.Height);
            texture.SetData(pixels);

            // Stocker dans le cache
            subTextureCache[cacheKey] = texture;

            return texture;
        }

    }

}
