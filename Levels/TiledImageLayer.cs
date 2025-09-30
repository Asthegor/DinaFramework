using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace DinaFramework.Levels
{
    public class TiledImageLayer : ILayer
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public float Opacity { get; set; } = 1.0f;
        public Vector2 Offset { get; set; } = Vector2.Zero;
        public bool RepeatX { get; set; }
        public bool RepeatY { get; set; }
        public Texture2D Texture { get; set; }
        public Color Transparency { get; set; }
        public List<IProperty> Properties { get; set; }
        public bool Visible { get; set; }
        public string Parent { get; internal set; } = string.Empty;
    }
}
