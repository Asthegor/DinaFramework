using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Classe représentant un rectangle de collision pour la gestion des interactions.
    /// </summary>
    public class CollideBox : IPosition, IDimensions, ICollide, IElement, ICopyable<CollideBox>
    {
        Vector2 _position;
        Vector2 _dimensions;
        Rectangle _rect;
        /// <summary>
        /// Position du rectangle de collision.
        /// </summary>
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _rect.Location = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        /// <summary>
        /// Dimensions du rectangle de collision.
        /// </summary>
        public Vector2 Dimensions
        {
            get { return _dimensions; }
            set
            {
                _dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        /// <summary>
        /// Ordre de dessin de l'élément.
        /// </summary>
        public int ZOrder { get; set; }

        /// <summary>
        /// Rectangle de collision calculé en fonction de la position et des dimensions.
        /// </summary>
        public Rectangle Rectangle { get { return _rect; } }
        /// <summary>
        /// Initialise une nouvelle instance de la classe CollideBox avec une position et des dimensions par défaut.
        /// </summary>
        /// <param name="position">Position initiale du rectangle de collision.</param>
        /// <param name="dimensions">Dimensions initiales du rectangle de collision.</param>
        public CollideBox(Vector2 position = default, Vector2 dimensions = default)
        {
            Position = position;
            Dimensions = dimensions;
        }
        /// <summary>
        /// Vérifie si le rectangle de collision actuel intersecte un autre élément de type ICollide.
        /// </summary>
        /// <param name="item">L'élément à tester pour une collision.</param>
        /// <returns>Retourne true si les deux rectangles de collision se chevauchent, sinon false.</returns>
        public bool Collide(ICollide item)
        {
            ArgumentNullException.ThrowIfNull(item);

            return Rectangle.Intersects(item.Rectangle);
        }

        /// <summary>
        /// Crée une copie du CollideBox actuelle avec les mêmes propriétés.
        /// </summary>
        /// <returns>Nouvelle instance de CollideBox avec les mêmes propriétés.</returns>
        public CollideBox Copy()
        {
            return new CollideBox()
            {
                _dimensions = _dimensions,
                _position = _position,
                _rect = _rect,
                Dimensions = Dimensions,
                Position = Position,
                ZOrder = ZOrder
            };
        }
        private CollideBox() { }
    }
}
