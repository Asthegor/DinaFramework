using Microsoft.Xna.Framework.Graphics;

namespace DinaFramework.Levels
{
    public interface ILayer
    {
        public string Name { get; set; }
        public bool Visible { get; set; }
        public float Opacity { get; set; }
        string Parent { get; }
    }
    public interface IProperty
    {
        public string Name { get; }
        public TiledPropertyType Type { get; }
    }
}
