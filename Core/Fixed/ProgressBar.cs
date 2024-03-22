using DinaFramework.Enums;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace DinaFramework.Core.Fixed
{
    public class ProgressBar : Base, IDraw, IUpdate, IVisible, ICopyable<ProgressBar>
    {
        public enum PBType { Color, Image2Parts, Image3Parts }
        private PBType _pbtype;
        private bool _visible;
        private float _value;
        private float _maxValue;
        private int _borderThickness;
        private int _imgOffset;
        private Color _frontColor;
        private Color _backColor;
        private Color _borderColor;
        private Rectangle[] _rectangles = new Rectangle[3];
        private Rectangle[] _rectanglesSource = new Rectangle[3];
        private ProgressBarMode _mode;
        private List<Texture2D> _textures = new List<Texture2D>();
        private float _timer;
        private float _delay;
        private float _increment;
        private bool _autoIncrement;

        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Color frontColor, Color borderColor, Color backColor, int borderThickness = 1, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            MaxValue = maxValue;
            Value = value;
            _rectangles[0] = new Rectangle(position.ToPoint(), dimensions.ToPoint()); // Border
            SetColors(frontColor, borderColor, backColor, borderThickness);
        }
        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Texture2D backImage, Texture2D frontImage, int offset = 0, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            MaxValue = maxValue;
            Value = value;
            _imgOffset = offset;
            _rectangles[0] = new Rectangle(position.ToPoint(), dimensions.ToPoint());
            SetImages(backImage, frontImage, _imgOffset);
        }
        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Texture2D leftImage, Texture2D centerImage, Texture2D rightImage, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            MaxValue = maxValue;
            Value = value;
            _rectangles[1] = new Rectangle(position.ToPoint(), dimensions.ToPoint());
            SetImages(leftImage, centerImage, rightImage);
        }
        public ProgressBar(float value, float maxValue, Texture2D leftImage, Texture2D centerImage, Texture2D rightImage, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0)
        {
            Debug.Assert(leftImage != null, "'leftImage' could not be null.");
            Debug.Assert(centerImage != null, "'centerImage' could not be null.");
            Debug.Assert(rightImage != null, "'rightImage' could not be null.");

            Visible = true;
            Mode = mode;
            MaxValue = maxValue;
            Value = value;
            Position = Vector2.Zero;
            Dimensions = new Vector2(leftImage.Width + rightImage.Width + centerImage.Width, leftImage.Height);
            ZOrder = zorder;

            _rectangles[1] = new Rectangle(Vector2.Zero.ToPoint(), Dimensions.ToPoint());
            SetImages(leftImage, centerImage, rightImage);
        }

        public bool AutoIncrement { get => _autoIncrement; set => _autoIncrement = value; }
        public float MaxValue { get => _maxValue; set => _maxValue = value; }
        public ProgressBarMode Mode { get => _mode; set => _mode = value; }
        public void ProgressInterval(float delay, float increment = 1)
        {
            if (delay > 0 && increment != 0)
            {
                _delay = delay;
                _increment = increment;
                AutoIncrement = true;
            }
        }
        public float Value
        {
            get => _value;
            set
            {
                if (value <= 0.0f)
                    _value = 0.0f;
                else if (value > MaxValue)
                    _value = MaxValue;
                else
                    _value = value;
                switch (_pbtype)
                {
                    case PBType.Color:
                        UpdateColorRectangle();
                        break;
                    case PBType.Image2Parts:
                        Update2ImagesRectangle(_imgOffset);
                        break;
                    case PBType.Image3Parts:
                        Update3ImagesRectangle();
                        break;
                }

            }
        }
        public bool Visible { get => _visible; set => _visible = value; }
        public void SetImages(Texture2D backImage, Texture2D frontImage, int offset = 0)
        {
            _pbtype = PBType.Image2Parts;
            if (_textures.Count == 0) _textures.Add(backImage); else _textures[0] = backImage;
            if (_textures.Count == 1) _textures.Add(frontImage); else _textures[1] = frontImage;
            Update2ImagesRectangle(offset);
        }
        public void SetImages(Texture2D leftImage, Texture2D centerImage, Texture2D rightImage)
        {
            _pbtype = PBType.Image3Parts;
            if (_textures.Count == 0) _textures.Add(leftImage); else _textures[0] = leftImage;
            if (_textures.Count == 1) _textures.Add(centerImage); else _textures[1] = centerImage;
            if (_textures.Count == 2) _textures.Add(rightImage); else _textures[2] = rightImage;
            Update3ImagesRectangle();
        }
        public void SetColors(Color frontColor, Color borderColor, Color backColor, int borderThickness = 1)
        {
            _pbtype = PBType.Color;
            _frontColor = frontColor;
            _backColor = backColor;
            _borderColor = borderColor;
            _borderThickness = borderThickness;
            Vector2 pos = Position;
            Vector2 backDimensions = Dimensions;
            if (_borderThickness > 0)
            {
                pos = new Vector2(Position.X + _borderThickness, Position.Y + _borderThickness);
                backDimensions = new Vector2(Dimensions.X - _borderThickness * 2.0f, Dimensions.Y - _borderThickness * 2.0f);
            }
            _rectangles[1] = new Rectangle(pos.ToPoint(), backDimensions.ToPoint()); // Back
            UpdateColorRectangle();
        }
        private void UpdateColorRectangle()
        {
            float ratio = Value / MaxValue;
            float posX;
            float posY;
            float width;
            float height;
            switch (Mode)
            {
                case ProgressBarMode.LeftToRight:
                    posX = _rectangles[1].X;
                    posY = _rectangles[1].Y;
                    width = _rectangles[1].Width * ratio;
                    height = _rectangles[1].Height;
                    break;
                case ProgressBarMode.RightToLeft:
                    posX = _rectangles[1].X + _rectangles[1].Width - _rectangles[1].Width * ratio;
                    posY = _rectangles[1].Y;
                    width = _rectangles[1].Width * ratio;
                    height = _rectangles[1].Height;
                    break;
                case ProgressBarMode.TopToBottom:
                    posX = _rectangles[1].X;
                    posY = _rectangles[1].Y;
                    width = _rectangles[1].Width;
                    height = _rectangles[1].Height * ratio;
                    break;
                case ProgressBarMode.BottomToTop:
                    posX = _rectangles[1].X;
                    posY = _rectangles[1].Y + _rectangles[1].Height - _rectangles[1].Height * ratio;
                    width = _rectangles[1].Width;
                    height = _rectangles[1].Height * ratio;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown mode.");
            }
            _rectangles[2] = new Rectangle(Convert.ToInt32(posX), Convert.ToInt32(posY), Convert.ToInt32(width), Convert.ToInt32(height));
        }
        private void Update2ImagesRectangle(int offset = 0)
        {
            float ratio = Value / MaxValue;
            float posX;
            float posY;
            float width;
            float height;
            switch (Mode)
            {
                case ProgressBarMode.LeftToRight:
                    width = (_rectangles[0].Width - offset * 2.0f) * ratio;
                    height = _rectangles[0].Height - offset * 2.0f;
                    posX = _rectangles[0].X + offset;
                    posY = _rectangles[0].Y + offset;
                    break;
                case ProgressBarMode.RightToLeft:
                    width = (_rectangles[0].Width - offset * 2.0f) * ratio;
                    height = _rectangles[0].Height - offset * 2.0f;
                    posX = _rectangles[0].X + _rectangles[0].Width - width - offset;
                    posY = _rectangles[0].Y + offset;
                    break;
                case ProgressBarMode.TopToBottom:
                    width = _rectangles[0].Width - offset * 2.0f;
                    height = (_rectangles[0].Height - offset * 2.0f) * ratio;
                    posX = _rectangles[0].X + offset;
                    posY = _rectangles[0].Y + offset;
                    break;
                case ProgressBarMode.BottomToTop:
                    width = _rectangles[0].Width - offset * 2.0f;
                    height = (_rectangles[0].Height - offset * 2.0f) * ratio;
                    posX = _rectangles[0].X + offset;
                    posY = _rectangles[0].Y + _rectangles[0].Height - height - offset;
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown mode.");
            }
            _rectangles[1] = new Rectangle(Convert.ToInt32(posX), Convert.ToInt32(posY), Convert.ToInt32(width), Convert.ToInt32(height));
        }
        private void Update3ImagesRectangle()
        {
            float ratio = Value / MaxValue;
            switch (Mode)
            {
                case ProgressBarMode.LeftToRight:
                    {
                        float width = ratio * Dimensions.X;
                        float ratioheight = Dimensions.Y / _textures[0].Height;
                        float maxleftwidth = _textures[0].Width * ratioheight;
                        float maxrightwidth = _textures[2].Width * ratioheight;
                        float maxmidwidth = Dimensions.X - maxleftwidth - maxrightwidth;
                        // Largeur
                        if (width <= maxleftwidth)
                        {
                            _rectangles[0].Width = Convert.ToInt32(width);
                            _rectangles[1].Width = 0;
                            _rectangles[2].Width = 0;
                            _rectanglesSource[0].Width = Convert.ToInt32(width / ratioheight);
                            _rectanglesSource[1].Width = 0;
                            _rectanglesSource[2].Width = 0;
                        }
                        else if (width <= Dimensions.X - maxrightwidth)
                        {
                            float ratiomid = (width - maxleftwidth) / maxmidwidth;
                            _rectangles[0].Width = Convert.ToInt32(maxleftwidth);
                            _rectangles[1].Width = Convert.ToInt32(width - maxleftwidth);
                            _rectangles[2].Width = 0;
                            _rectanglesSource[0].Width = _textures[0].Width;
                            _rectanglesSource[1].Width = Convert.ToInt32(_textures[1].Width * ratiomid);
                            _rectanglesSource[2].Width = 0;
                        }
                        else
                        {
                            float ratioright = (width - maxleftwidth - maxmidwidth) / maxrightwidth;
                            _rectangles[0].Width = Convert.ToInt32(maxleftwidth);
                            _rectangles[1].Width = Convert.ToInt32(Dimensions.X - maxleftwidth - maxrightwidth);
                            _rectangles[2].Width = Convert.ToInt32(width - _rectangles[0].Width - _rectangles[1].Width);
                            _rectanglesSource[0].Width = _textures[0].Width;
                            _rectanglesSource[1].Width = _textures[1].Width;
                            _rectanglesSource[2].Width = Convert.ToInt32(_textures[2].Width * ratioright);
                        }
                        // Hauteur
                        _rectangles[0].Height = Convert.ToInt32(Dimensions.Y);
                        _rectangles[1].Height = Convert.ToInt32(Dimensions.Y);
                        _rectangles[2].Height = Convert.ToInt32(Dimensions.Y);
                        _rectanglesSource[0].Height = _textures[0].Height;
                        _rectanglesSource[1].Height = _textures[1].Height;
                        _rectanglesSource[2].Height = _textures[2].Height;
                        // Position X
                        _rectangles[0].X = Convert.ToInt32(Position.X);
                        _rectangles[1].X = Convert.ToInt32(Position.X + _rectangles[0].Width);
                        _rectangles[2].X = Convert.ToInt32(Position.X + _rectangles[0].Width + _rectangles[1].Width);
                        _rectanglesSource[0].X = 0;
                        _rectanglesSource[1].X = 0;
                        _rectanglesSource[2].X = 0;
                        // Position Y
                        _rectangles[0].Y = Convert.ToInt32(Position.Y);
                        _rectangles[1].Y = Convert.ToInt32(Position.Y);
                        _rectangles[2].Y = Convert.ToInt32(Position.Y);
                        _rectanglesSource[0].Y = 0;
                        _rectanglesSource[1].Y = 0;
                        _rectanglesSource[2].Y = 0;
                    }
                    break;
                case ProgressBarMode.RightToLeft:
                    {
                        float width = ratio * Dimensions.X;
                        float ratioheight = _textures[0].Height / Dimensions.Y;
                        float maxleftwidth = _textures[0].Width * ratioheight;
                        float maxrightwidth = _textures[2].Width * ratioheight;
                        float maxmidwidth = Dimensions.X - maxleftwidth - maxrightwidth;
                        // Largeur
                        if (width <= maxrightwidth)
                        {
                            _rectangles[0].Width = 0;
                            _rectangles[1].Width = 0;
                            _rectangles[2].Width = Convert.ToInt32(width);
                            _rectanglesSource[0].Width = 0;
                            _rectanglesSource[1].Width = 0;
                            _rectanglesSource[2].Width = Convert.ToInt32(width);
                            _rectanglesSource[0].X = _textures[0].Width;
                            _rectanglesSource[1].X = _textures[1].Width;
                            _rectanglesSource[2].X = Convert.ToInt32(_textures[2].Width - width);

                        }
                        else if (width <= Dimensions.X - maxleftwidth)
                        {
                            float ratiomid = (width - maxrightwidth) / maxmidwidth;
                            _rectangles[0].Width = 0;
                            _rectangles[1].Width = Convert.ToInt32(width - maxleftwidth);
                            _rectangles[2].Width = Convert.ToInt32(maxrightwidth);
                            _rectanglesSource[0].Width = 0;
                            _rectanglesSource[1].Width = Convert.ToInt32(_textures[1].Width * ratiomid);
                            _rectanglesSource[2].Width = _textures[2].Width;
                            _rectanglesSource[0].X = _textures[0].Width;
                            _rectanglesSource[1].X = Convert.ToInt32(_textures[1].Width - _rectanglesSource[1].Width);
                            _rectanglesSource[2].X = 0;
                        }
                        else
                        {
                            _rectangles[0].Width = Convert.ToInt32(width - _rectangles[2].Width - _rectangles[1].Width);
                            _rectangles[1].Width = Convert.ToInt32(Dimensions.X - maxleftwidth - maxrightwidth);
                            _rectangles[2].Width = Convert.ToInt32(maxrightwidth);
                            _rectanglesSource[0].Width = _rectangles[0].Width;
                            _rectanglesSource[1].Width = _textures[1].Width;
                            _rectanglesSource[2].Width = _textures[2].Width;
                            _rectanglesSource[0].X = Convert.ToInt32(_textures[0].Width - _rectanglesSource[0].Width);
                            _rectanglesSource[1].X = 0;
                            _rectanglesSource[2].X = 0;
                        }
                        // Hauteur
                        _rectangles[0].Height = Convert.ToInt32(Dimensions.Y);
                        _rectangles[1].Height = Convert.ToInt32(Dimensions.Y);
                        _rectangles[2].Height = Convert.ToInt32(Dimensions.Y);
                        _rectanglesSource[0].Height = _textures[0].Height;
                        _rectanglesSource[1].Height = _textures[1].Height;
                        _rectanglesSource[2].Height = _textures[2].Height;
                        // Position X
                        _rectangles[0].X = Convert.ToInt32(Position.X + Dimensions.X - _rectangles[2].Width - _rectangles[1].Width - _rectangles[0].Width);
                        _rectangles[1].X = Convert.ToInt32(Position.X + Dimensions.X - _rectangles[2].Width - _rectangles[1].Width);
                        _rectangles[2].X = Convert.ToInt32(Position.X + Dimensions.X - _rectangles[2].Width);
                        // Position Y
                        _rectangles[0].Y = Convert.ToInt32(Position.Y);
                        _rectangles[1].Y = Convert.ToInt32(Position.Y);
                        _rectangles[2].Y = Convert.ToInt32(Position.Y);
                        _rectanglesSource[0].Y = 0;
                        _rectanglesSource[1].Y = 0;
                        _rectanglesSource[2].Y = 0;
                    }
                    break;
                case ProgressBarMode.TopToBottom:
                    {
                        float height = ratio * Dimensions.Y;
                        float ratiowidth = Dimensions.X / _textures[0].Width;
                        float maxtopheight = _textures[0].Height * ratiowidth;
                        float maxbottomheight = _textures[2].Height * ratiowidth;
                        float maxmidheight = Dimensions.Y - maxtopheight - maxbottomheight;
                        // Hauteur
                        if (height <= maxtopheight)
                        {
                            _rectangles[0].Height = Convert.ToInt32(height);
                            _rectangles[1].Height = 0;
                            _rectangles[2].Height = 0;
                            _rectanglesSource[0].Height = Convert.ToInt32(height / ratiowidth);
                            _rectanglesSource[1].Height = 0;
                            _rectanglesSource[2].Height = 0;
                        }
                        else if (height <= Dimensions.Y - maxbottomheight)
                        {
                            float ratiomid = (height - maxtopheight) / maxmidheight;
                            _rectangles[0].Height = Convert.ToInt32(maxtopheight);
                            _rectangles[1].Height = Convert.ToInt32(height - maxtopheight);
                            _rectangles[2].Height = 0;
                            _rectanglesSource[0].Height = _textures[0].Height;
                            _rectanglesSource[1].Height = Convert.ToInt32(_textures[1].Height * ratiomid);
                            _rectanglesSource[2].Height = 0;
                        }
                        else
                        {
                            float ratiobottom = (height - maxtopheight - maxmidheight) / maxbottomheight;
                            _rectangles[0].Height = Convert.ToInt32(maxtopheight);
                            _rectangles[1].Height = Convert.ToInt32(Dimensions.Y - maxtopheight - maxbottomheight);
                            _rectangles[2].Height = Convert.ToInt32(height - _rectangles[0].Height - _rectangles[1].Height);
                            _rectanglesSource[0].Height = _textures[0].Height;
                            _rectanglesSource[1].Height = _textures[1].Height;
                            _rectanglesSource[2].Height = Convert.ToInt32(_textures[2].Height * ratiobottom);
                        }
                        // Largeur
                        _rectangles[0].Width = Convert.ToInt32(Dimensions.X);
                        _rectangles[1].Width = Convert.ToInt32(Dimensions.X);
                        _rectangles[2].Width = Convert.ToInt32(Dimensions.X);
                        _rectanglesSource[0].Width = _textures[0].Width;
                        _rectanglesSource[1].Width = _textures[1].Width;
                        _rectanglesSource[2].Width = _textures[2].Width;
                        // Position Y
                        _rectangles[0].Y = Convert.ToInt32(Position.Y);
                        _rectangles[1].Y = Convert.ToInt32(Position.Y + _rectangles[0].Height);
                        _rectangles[2].Y = Convert.ToInt32(Position.Y + _rectangles[0].Height + _rectangles[1].Height);
                        _rectanglesSource[0].Y = 0;
                        _rectanglesSource[1].Y = 0;
                        _rectanglesSource[2].Y = 0;
                        // Position X
                        _rectangles[0].X = Convert.ToInt32(Position.X);
                        _rectangles[1].X = Convert.ToInt32(Position.X);
                        _rectangles[2].X = Convert.ToInt32(Position.X);
                        _rectanglesSource[0].X = 0;
                        _rectanglesSource[1].X = 0;
                        _rectanglesSource[2].X = 0;
                    }
                    break;
                case ProgressBarMode.BottomToTop:
                    {
                        float height = ratio * Dimensions.Y;
                        float ratiowidth = _textures[0].Width / Dimensions.X;
                        float maxtopheight = _textures[0].Height * ratiowidth;
                        float maxbottomheight = _textures[2].Height * ratiowidth;
                        float maxmidheight = Dimensions.Y - maxtopheight - maxbottomheight;
                        // Hauteur
                        if (height <= maxbottomheight)
                        {
                            _rectangles[0].Height = 0;
                            _rectangles[1].Height = 0;
                            _rectangles[2].Height = Convert.ToInt32(height);
                            _rectanglesSource[0].Height = 0;
                            _rectanglesSource[1].Height = 0;
                            _rectanglesSource[2].Height = Convert.ToInt32(height);
                            _rectanglesSource[0].Y = _textures[0].Height;
                            _rectanglesSource[1].Y = _textures[1].Height;
                            _rectanglesSource[2].Y = Convert.ToInt32(_textures[2].Height - height);

                        }
                        else if (height <= Dimensions.Y - maxtopheight)
                        {
                            float ratiomid = (height - maxbottomheight) / maxmidheight;
                            _rectangles[0].Height = 0;
                            _rectangles[1].Height = Convert.ToInt32(height - maxtopheight);
                            _rectangles[2].Height = Convert.ToInt32(maxbottomheight);
                            _rectanglesSource[0].Height = 0;
                            _rectanglesSource[1].Height = Convert.ToInt32(_textures[1].Height * ratiomid);
                            _rectanglesSource[2].Height = _textures[2].Height;
                            _rectanglesSource[0].Y = _textures[0].Height;
                            _rectanglesSource[1].Y = Convert.ToInt32(_textures[1].Height - _rectanglesSource[1].Height);
                            _rectanglesSource[2].Y = 0;
                        }
                        else
                        {
                            _rectangles[0].Height = Convert.ToInt32(height - _rectangles[2].Height - _rectangles[1].Height);
                            _rectangles[1].Height = Convert.ToInt32(Dimensions.Y - maxtopheight - maxbottomheight);
                            _rectangles[2].Height = Convert.ToInt32(maxbottomheight);
                            _rectanglesSource[0].Height = _rectangles[0].Height;
                            _rectanglesSource[1].Height = _textures[1].Height;
                            _rectanglesSource[2].Height = _textures[2].Height;
                            _rectanglesSource[0].Y = Convert.ToInt32(_textures[0].Height - _rectanglesSource[0].Height);
                            _rectanglesSource[1].Y = 0;
                            _rectanglesSource[2].Y = 0;
                        }
                        // Largeur
                        _rectangles[0].Width = Convert.ToInt32(Dimensions.X);
                        _rectangles[1].Width = Convert.ToInt32(Dimensions.X);
                        _rectangles[2].Width = Convert.ToInt32(Dimensions.X);
                        _rectanglesSource[0].Width = _textures[0].Width;
                        _rectanglesSource[1].Width = _textures[1].Width;
                        _rectanglesSource[2].Width = _textures[2].Width;
                        // Position Y
                        _rectangles[0].Y = Convert.ToInt32(Position.Y + Dimensions.Y - _rectangles[2].Height - _rectangles[1].Height - _rectangles[0].Height);
                        _rectangles[1].Y = Convert.ToInt32(Position.Y + Dimensions.Y - _rectangles[2].Height - _rectangles[1].Height);
                        _rectangles[2].Y = Convert.ToInt32(Position.Y + Dimensions.Y - _rectangles[2].Height);
                        // Position X
                        _rectangles[0].X = Convert.ToInt32(Position.X);
                        _rectangles[1].X = Convert.ToInt32(Position.X);
                        _rectangles[2].X = Convert.ToInt32(Position.X);
                        _rectanglesSource[0].X = 0;
                        _rectanglesSource[1].X = 0;
                        _rectanglesSource[2].X = 0;
                    }
                    break;
                default:
                    throw new InvalidEnumArgumentException("Unknown mode.");
            }

        }
        public void Update(GameTime gameTime)
        {
            ArgumentNullException.ThrowIfNull(gameTime);
            if (AutoIncrement)
            {
                _timer += Convert.ToSingle(gameTime.ElapsedGameTime.TotalSeconds);
                if (_timer >= _delay)
                {
                    _timer = 0;
                    Value += _increment;
                }
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (Visible)
            {
                switch (_pbtype)
                {
                    case PBType.Color:
                        if (_textures.Count == 0)
                        {
                            _textures.Add(new Texture2D(spritebatch.GraphicsDevice, 1, 1));
                            _textures[0].SetData(new[] { Color.White });
                        }
                        spritebatch.Draw(_textures[0], _rectangles[0], _borderColor);
                        spritebatch.Draw(_textures[0], _rectangles[1], _backColor);
                        spritebatch.Draw(_textures[0], _rectangles[2], _frontColor);
                        break;
                    case PBType.Image2Parts:
                        spritebatch.Draw(_textures[0], _rectangles[0], Color.White);
                        spritebatch.Draw(_textures[1], _rectangles[1], Color.White);
                        break;
                    case PBType.Image3Parts:
                        spritebatch.Draw(_textures[0], _rectangles[0], _rectanglesSource[0], Color.White);
                        spritebatch.Draw(_textures[1], _rectangles[1], _rectanglesSource[1], Color.White);
                        spritebatch.Draw(_textures[2], _rectangles[2], _rectanglesSource[2], Color.White);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unknown type of ProgressBar");
                }
            }
        }

        public void Reset(float value = 0)
        {
            _timer = 0;
            Value = value;
        }

        public ProgressBar Copy()
        {
            Rectangle[] copiedRectangles = new Rectangle[_rectangles.Length];
            Array.Copy(this._rectangles, copiedRectangles, _rectangles.Length);
            Rectangle[] copiedRectanglesSource = new Rectangle[_rectangles.Length];
            Array.Copy(this._rectanglesSource, copiedRectanglesSource, _rectanglesSource.Length);
            List<Texture2D> copiedTextures = new List<Texture2D>();
            foreach(Texture2D texture in _textures)
                copiedTextures.Add(texture);
            return new ProgressBar()
            {
                _autoIncrement = this._autoIncrement,
                _backColor = this._backColor,
                _borderColor = this._borderColor,
                _borderThickness = this._borderThickness,
                _delay = this._delay,
                _frontColor = this._frontColor,
                _imgOffset = this._imgOffset,
                _increment = this._increment,
                _maxValue = this._maxValue,
                _mode = this._mode,
                _pbtype = this._pbtype,
                _rectangles = copiedRectangles,
                _rectanglesSource = copiedRectanglesSource,
                _textures = copiedTextures,
                _timer = this._timer,
                _value = this._value,                
                _visible = this._visible,
                AutoIncrement = this.AutoIncrement,
                Dimensions = this.Dimensions,
                MaxValue = this.MaxValue,
                Mode = this.Mode,
                Position = this.Position,
                Value = this.Value,
                Visible = this.Visible,
                ZOrder = this.ZOrder,
            };
        }
        private ProgressBar() { }
    }
}
