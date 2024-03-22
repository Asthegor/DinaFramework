﻿using DinaFramework.Core.Fixed;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace DinaFramework.Core.Animated
{
    public class Animation : Base, IReset, IUpdate, IDraw, IColor, ICollide, IVisible, ICopyable<Animation>
    {
        private List<Sprite> _frames = new List<Sprite>();
        private float _speed;
        private float _currentframe;
        private float _rotation;
        private Color _color;
        private Vector2 _origin;
        private Vector2 _flip;
        private Rectangle _rect;
        private int _nbRepetitions = -1;
        private int _currentrepetition = -1;
        private Vector2 _scale;
        private bool _visible;
        private Rectangle[] _sourceRectangles; // Les rectangles délimitant chaque frame dans l'image principale

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
            Visible = true;
            AddFrames(content, prefix, nbframes, start, Dimensions, rotation, Origin);
        }
        public Animation(ContentManager content, string prefix, int nbframes, float speed, int start, int nbRepetitions, Color color,
                         Vector2 position, float rotation, Vector2 origin = default, Vector2 scale = default,
                         Vector2 flip = default, int zorder = default) : base(position, default, zorder)
        {
            ArgumentNullException.ThrowIfNull(content);

            Origin = origin;
            Scale = scale == default ? Vector2.One : scale;
            AddFrames(content, prefix, nbframes, start, rotation, Origin, Scale);
            _speed = speed;
            _nbRepetitions = nbRepetitions;
            _currentrepetition = nbRepetitions;
            Color = color == default ? Color.White : color;
            Rotation = rotation;
            Flip = flip == default ? Vector2.One : flip;
            Visible = true;
        }
        public Animation(ContentManager content, string[] frameNames, float speed, int startframe, int nbRepetitions, Color color,
                         Vector2 position, float rotation, Vector2 origin = default, Vector2 scale = default,
                         Vector2 flip = default, int zorder = default) : base(position, default, zorder)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(frameNames);

            Flip = flip == default ? Vector2.One : flip;
            foreach (string name in frameNames)
            {
                Texture2D texture = content.Load<Texture2D>(name);
                //_frames.Add(new Sprite(texture, color, Position, rotation, origin, scale, Flip, ZOrder));
                _frames.Add(new Sprite(texture, color, Position, null, origin, Flip, rotation, scale, SpriteEffects.None, ZOrder));
            }
            _speed = speed;
            _nbRepetitions = nbRepetitions;
            _currentrepetition = nbRepetitions;
            Color = color == default ? Color.White : color;
            Rotation = rotation;
            Origin = origin;
            Scale = scale == default ? Vector2.One : scale;
            Visible = true;
            Dimensions = _frames.Count > 0 ? _frames[0].Dimensions : Vector2.Zero;
            _currentframe = startframe >= 0 && startframe <= _frames.Count ? startframe : 0;
        }
        public Animation(ContentManager content, string[] frameNames, float speed, int startframe, int nbRepetitions, Color color,
                         Vector2 position, Vector2 dimensions, float rotation = default, Vector2 origin = default, Vector2 scale = default,
                         Vector2 flip = default, int zorder = default) : base(position, dimensions, zorder)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(frameNames);

            Color = color == default ? Color.White : color;
            Flip = flip == default ? Vector2.One : flip;
            foreach (string name in frameNames)
            {
                Texture2D texture = content.Load<Texture2D>(name);
                if (dimensions == Vector2.Zero)
                {
                    dimensions = new Vector2(texture.Width, texture.Height);
                }
                //_frames.Add(new Sprite(texture, Color, Position, dimensions, rotation, origin, Flip, ZOrder));
                _frames.Add(new Sprite(texture, Color, Position, new Rectangle(Point.Zero, dimensions.ToPoint()), origin, Flip, rotation, Vector2.One, SpriteEffects.None, ZOrder));
            }
            _speed = speed;
            _nbRepetitions = nbRepetitions;
            _currentrepetition = nbRepetitions;
            Rotation = rotation;
            Origin = origin;
            Scale = scale == default ? Vector2.One : scale;
            Visible = true;

            _currentframe = startframe >= 0 && startframe <= _frames.Count ? startframe : 0;

            Dimensions = dimensions;
        }
        public Animation(ContentManager content, Texture2D spritesheet, int frameWidth, int frameHeight, int frameCount, float speed, int startframe, int nbRepetitions,
                         Color color, Vector2 position, Vector2 dimensions, float rotation = 0, Vector2 origin = default, Vector2 scale = default,
                         Vector2 flip = default, int zorder = default) : base(position, dimensions, zorder)
        {
            ArgumentNullException.ThrowIfNull(content);
            ArgumentNullException.ThrowIfNull(spritesheet);

            //_speed = speed;
            //_nbRepetitions = nbRepetitions;
            //_currentrepetition = nbRepetitions;
            //Color = color == default ? Color.White : color;
            //Rotation = rotation;
            //Origin = origin;
            //Scale = scale == default ? Vector2.One : scale;
            //Flip = flip == default ? Vector2.One : flip;
            //Visible = true;
            ArgumentNullException.ThrowIfNull(spritesheet);
            if (frameCount <= 0)
                throw new ArgumentException("frameCount must be greater than 0");
            if (frameWidth <= 0 || frameHeight <= 0)
                throw new ArgumentException("frameWidth and frameHeight must be greater than 0");

            _speed = speed;
            _nbRepetitions = nbRepetitions;
            _currentrepetition = nbRepetitions;
            _color = color;
            _origin = origin;
            _flip = flip == default ? Vector2.One : flip;
            _scale = Vector2.One;
            Visible = true;
            _currentframe = 0;

            // Calcule le nombre de frames dans l'image principale
            int framesPerRow = spritesheet.Width / frameWidth;
            int frameRows = spritesheet.Height / frameHeight;
            int totalFrames = framesPerRow * frameRows;

            if (frameCount > totalFrames)
                throw new ArgumentException("frameCount exceeds the number of frames in the sprite sheet");

            // Calcule les rectangles source pour chaque frame
            _sourceRectangles = new Rectangle[frameCount];
            int frameIndex = 0;
            for (int row = 0; row < frameRows; row++)
            {
                for (int col = 0; col < framesPerRow; col++)
                {
                    if (frameIndex >= frameCount)
                        break;

                    _sourceRectangles[frameIndex] = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);

                    Rectangle sourceRect = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
                    //_frames[frameIndex] = new Sprite(spritesheet, color, position, dimensions, rotation, origin, flip, sourceRect, zorder);
                    //Sprite s = new Sprite(spritesheet, Color.White, position, _sourceRectangles[frameIndex], Vector2.Zero, Vector2.One, 0, Vector2.One, SpriteEffects.None, zorder);
                    //_frames.Add(s);
                    _frames.Add(new Sprite(spritesheet, color, position, sourceRect, origin, flip, rotation, Vector2.One, SpriteEffects.None, zorder));

                    frameIndex++;
                }
            }
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
                //_frames.Add(new Sprite(texture, Color, Position, dimensions, rotation, origin, Flip, ZOrder));
                _frames.Add(new Sprite(texture, Color, Position, new Rectangle(Point.Zero, dimensions.ToPoint()), origin, Flip, rotation, Vector2.One, SpriteEffects.None, ZOrder));
            }
            Dimensions = dimensions;
        }
        private void AddFrames(ContentManager content, string prefix, int nbframes, int start, float rotation, Vector2 origin, Vector2 scale)
        {
            Texture2D texture;
            for (int index = start; index < nbframes + start; index++)
            {
                if (ImageExists(content, prefix + index.ToString("00")))
                    texture = content.Load<Texture2D>(prefix + index.ToString("00"));
                else
                    texture = content.Load<Texture2D>(prefix + index.ToString());
                //_frames.Add(new Sprite(texture, Color, Position, rotation, origin, scale, Flip, ZOrder));
                _frames.Add(new Sprite(texture, Color, Position, new Rectangle(0, 0, texture.Width, texture.Height), origin, Flip, rotation, scale, SpriteEffects.None, ZOrder));
            }
        }
        private bool ImageExists(ContentManager content, string imageName)
        {
            try
            {
                content.Load<Texture2D>(imageName);
                return true; // L'image existe
            }
            catch (ContentLoadException)
            {
                return false; // L'image n'existe pas
            }
        }
        public float Speed { get => _speed; set => _speed = value; }
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
        public Vector2 Scale
        {
            get => _scale;
            set
            {
                _scale = value;
                foreach (Sprite frame in _frames)
                    frame.Scale = value;
            }
        }
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

        public bool Visible { get => _visible; set => _visible = value; }

        public void Update(GameTime gameTime)
        {
            if (Visible && _currentrepetition != 0)
            {
                ArgumentNullException.ThrowIfNull(gameTime);

                _currentframe += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds) * _speed;
                if (_currentframe >= _frames.Count)
                {
                    _currentframe = 0;
                    if (_currentrepetition > 0)
                        _currentrepetition--;
                    if (_currentrepetition == 0)
                        Visible = false;
                }
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (Visible && _currentrepetition != 0)
                _frames[(int)_currentframe].Draw(spritebatch);
        }
        public bool Collide(ICollide item)
        {
            if ((item == null))
                return false;

            return Rectangle.Intersects(item.Rectangle);
        }
        public bool IsFinished() => _currentrepetition == 0;
        public void Reset()
        {
            _currentrepetition = _nbRepetitions;
            _currentframe = 0;
        }
        public Animation Copy()
        {
            List<Sprite> copiedFrames = new List<Sprite>();
            foreach (Sprite sprite in _frames)
                copiedFrames.Add(sprite.Copy());
            Rectangle[] copiedRectangles = new Rectangle[_sourceRectangles.Length];
            Array.Copy(_sourceRectangles, copiedRectangles, _sourceRectangles.Length);
            return new Animation()
            {
                _color = this._color,
                _currentframe = this._currentframe,
                _currentrepetition = this._currentrepetition,
                _frames = copiedFrames,
                _flip = this._flip,
                _origin = this._origin,
                _rect = this._rect,
                _rotation = this._rotation,
                _scale = this._scale,
                _sourceRectangles = copiedRectangles,
                _speed = this._speed,
                _visible = this._visible,
                Color = this.Color,
                Dimensions = this.Dimensions,
                Flip = this.Flip,
                Origin = this.Origin,
                Position = this.Position,
                Rotation = this.Rotation,
                Scale = this.Scale,
                Speed = this.Speed,
                Visible = this.Visible,
                ZOrder = this.ZOrder,
            };
        }
        private Animation() { }
    }
}
