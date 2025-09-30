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
        /// <summary>
        /// Échelle de rendu du niveau.
        /// </summary>
        public Vector2 Scale { get; set; } = Vector2.One;

        public Vector2 Offset { get; set; } = Vector2.Zero;


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
        public void AddObjectGroup(TiledObjectGroup obj)
        {
            _objects.Add(obj);
            _datas.Add(obj);
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
        public void AddImageLayer(TiledImageLayer image)
        {
            _images.Add(image);
            _datas.Add(image);
        }
        /// <summary>
        /// Dessine l’ensemble des calques du niveau.
        /// </summary>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch, nameof(spritebatch));
            DrawLayers(spritebatch, _datas);
        }

        private void DrawLayers(SpriteBatch spritebatch, IReadOnlyList<ILayer> datas)
        {
            foreach (var data in datas)
            {
                if (data.Visible == false)
                    continue;

                if (data is TiledGroup tiledGroup)
                    DrawLayers(spritebatch, tiledGroup.Datas);
                else if (data is TiledLayer tiledLayer)
                    DrawLayer(spritebatch, tiledLayer);
                else if (data is TiledObjectGroup tiledObjectGroup)
                    DrawObjects(spritebatch, tiledObjectGroup);
                else if (data is TiledImageLayer tiledImageLayer)
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

                    var rectDest = new Rectangle((int)((x - -Offset.X) * Scale.X), (int)((y - Offset.Y) * Scale.Y), (int)(tileset.TileWidth * Scale.X), (int)(tileset.TileHeight * Scale.Y));
                    
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

                var rectDest = new Rectangle((int)((x - Offset.X) * Scale.X), (int)((y - Offset.Y) * Scale.Y), (int)(tileset.TileWidth * Scale.X), (int)(tileset.TileHeight * Scale.Y));

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

        /// <summary>
        /// Permet de récupérer un calque (layer, groupe, image) ou un groupe d’objets par son nom.
        /// </summary>
        public ILayer? GetLayer(string layerName)
        {
            foreach (var layer in _datas)
            {
                if (layer.Name.Equals(layerName, StringComparison.OrdinalIgnoreCase))
                    return layer;
                if (layer is TiledGroup group)
                {
                    var childLayer = GetLayer(group, layerName);
                    if (childLayer != null)
                        return childLayer;
                }
            }
            return default;
        }
        private static ILayer? GetLayer(TiledGroup tiledGroup, string layerName)
        {
            foreach (var layer in tiledGroup.Datas)
            {
                if (layer.Name.Equals(layerName, StringComparison.OrdinalIgnoreCase))
                    return layer;
                if (layer is TiledGroup group)
                {
                    var childLayer = GetLayer(group, layerName);
                    if (childLayer != null)
                        return childLayer;
                }
            }
            return default;
        }
        /// <summary>
        /// Permet de récupérer l’ID de la tuile (tile) à une position donnée (x, y en pixels).
        /// </summary>
        public uint GetTileIdFromCoord(int x, int y)
        {
            int col = x / TileWidth;
            int row = y / TileHeight;
            int index = row * Width + col;
            foreach (var layer in _datas)
            {
                if (layer is TiledGroup tiledGroup)
                {
                    var tileId = GetTileIdFromCoord(tiledGroup, x, y);
                    if (tileId != 0)
                        return tileId;
                }
                else if (layer is TiledLayer tiledLayer)
                {
                    var tileId = GetTileIdFromCoord(tiledLayer, x, y);
                    if (tileId != 0)
                        return tileId;
                }
            }
            return 0;
        }
        private uint GetTileIdFromCoord(TiledGroup group, int x, int y)
        {
            foreach (var layer in group.Datas)
            {
                if (layer is TiledGroup tiledGroup)
                {
                    var tileId = GetTileIdFromCoord(tiledGroup, x, y);
                    if (tileId != 0)
                        return tileId;
                }
                else if (layer is TiledLayer tiledLayer)
                {
                    var tileId = GetTileIdFromCoord(tiledLayer, x, y);
                    if (tileId != 0)
                        return tileId;
                }
            }
            return 0;
        }
        private uint GetTileIdFromCoord(TiledLayer layer, int x, int y)
        {
            int col = x / TileWidth;
            int row = y / TileHeight;
            int index = row * Width + col;
            if (index < 0 || index >= layer.Data.Length)
                return 0;
            return layer.Data[index];
        }

        /// <summary>
        /// Permet de récupérer la liste des objets (TiledObject) à une position donnée.
        /// </summary>
        public IReadOnlyList<TiledObject> GetObjectsFromCoord(int x, int y)
        {
            List<TiledObject> results = [];
            foreach (var layer in _datas)
            {
                if (layer is TiledGroup tiledGroup)
                {
                    var objs = GetObjectsFromCoord(tiledGroup, x, y);
                    results.AddRange(objs);
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var objs = GetObjectsFromCoord(tiledObjectGroup, x, y);
                    results.AddRange(objs);
                }
            }
            return results;
        }
        private static List<TiledObject> GetObjectsFromCoord(TiledGroup group, int x, int y)
        {
            List<TiledObject> results = [];
            foreach (var layer in group.Datas)
            {
                if (layer is TiledGroup tiledGroup)
                {
                    var objs = GetObjectsFromCoord(tiledGroup, x, y);
                    results.AddRange(objs);
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var objs = GetObjectsFromCoord(tiledObjectGroup, x, y);
                    results.AddRange(objs);
                }
            }
            return results;
        }
        private static List<TiledObject> GetObjectsFromCoord(TiledObjectGroup objectGroup, int x, int y)
        {
            List<TiledObject> results = [];
            foreach (var obj in objectGroup.Objects)
            {
                if (obj.Bounds.Contains(x, y))
                    results.Add(obj);
            }
            return results;
        }
        /// <summary>
        /// Permet de récupérer un objet (TiledObject) par son nom.
        /// </summary>
        public TiledObject? GetObjectFromName(string name)
        {
            if (string.IsNullOrEmpty(name))
                return default;

            foreach (var layer in _datas)
            {
                if (layer is TiledGroup tiledGroup)
                {
                    var obj = GetObjectFromName(tiledGroup, name);
                    if (obj != null)
                        return obj;
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var obj = GetObjectFromName(tiledObjectGroup, name);
                    if (obj != null)
                        return obj;
                }
            }
            return null;
        }
        private static TiledObject? GetObjectFromName(TiledGroup tiledGroup, string name)
        {
            foreach (var layer in tiledGroup.Datas)
            {
                if (layer is TiledGroup childGroup)
                {
                    var obj = GetObjectFromName(childGroup, name);
                    if (obj != null)
                        return obj;
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var obj = GetObjectFromName(tiledObjectGroup, name);
                    if (obj != null)
                        return obj;
                }
            }
            return null;
        }
        private static TiledObject? GetObjectFromName(TiledObjectGroup objectGroup, string name)
        {
            foreach (var obj in objectGroup.Objects)
            {
                if (obj.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return obj;
            }
            return null;
        }

        /// <summary>
        /// Permet de récupérer la liste des objets (TiledObject) d’une certaine classe.
        /// </summary>
        public IReadOnlyList<TiledObject> GetObjectsFromClass(string className)
        {
            List<TiledObject> results = [];
            foreach (var layer in _datas)
            {
                if (layer is TiledGroup tiledGroup)
                {
                    var objs = GetObjectsFromClass(tiledGroup, className);
                    results.AddRange(objs);
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var objs = GetObjectsFromClass(tiledObjectGroup, className);
                    results.AddRange(objs);
                }
            }
            return results;
        }
        private static List<TiledObject> GetObjectsFromClass(TiledGroup tiledGroup, string className)
        {
            List<TiledObject> results = [];
            foreach (var layer in tiledGroup.Datas)
            {
                if (layer is TiledGroup childGroup)
                {
                    var objs = GetObjectsFromClass(childGroup, className);
                    results.AddRange(objs);
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var objs = GetObjectsFromClass(tiledObjectGroup, className);
                    results.AddRange(objs);
                }
            }
            return results;
        }
        private static List<TiledObject> GetObjectsFromClass(TiledObjectGroup objectGroup, string className)
        {
            List<TiledObject> results = [];
            foreach (var obj in objectGroup.Objects)
            {
                if (obj.Class.Equals(className, StringComparison.OrdinalIgnoreCase))
                    results.Add(obj);
            }
            return results;
        }

        /// <summary>
        /// Permet de récupérer un objet (TiledObject) par son ID.
        /// </summary>
        public TiledObject? GetObjectFromId(int id)
        {
            foreach (var layer in _datas)
            {
                if (layer is TiledGroup tiledGroup)
                {
                    var obj = GetObjectFromId(tiledGroup, id);
                    if (obj != null)
                        return obj;
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var obj = GetObjectFromId(tiledObjectGroup, id);
                    if (obj != null)
                        return obj;
                }
            }
            return null;
        }
        private static TiledObject? GetObjectFromId(TiledGroup tiledGroup, int id)
        {
            foreach (var layer in tiledGroup.Datas)
            {
                if (layer is TiledGroup childGroup)
                {
                    var obj = GetObjectFromId(childGroup, id);
                    if (obj != null)
                        return obj;
                }
                else if (layer is TiledObjectGroup tiledObjectGroup)
                {
                    var obj = GetObjectFromId(tiledObjectGroup, id);
                    if (obj != null)
                        return obj;
                }
            }
            return null;
        }
        private static TiledObject? GetObjectFromId(TiledObjectGroup objectGroup, int id)
        {
            foreach (var obj in objectGroup.Objects)
            {
                if (obj.ID == id)
                    return obj;
            }
            return null;
        }
    }
}
