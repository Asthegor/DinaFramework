using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace DinaFramework.Levels
{
    /// <summary>
    /// Représente un niveau Tiled (.tmx), contenant les layers, objets, groupes et tilesets.
    /// Permet le rendu des éléments directement via <see cref="Draw(SpriteBatch)"/>.
    /// </summary>
    public class Level
    {
        /// <summary>
        /// Orientation de la carte (orthogonal, isometric, etc.).
        /// </summary>
        public TiledOrientation Orientation { get; set; }
        /// <summary>
        /// Ordre de rendu défini dans le fichier TMX.
        /// </summary>
        public string RenderOrder { get; set; } = string.Empty;
        /// <summary>
        /// Largeur de la carte (en nombre de tiles).
        /// </summary>
        public int Width { get; set; }
        /// <summary>
        /// Hauteur de la carte (en nombre de tiles).
        /// </summary>
        public int Height { get; set; }
        /// <summary>
        /// Largeur d’une tuile (en pixels).
        /// </summary>
        public int TileWidth { get; set; }
        /// <summary>
        /// Hauteur d’une tuile (en pixels).
        /// </summary>
        public int TileHeight { get; set; }
        /// <summary>
        /// Indique si la carte est infinie (1) ou non (0).
        /// </summary>
        public int Infinite { get; set; }
        /// <summary>
        /// Opacité globale du niveau.
        /// </summary>
        public int Opacity { get; set; }


        private readonly List<TiledTileset> _tilesets = [];
        private readonly List<TiledLayer> _layers = [];
        private readonly List<TiledObjectGroup> _objects = [];
        private readonly List<TiledGroup> _groups = [];
        private readonly List<TiledImageLayer> _images = [];
        private readonly List<ILayer> _datas = [];
        private readonly List<IProperty> _properties = [];

        /// <summary>
        /// Ensemble des tilesets utilisés par le niveau.
        /// </summary>
        public IReadOnlyList<TiledTileset> Tilesets => _tilesets;
        /// <summary>
        /// Liste des layers de type tuile.
        /// </summary>
        public IReadOnlyList<TiledLayer> Layers => _layers;
        /// <summary>
        /// Liste des calques d’objets.
        /// </summary>
        public IReadOnlyList<TiledObjectGroup> Objects => _objects;
        /// <summary>
        /// Liste des groupes (sous-calques regroupés).
        /// </summary>
        public IReadOnlyList<TiledGroup> Groups => _groups;
        /// <summary>
        /// Liste des calques d’images.
        /// </summary>
        public IReadOnlyList<TiledImageLayer> Images => _images;
        /// <summary>
        /// Liste plate de toutes les données de calques (layers, groupes, images).
        /// </summary>
        public IReadOnlyList<ILayer> Datas => _datas;
        /// <summary>
        /// Liste des propriétés personnalisées définies dans le niveau.
        /// </summary>
        public IReadOnlyList<IProperty> Properties => _properties;

        /// <summary>
        /// Ajoute un tileset au niveau.
        /// </summary>
        public void AddTileset(TiledTileset tileset)
        {
            _tilesets.Add(tileset);
        }

        /// <summary>
        /// Ajoute un layer de tuiles au niveau.
        /// </summary>
        public void AddLayer(TiledLayer layer)
        {
            _layers.Add(layer);
            _datas.Add(layer);
        }

        /// <summary>
        /// Ajoute un groupe d’objets au niveau.
        /// </summary>
        public void AddObject(TiledObjectGroup obj)
        {
            _objects.Add(obj);
        }

        /// <summary>
        /// Ajoute un groupe de calques au niveau.
        /// </summary>
        public void AddGroup(TiledGroup group)
        {
            _groups.Add(group);
            _datas.Add(group);
        }

        /// <summary>
        /// Ajoute un calque d’image au niveau.
        /// </summary>
        public void AddImage(TiledImageLayer image)
        {
            _images.Add(image);
            _datas.Add(image);
        }
        /// <summary>
        /// Dessine l’ensemble des calques du niveau.
        /// </summary>
        /// <param name="spritebatch">SpriteBatch utilisé pour le rendu.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch, nameof(spritebatch));
            DrawLayers(spritebatch, _datas);
        }

        private void DrawLayers(SpriteBatch spritebatch, IReadOnlyList<ILayer> datas)
        {
            foreach (var data in datas)
            {
                if (data is TiledGroup tiledGroup)
                    DrawLayers(spritebatch, tiledGroup.Datas);
                if (data is TiledLayer tiledLayer)
                    DrawLayer(spritebatch, tiledLayer);
                if (data is TiledObjectGroup tiledObjectGroup)
                    DrawObjects(spritebatch, tiledObjectGroup);
                if (data is TiledImageLayer tiledImageLayer)
                    DrawImages(spritebatch, tiledImageLayer);
            }
        }

        private void DrawLayer(SpriteBatch spritebatch, TiledLayer tiledLayer)
        {
            for (int row = 0; row < Height; row++)
            {
                for (int col = 0; col < Width; col++)
                {
                    int index = row * Width + col;
                    (int tileid, var rotation, var scaleX, var scaleY) = GetRotation(tiledLayer.Data[index]);
                    if (tileid == 0)
                        continue;

                    var tileset = GetTilesetFromTileId(tileid);
                    if (tileset == null)
                        continue;
                    var rectTile = tileset.GetTile(tileid);

                    var diffHeight = rectTile.Height - TileHeight;
                    var x = col * TileWidth;
                    var y = row * TileHeight;

                    Vector2 origin = Vector2.Zero;
                    SpriteEffects effects = SpriteEffects.None;

                    if (rotation > 0)
                        origin += new Vector2(rectTile.Width - TileWidth, rectTile.Height);
                    else if (rotation < 0)
                        origin += new Vector2(TileWidth, 0);
                    else if (scaleX < 0 && scaleY < 0)
                    {
                        origin += new Vector2(TileWidth - rectTile.Width, rectTile.Height - TileHeight);
                        effects |= SpriteEffects.FlipVertically;
                    }
                    else
                        origin.Y += Math.Abs(diffHeight) > 0 ? diffHeight : 0;

                    var rectDest = new Rectangle(x, y, tileset.TileWidth, tileset.TileHeight);

                    spritebatch.Draw(tileset.Image, rectDest, rectTile, Color.White * tiledLayer.Opacity, rotation, origin, effects, 1);
                }
            }
        }
        private void DrawObjects(SpriteBatch spritebatch, TiledObjectGroup tiledObjectGroup)
        {
            foreach (var tiledobject in tiledObjectGroup.Objects)
            {
                if (tiledobject.GID == 0)
                    continue;

                (var tiledid, var rotation, var scaleX, var scaleY) = GetRotation(tiledobject.GID);
                var tileset = GetTilesetFromTileId(tiledid);

                if (tileset == null)
                    continue;

                var rectTile = tileset.Quads[tiledid];
                rotation += (float)(tiledobject.Rotation * Math.PI) / 180f;
                //rotation = 0;

                var x = tiledobject.Bounds.X;
                var y = tiledobject.Bounds.Y;

                Vector2 origin = Vector2.Zero;
                SpriteEffects effects = SpriteEffects.None;

                if (rotation != 0)
                    origin += new Vector2(tiledobject.Bounds.Width - TileWidth, tiledobject.Bounds.Height);
                else if (scaleX < 0 && scaleY < 0)
                {
                    origin += new Vector2(TileWidth - tiledobject.Bounds.Width, tiledobject.Bounds.Height - TileHeight);
                    effects |= SpriteEffects.FlipVertically;
                }
                else
                    origin.Y = tiledobject.Bounds.Height;

                var rectDest = new Rectangle(x, y, tileset.TileWidth, tileset.TileHeight);

                spritebatch.Draw(tileset.Image, rectDest, rectTile, Color.White * tiledObjectGroup.Opacity, rotation, origin, effects, 1);
            }
        }
        private void DrawImages(SpriteBatch spritebatch, TiledImageLayer imagelayer)
        {
            var height = Height;
        }
        private TiledTileset? GetTilesetFromTileId(int tileid)
        {
            return _tilesets.Find(t => tileid >= t.FirstGid && tileid < t.FirstGid + t.TileCount);
        }
        private static (int tileId, float rotation, int scaleX, int scaleY) GetRotation(uint gid)
        {
            const uint FLIPH = 0x80000000;
            const uint FLIPV = 0x40000000;
            const uint FLIPD = 0x20000000;

            uint numTile = gid;

            float rotation = 0f;
            int sx = 1, sy = 1;

            bool flipX = (numTile & FLIPH) != 0;
            bool flipY = (numTile & FLIPV) != 0;
            bool flipD = (numTile & FLIPD) != 0;

            // enlève les flags pour récupérer l'id brut
            numTile &= ~(FLIPH | FLIPV | FLIPD);

            if (flipX)
            {
                if (flipY && flipD)
                {
                    rotation = MathF.PI / -2f; // -90°
                    sy = -1;
                }
                else if (flipY)
                {
                    sx = -1;
                    sy = -1;
                }
                else if (flipD)
                {
                    rotation = MathF.PI / 2f; // 90°
                }
                else
                {
                    sx = -1;
                }
            }
            else if (flipY)
            {
                if (flipD)
                {
                    rotation = MathF.PI / -2f; // -90°
                }
                else
                {
                    sy = -1;
                }
            }
            else if (flipD)
            {
                rotation = MathF.PI / 2f; // 90°
                sy = -1;
            }

            return ((int)numTile, rotation, sx, sy);
        }
    }
}
