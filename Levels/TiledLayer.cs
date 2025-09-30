using System.Collections.Generic;

namespace DinaFramework.Levels
{
    public class TiledLayer : ILayer
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public float Opacity { get; set; }
        public uint[] Data { get; set; }
        public List<IProperty> Properties { get; set; }
        public bool Visible { get; set; }
        public string Parent { get; internal set; } = string.Empty;
    }
}
