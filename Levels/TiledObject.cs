using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;


namespace DinaFramework.Levels
{
    public class TiledObject
    {
        public string Name { get; set; }
        public string Class { get; set; }
        public uint GID { get; set; }
        public Rectangle Bounds { get; set; }
        public float Rotation { get; set; }
        public bool Visible { get; set; }
        public TiledObjectType Shape { get; set; } = TiledObjectType.Default;
        public List<IProperty> Properties { get; set; } = [];
        public string Parent { get; internal set; } = string.Empty;
        public int ID { get; internal set; }

        /// <summary>
        /// Permet de récupérer une propriété par son nom
        /// </summary>
        public IProperty? GetProperty(string name)
        {
            foreach (var property in Properties)
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase))
                    return property;
            }
            return null;
        }
        /// <summary>
        /// Permet de récupérer une propriété par son nom et son type.
        /// </summary>
        public TiledProperty<T>? GetPropertyAs<T>(string name)
        {
            foreach (var property in Properties)
            {
                if (property.Name.Equals(name, StringComparison.OrdinalIgnoreCase) && property is TiledProperty<T> typedProperty)
                    return typedProperty;
            }
            return null;
        }
        /// <summary>
        /// Permet de recalculer la vraie position de l'objet à l'écran.
        /// L'erreur de calcul provient du fait que Tiled place l'origine des objets utilisant une image (GID != 0) en bas à gauche.
        /// </summary>
        public bool Contains(Point point)
        {
            if (GID == 0)
                return Bounds.Contains(point);

            Point size = new Point(Bounds.Width, Bounds.Height);
            Point location = Bounds.Location - new Point(0, Bounds.Height);
            Rectangle rect = new Rectangle(location, size);
            return rect.Contains(point);
        }
    }
}
