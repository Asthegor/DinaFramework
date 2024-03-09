using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Core.Fixed
{
    public class Sprite : Base, IColor, IVisible, IDraw, ICollide
    {
        private Rectangle _rectangle;
        private Color _color;
        private Vector2 _origin;
        private bool _visible;
        private Texture2D _texture;
        private SpriteEffects _effects;
        private Vector2 _flip;
        private Vector2 _scale;

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
        public Texture2D Texture
        {
            get { return _texture; }
            set { _texture = value; }
        }
        public Rectangle Rectangle
        {
            get { return _rectangle; }
            private set { _rectangle = value; }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
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
        public Vector2 Origin
        {
            get { return _origin; }
            set { _origin = value; }
        }
        public void CenterOrigin() { _origin = new Vector2(Texture.Width, Texture.Height) / 2.0f; }
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public void SetVisible(bool value = false)
        {
            _visible = value;
        }
        public float Rotation { get; set; }
        public Vector2 Scale { get => _scale;
            set
            {
                _scale = value;
                _rectangle.Size = new Vector2(Dimensions.X * _scale.X, Dimensions.Y * _scale.Y).ToPoint();
            }
        }
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

        public bool Collide(ICollide item)
        {
            if (item == null)
                return false;
            return Rectangle.Intersects(item.Rectangle);
        }
    }
}
