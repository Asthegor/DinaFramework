using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace DinaFramework.Levels
{
    public class TiledTileset
    {
        public int FirstGid { get; internal set; } = 1;
        public string Name { get; set; }
        public string Class { get; set; }
        public int TileWidth { get; internal set; } = 0;
        public int TileHeight { get; internal set; } = 0;
        public int Spacing { get; internal set; } = 0;
        public int Margin { get; internal set; } = 0;
        public int TileCount { get; internal set; } = 0;
        public int Columns { get; internal set; } = 0;
        public bool Visible { get; internal set; }
        public Vector2 TileOffset { get; internal set; }
        public bool HorizontalFlip { get; internal set; }
        public bool VerticalFlip { get; internal set; }
        public bool Rotate { get; internal set; }
        public bool PreferUntransformed { get; internal set; }
        public Color Transparency { get; internal set; } = Color.Transparent;
        public Texture2D Image { get; internal set; }
        public Dictionary<int, Rectangle> Quads { get; internal set; } = [];
        public List<IProperty> Properties { get; set; } = [];

        public Rectangle GetTile(int tileid)
        {
            return Quads[tileid];
        }
    }
}
