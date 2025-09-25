using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente un sprite (image) avec des propriétés telles que la couleur, la position, la rotation, le redimensionnement et l'effet de réflexion.
    /// </summary>
    public class Sprite : Base, IColor, IVisible, IDraw, ICollide, ICopyable<Sprite>
    {
        private Rectangle _rectangle;
        private Color _color;
        private Vector2 _origin;
        private bool _visible;
        private Texture2D _texture;
        private SpriteEffects _effects;
        private Vector2 _flip;
        private Vector2 _scale;

        /// <summary>
        /// Initialise une nouvelle instance de la classe Sprite avec les paramètres spécifiés.
        /// </summary>
        /// <param name="texture">La texture (image) à afficher.</param>
        /// <param name="color">La couleur du sprite.</param>
        /// <param name="position">La position du sprite dans l'espace.</param>
        public Sprite(Texture2D texture, Color color, Vector2 position) :
            this(texture, color, position, null, Vector2.Zero, Vector2.One, 0, Vector2.One, SpriteEffects.None, 0)
        { }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Sprite avec les paramètres spécifiés.
        /// </summary>
        /// <param name="texture">La texture (image) à afficher.</param>
        /// <param name="color">La couleur du sprite.</param>
        /// <param name="position">La position du sprite dans l'espace.</param>
        /// <param name="sourceRectangle">La portion de la texture à afficher.</param>
        /// <param name="origin">L'origine du sprite pour la rotation.</param>
        /// <param name="flip">Les effets de réflexion du sprite.</param>
        /// <param name="rotation">L'angle de rotation du sprite.</param>
        /// <param name="scale">Le facteur de mise à l'échelle du sprite.</param>
        /// <param name="effects">Les effets de sprite (par exemple, inversion horizontale ou verticale).</param>
        /// <param name="zOrder">L'ordre de superposition du sprite.</param>
        public Sprite(Texture2D texture, Color color, Vector2 position, Rectangle? sourceRectangle, Vector2 origin, Vector2 flip, float rotation, Vector2 scale, SpriteEffects effects, int zOrder)
            : base(position, new Vector2(sourceRectangle?.Width ?? texture?.Width ?? 0, sourceRectangle?.Height ?? texture?.Height ?? 0), zOrder)
        {
            _texture = texture ?? throw new ArgumentNullException(nameof(texture));
            _color = color;
            _origin = origin;
            _visible = true;
            Rotation = rotation;
            _scale = scale;
            _effects = effects;
            Flip = flip;

            if (sourceRectangle.HasValue)
                Rectangle = sourceRectangle.Value;
            else
                Rectangle = new Rectangle((int)Position.X, (int)Position.Y, texture.Width, texture.Height);
        }

        //public Sprite(Texture2D texture, Color color, int zorder = default) : base(default, default, zorder)
        //{
        //    Texture = texture;
        //    Color = color;
        //    Visible = true;
        //    Dimensions = new Vector2(_texture.Width, _texture.Height);
        //}
        //public Sprite(Texture2D texture, Color color, Vector2 position, int zorder) : this(texture, color, zorder)
        //{
        //    Position = position;
        //}
        //public Sprite(Texture2D texture, Color color, Vector2 position, Vector2 dimensions, int zorder) : this(texture, color, position, zorder)
        //{
        //    Dimensions = dimensions;
        //}
        //public Sprite(Texture2D texture, Color color, Vector2 position, Vector2 dimensions, float rotation = default, Vector2 origin = default,
        //              Vector2 flip = default, int zorder = default) : this(texture, color, position, dimensions, zorder)
        //{
        //    Rectangle = new Rectangle(Convert.ToInt32(Position.X), Convert.ToInt32(Position.Y), Convert.ToInt32(Dimensions.X), Convert.ToInt32(Dimensions.Y));
        //    Rotation = rotation;
        //    Origin = origin;
        //    Flip = flip == default ? Vector2.One : flip;
        //}
        //public Sprite(Texture2D texture, Color color, Vector2 position, float rotation, Vector2 origin = default, Vector2 scale = default,
        //              Vector2 flip = default, int zorder = default) : this(texture, color, position, zorder)
        //{
        //    Scale = scale;
        //    Rotation = rotation;
        //    Origin = origin;
        //    Flip = flip == default ? Vector2.One : flip;
        //    Rectangle = new Rectangle(Convert.ToInt32(Position.X), Convert.ToInt32(Position.Y), Convert.ToInt32(Dimensions.X), Convert.ToInt32(Dimensions.Y));
        //}
        //public Sprite(Sprite sprite)
        //{
        //    ArgumentNullException.ThrowIfNull(sprite);

        //    Texture = sprite.Texture;
        //    Rectangle = sprite.Rectangle;
        //    Color = sprite.Color;
        //    Position = sprite.Position;
        //    Dimensions = sprite.Dimensions;
        //    Origin = sprite.Origin;
        //    Visible = sprite.Visible;
        //    Rotation = sprite.Rotation;
        //    Scale = sprite.Scale;
        //    Flip = sprite.Flip;
        //    _effects = sprite._effects;
        //}
        /// <summary>
        /// Obtient ou définit la texture du sprite.
        /// </summary>
        public Texture2D Texture
        {
            get { return _texture; }
            set
            {
                ArgumentNullException.ThrowIfNull(value);
                _texture = value;
            }
        }
        /// <summary>
        /// Obtient ou définit le rectangle représentant la zone d'affichage du sprite.
        /// </summary>
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            private set { _rectangle = value; }
        }
        /// <summary>
        /// Obtient ou définit la couleur du sprite.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        /// <summary>
        /// Obtient ou définit la position du sprite.
        /// </summary>
        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                _rectangle.Location = value.ToPoint();
                //_rectangle.Location = new Point(Convert.ToInt32(value.X - Origin.X), Convert.ToInt32(value.Y - Origin.Y));
            }
        }
        /// <summary>
        /// Obtient ou définit les dimensions du sprite.
        /// </summary>
        public override Vector2 Dimensions
        {
            get
            {
                //if (base.Dimensions == default)
                //    return new Vector2(_texture.Width, _texture.Height) * Scale;
                return base.Dimensions;
            }
            set
            {
                base.Dimensions = value;
                _rectangle.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        /// <summary>
        /// Obtient ou définit l'origine du sprite pour la rotation.
        /// </summary>
        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        /// <summary>
        /// Centre l'origine du sprite sur son centre.
        /// </summary>
        public void CenterOrigin() { _origin = new Vector2(Texture.Width, Texture.Height) / 2.0f; }
        /// <summary>
        /// Obtient ou définit la visibilité du sprite.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        /// <summary>
        /// Définit la visibilité du sprite (utilisé dans le MenuManager).
        /// </summary>
        /// <param name="value">La valeur de visibilité (true ou false).</param>
        public void SetVisible(bool value = false)
        {
            _visible = value;
        }
        /// <summary>
        /// Obtient ou définit la rotation du sprite en radians.
        /// </summary>
        public float Rotation { get; set; }
        /// <summary>
        /// Obtient ou définit l'échelle du sprite.
        /// </summary>
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                _rectangle.Size = new Vector2(Dimensions.X * _scale.X, Dimensions.Y * _scale.Y).ToPoint();
            }
        }
        /// <summary>
        /// Obtient ou définit l'effet de réflexion du sprite (horizontal ou vertical).
        /// </summary>
        public Vector2 Flip
        {
            get { return _flip; }
            set
            {
                if (value.X != 0)
                    value.X /= Math.Abs(value.X);
                if (value.Y != 0)
                    value.Y /= Math.Abs(value.Y);
                _effects = SpriteEffects.None;
                if (value.X < 0)
                    _effects |= SpriteEffects.FlipHorizontally;
                if (value.Y < 0)
                    _effects |= SpriteEffects.FlipVertically;
                _flip = value;
            }
        }
        /// <summary>
        /// Dessine le sprite sur l'écran avec un SpriteBatch.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour dessiner le sprite.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (Visible)
            {
                ArgumentNullException.ThrowIfNull(spritebatch);

                if (Dimensions == default)
                    spritebatch.Draw(Texture, Position, new Rectangle(0, 0, _texture.Width, _texture.Height), Color, Rotation, Origin, Scale, _effects, ZOrder);
                else
                    spritebatch.Draw(Texture, Rectangle, new Rectangle(0, 0, _texture.Width, _texture.Height), Color, Rotation, Origin, _effects, ZOrder);
            }
        }

        /// <summary>
        /// Vérifie si le sprite entre en collision avec un autre objet ICollide.
        /// </summary>
        /// <param name="item">L'objet avec lequel vérifier la collision.</param>
        /// <returns>True si une collision est détectée, sinon false.</returns>
        public bool Collide(ICollide item)
        {
            if (item == null)
                return false;
            return Rectangle.Intersects(item.Rectangle);
        }

        /// <summary>
        /// Crée une copie du sprite actuel.
        /// </summary>
        /// <returns>Une nouvelle instance de Sprite avec les mêmes propriétés.</returns>
        public Sprite Copy()
        {
            return new Sprite(Texture, Color, Position)
            {
                Dimensions = Dimensions,
                Flip = Flip,
                Origin = Origin,
                Rectangle = Rectangle,
                Rotation = Rotation,
                Scale = Scale,
                Visible = Visible,
                ZOrder = ZOrder,
                _color = _color,
                _effects = _effects,
                _origin = _origin,
                _flip = _flip,
                _scale = _scale,
                _rectangle = _rectangle,
                _texture = _texture,
                _visible = _visible,
            };
        }
    }
}
