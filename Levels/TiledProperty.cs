namespace DinaFramework.Levels
{
    public class TiledProperty<T> : IProperty
    {
        public int ID { get; internal set; }
        public string Name { get; internal set; }
        public TiledPropertyType Type { get; internal set; } = TiledPropertyType.String;
        public T Value { get; internal set; }
    }
}
