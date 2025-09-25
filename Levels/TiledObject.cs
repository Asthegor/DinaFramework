using Microsoft.Xna.Framework;


namespace DinaFramework.Levels
{
    public class TiledObject
    {
        public string Name { get; set; }
        public string Type { get; set; }
        public uint GID { get; set; }
        public Rectangle Bounds { get; set; }
        public float Rotation { get; set; }
        public TiledObjectType Shape { get; set; } = TiledObjectType.Default;

    }
}
