using DinaFramework.Core.Fixed;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Reflection.Metadata.Ecma335;

namespace DinaFramework.Core.Animated
{
    public class Animation : Base, IUpdate, IDraw, IColor, ICollide
    {
        private readonly List<Sprite> _frames = new List<Sprite>();
        private readonly float _speed;
        private float _currentframe;
        private float _rotation;
        private Color _color;
        private Vector2 _origin;
        private Vector2 _flip;
        private Rectangle _rect;
        private readonly int _nbRepetitions = -1;
        private int _currentrepetition = -1;
        public Animation(ContentManager content, string prefix, int nbframes, float speed, int start, int nbRepetitions, Color color,
                         Vector2 position, Vector2 dimensions, float rotation = default, Vector2 origin = default,
                         Vector2 flip = default, int zorder = default) : base(position, dimensions, zorder)
        {
            ArgumentNullException.ThrowIfNull(content);

            _speed = speed;
            _nbRepetitions = nbRepetitions;
            _currentrepetition = nbRepetitions;
            Color = color == default ? Color.White : color;
            Rotation = 0.0f;
            Origin = origin;
            Flip = flip == default ? Vector2.One : flip;
            Scale = Vector2.One;
            AddFrames(content, prefix, nbframes, start, Dimensions, rotation, Origin);
        }
        public Animation(ContentManager content, string prefix, int nbframes, float speed, int start, int nbRepetitions, Color color,
                         Vector2 position, float rotation, Vector2 origin = default, Vector2 scale = default,
                         Vector2 flip = default, int zorder = default) : base(position, default, zorder)
        {
            ArgumentNullException.ThrowIfNull(content);

            _speed = speed;
            _nbRepetitions = nbRepetitions;
            _currentrepetition = nbRepetitions;
            Color = color == default ? Color.White : color;
            Rotation = rotation;
            Origin = origin;
            Scale = scale == default ? Vector2.One : scale;
            Flip = flip == default ? Vector2.One : flip;
            AddFrames(content, prefix, nbframes, start, rotation, Origin, Scale);
        }
        public Animation(Animation animation, bool duplicate = true)
        {
            ArgumentNullException.ThrowIfNull(animation);

            _frames = new List<Sprite>();
            foreach (Sprite item in animation._frames)
            {
                if (duplicate)
                    _frames.Add((Sprite)Activator.CreateInstance(typeof(Sprite), item));
                else
                    _frames.Add(item);
            }
            _speed = animation._speed;
            _nbRepetitions = animation._nbRepetitions;
            _currentrepetition = animation._nbRepetitions;
            Color = animation.Color;
            Rotation = animation.Rotation;
            Origin = animation.Origin;
            Scale = animation.Scale;
            Flip = animation.Flip;
            _currentframe = 0;
            Position = animation.Position;
            Dimensions = animation.Dimensions;
            ZOrder = animation.ZOrder;
        }
        private void AddFrames(ContentManager content, string prefix, int nbframes, int start, Vector2 dimensions, float rotation, Vector2 origin)
        {
            ArgumentNullException.ThrowIfNull(content);

            Texture2D texture;
            for (int index = start; index < nbframes + start; index++)
            {
                string strIndex = index.ToString();
                texture = content.Load<Texture2D>(prefix + strIndex);
                if (dimensions == Vector2.Zero)
                {
                    dimensions = new Vector2(texture.Width, texture.Height);
                }
                _frames.Add(new Sprite(texture, Color, Position, dimensions, rotation, origin, Flip, ZOrder));
            }
            Dimensions = dimensions;
        }
        private void AddFrames(ContentManager content, string prefix, int nbframes, int start, float rotation, Vector2 origin, Vector2 scale)
        {
            Texture2D texture;
            for (int index = start; index < nbframes + start; index++)
            {
                texture = content.Load<Texture2D>(prefix + index.ToString());
                //if (Dimensions == default)
                //    Dimensions = new Vector2(texture.Width, texture.Height) * scale;
                _frames.Add(new Sprite(texture, Color, Position, rotation, origin, scale, Flip, ZOrder));
            }
        }
        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                foreach (Sprite frame in _frames)
                    frame.Position = value + Origin;
                base.Position = value;
                _rect.Location = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        public override Vector2 Dimensions
        {
            get { return base.Dimensions; }
            set
            {
                foreach (Sprite frame in _frames)
                    frame.Dimensions = value;
                base.Dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        public Vector2 Origin
        {
            get { return _origin; }
            set
            {
                foreach (Sprite frame in _frames)
                    frame.Origin = value;
                _origin = value;
            }
        }
        public void CenterOrigin()
        {
            foreach (Sprite frame in _frames)
                frame.CenterOrigin();
        }
        public float Rotation
        {
            get { return _rotation; }
            set
            {
                foreach (Sprite frame in _frames)
                    frame.Rotation = value;
                _rotation = value;
            }
        }
        public Vector2 Scale { get; set; }
        public Vector2 Flip
        {
            get { return _flip; }
            set
            {
                foreach (Sprite frame in _frames)
                    frame.Flip = value;
                if (value.X != 0)
                    value.X /= Math.Abs(value.X);
                if (value.Y != 0)
                    value.Y /= Math.Abs(value.Y);
                _flip = value;
            }
        }
        public Rectangle Rectangle { get { return _rect; } }

        public void Update(GameTime gameTime)
        {
            if (_currentrepetition != 0)
            {
                ArgumentNullException.ThrowIfNull(gameTime);

                _currentframe += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds) * _speed;
                if (_currentframe >= _frames.Count)
                {
                    if (_currentrepetition < 0)
                        _currentframe = 0;
                    else
                    {
                        _currentrepetition--;
                        if (_currentrepetition > 0)
                            _currentframe = 0;
                    }
                }
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (_currentrepetition != 0)
                _frames[(int)_currentframe].Draw(spritebatch);
        }
        public bool Collide(ICollide item)
        {
            if ( (item == null))
                return false;

            return Rectangle.Intersects(item.Rectangle);
        }
        public bool IsFinished() => _currentrepetition == 0;
        public void Reset()
        {
            _currentrepetition = _nbRepetitions;
            _currentframe = 0;
        }
    }
}
