using DinaFramework.Enums;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace DinaFramework.Core.Fixed
{
    public class ProgressBar : Base, IUpdate, IDraw, IVisible
    {
        public enum PBType { Color, Image2Parts, Image3Parts }
        public PBType _PBType;
        private bool _visible;
        private float _value;
        private readonly float _maxValue;
        private int _borderThickness;
        private readonly int _imgoffset;
        private Color _frontColor;
        private Color _backColor;
        private Color _borderColor;
        private readonly Rectangle[] _rectangles = new Rectangle[3];
        private readonly Rectangle[] _rectanglesSource = new Rectangle[3];
        private ProgressBarMode _mode;
        private readonly List<Texture2D> _textures = new List<Texture2D>();


        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Color frontColor, Color borderColor, Color backColor, int borderThickness = 1, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            _maxValue = maxValue;
            Value = value;
            _rectangles[0] = new Rectangle(position.ToPoint(), dimensions.ToPoint()); // Border
            SetColors(frontColor, borderColor, backColor, borderThickness);
        }
        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Texture2D backImage, Texture2D frontImage, int offset = 0, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            _maxValue = maxValue;
            Value = value;
            _imgoffset = offset;
            //_rectBack = new Rectangle(position.ToPoint(), dimensions.ToPoint());
            _rectangles[0] = new Rectangle(position.ToPoint(), dimensions.ToPoint());
            SetImages(backImage, frontImage, _imgoffset);
        }
        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Texture2D leftImage, Texture2D centerImage, Texture2D rightImage, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            _maxValue = maxValue;
            Value = value;
            _rectangles[1] = new Rectangle(position.ToPoint(), dimensions.ToPoint());
            SetImages(leftImage, centerImage, rightImage);
        }
        public bool Visible { get => _visible; set => _visible = value; }
        public float Value
        {
            get => _value;
            set
            {
                if (value < 0.0f)
                    value = 0.0f;
                if (value > _maxValue)
                    value = _maxValue;
                _value = value;
                switch (_PBType)
                {
                    case PBType.Color:
                        UpdateColorRectangle();
                        break;
                    case PBType.Image2Parts:
                        Update2ImagesRectangle(_imgoffset);
                        break;
                    case PBType.Image3Parts:
                        Update3ImagesRectangle();
                        break;
                    default:
                        throw new Exception("Unknown ProgressBarType");
                }

            }
        }

        public ProgressBarMode Mode { get => _mode; set => _mode = value; }

        public float GetMaxValue() => _maxValue;
        public void SetImages(Texture2D backImage, Texture2D frontImage, int offset = 0)
        {
            _PBType = PBType.Image2Parts;
            if (_textures.Count == 0) _textures.Add(backImage); else _textures[0] = backImage;
            if (_textures.Count == 1) _textures.Add(frontImage); else _textures[1] = frontImage;
            Update2ImagesRectangle(offset);
        }
        public void SetImages(Texture2D leftImage, Texture2D centerImage, Texture2D rightImage)
        {
            _PBType = PBType.Image3Parts;
            if (_textures.Count == 0) _textures.Add(leftImage); else _textures[0] = leftImage;
            if (_textures.Count == 1) _textures.Add(centerImage); else _textures[1] = centerImage;
            if (_textures.Count == 2) _textures.Add(rightImage); else _textures[2] = rightImage;
            Update3ImagesRectangle();
        }
        public void SetColors(Color frontColor, Color borderColor, Color backColor, int borderThickness = 1)
        {
            _PBType = PBType.Color;
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

        public void Draw(SpriteBatch spritebatch)
        {
            if (Visible)
            {
                switch (_PBType)
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
                        throw new Exception("Unknown type of ProgressBar");
                }
            }
        }

        public void Update(GameTime gameTime)
        {

        }


        private void UpdateColorRectangle()
        {
            float ratio = _value / _maxValue;
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
                    throw new Exception("Unknown mode.");
            }
            _rectangles[2] = new Rectangle(Convert.ToInt32(posX), Convert.ToInt32(posY), Convert.ToInt32(width), Convert.ToInt32(height));
        }
        private void Update2ImagesRectangle(int offset = 0)
        {
            float ratio = _value / _maxValue;
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
                    throw new Exception("Unknown mode.");
            }
            _rectangles[1] = new Rectangle(Convert.ToInt32(posX), Convert.ToInt32(posY), Convert.ToInt32(width), Convert.ToInt32(height));
        }
        private void Update3ImagesRectangle()
        {
            float ratio = _value / _maxValue;
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
                    throw new Exception("Unknown mode.");
            }

        }
    }
}
