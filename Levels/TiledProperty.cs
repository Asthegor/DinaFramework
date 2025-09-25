namespace DinaFramework.Levels
{
    public class TiledProperty<T> : IProperty
    {
        public string Name { get; set; }
        public TiledPropertyType Type { get; set; } = TiledPropertyType.String;
        public T Value { get; set; }
    }
}
