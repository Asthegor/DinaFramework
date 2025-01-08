using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Classe représentant une barre de progression.
    /// </summary>
    public class ProgressBar : Base, IDraw, IUpdate, IVisible, ICopyable<ProgressBar>
    {
        private ProgressBarType _pbtype;
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

        /// <summary>
        /// Initialise une nouvelle instance de la classe ProgressBar avec un fond de couleur et une bordure.
        /// </summary>
        /// <param name="position">Position de la barre de progression.</param>
        /// <param name="dimensions">Dimensions de la barre de progression.</param>
        /// <param name="value">Valeur initiale de la barre.</param>
        /// <param name="maxValue">Valeur maximale de la barre.</param>
        /// <param name="frontColor">Couleur du remplissage.</param>
        /// <param name="borderColor">Couleur de la bordure.</param>
        /// <param name="backColor">Couleur de fond.</param>
        /// <param name="borderThickness">Épaisseur de la bordure.</param>
        /// <param name="mode">Mode de la barre de progression.</param>
        /// <param name="zorder">Ordre de superposition.</param>
        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Color frontColor, Color borderColor, Color backColor, int borderThickness = 1, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            MaxValue = maxValue;
            Value = value;
            _rectangles[0] = new Rectangle(position.ToPoint(), dimensions.ToPoint()); // Border
            SetColors(frontColor, borderColor, backColor, borderThickness);
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe ProgressBar avec une image de fond et une image de remplissage.
        /// </summary>
        /// <param name="position">Position de la barre de progression.</param>
        /// <param name="dimensions">Dimensions de la barre de progression.</param>
        /// <param name="value">Valeur initiale de la barre.</param>
        /// <param name="maxValue">Valeur maximale de la barre.</param>
        /// <param name="backImage">Image de fond.</param>
        /// <param name="frontImage">Image de remplissage.</param>
        /// <param name="offset">Décalage de l'image.</param>
        /// <param name="mode">Mode de la barre de progression.</param>
        /// <param name="zorder">Ordre de superposition.</param>
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
        /// <summary>
        /// Initialise une nouvelle instance de la classe ProgressBar avec trois images (gauche, centre, droite) pour le fond et le remplissage.
        /// </summary>
        /// <param name="position">Position de la barre de progression.</param>
        /// <param name="dimensions">Dimensions de la barre de progression.</param>
        /// <param name="value">Valeur initiale de la barre.</param>
        /// <param name="maxValue">Valeur maximale de la barre.</param>
        /// <param name="leftImage">Image de la partie gauche.</param>
        /// <param name="centerImage">Image de la partie centrale.</param>
        /// <param name="rightImage">Image de la partie droite.</param>
        /// <param name="mode">Mode de la barre de progression.</param>
        /// <param name="zorder">Ordre de superposition.</param>
        public ProgressBar(Vector2 position, Vector2 dimensions, float value, float maxValue, Texture2D leftImage, Texture2D centerImage, Texture2D rightImage, ProgressBarMode mode = ProgressBarMode.LeftToRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            Visible = true;
            Mode = mode;
            MaxValue = maxValue;
            Value = value;
            _rectangles[1] = new Rectangle(position.ToPoint(), dimensions.ToPoint());
            SetImages(leftImage, centerImage, rightImage);
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe ProgressBar avec trois images (gauche, centre, droite) pour le fond et le remplissage.
        /// </summary>
        /// <param name="value">Valeur initiale de la barre.</param>
        /// <param name="maxValue">Valeur maximale de la barre.</param>
        /// <param name="leftImage">Image de la partie gauche.</param>
        /// <param name="centerImage">Image de la partie centrale.</param>
        /// <param name="rightImage">Image de la partie droite.</param>
        /// <param name="mode">Mode de la barre de progression.</param>
        /// <param name="zorder">Ordre de superposition.</param>
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

        /// <summary>
        /// Indique si la barre de progression doit s'incrémenter automatiquement.
        /// </summary>
        public bool AutoIncrement { get => _autoIncrement; set => _autoIncrement = value; }
        /// <summary>
        /// La valeur maximale de la barre de progression.
        /// </summary>
        public float MaxValue { get => _maxValue; set => _maxValue = value; }
        /// <summary>
        /// Mode de la barre de progression, définissant la direction de l'animation.
        /// </summary>
        public ProgressBarMode Mode { get => _mode; set => _mode = value; }
        /// <summary>
        /// Définit l'intervalle de progression automatique avec un délai et un incrément.
        /// </summary>
        /// <param name="delay">Délai entre chaque incrément.</param>
        /// <param name="increment">Valeur de l'incrément à chaque intervalle.</param>
        public void ProgressInterval(float delay, float increment = 1)
        {
            if (delay > 0 && increment != 0)
            {
                _delay = delay;
                _increment = increment;
                AutoIncrement = true;
            }
        }
        /// <summary>
        /// La valeur actuelle de la barre de progression.
        /// </summary>
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
                    case ProgressBarType.Color:
                        UpdateColorRectangle();
                        break;
                    case ProgressBarType.Image2Parts:
                        Update2ImagesRectangle(_imgOffset);
                        break;
                    case ProgressBarType.Image3Parts:
                        Update3ImagesRectangle();
                        break;
                }

            }
        }
        /// <summary>
        /// Indique si la barre de progression est visible ou non.
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }
        /// <summary>
        /// Le délai de progression automatique.
        /// </summary>
        public float Delay
        {
            get => _delay;
            set => _delay = value;
        }
        /// <summary>
        /// L.incrément de progression automatique.
        /// </summary>
        public float Increment
        {
            get => _increment;
            set => _increment = value;
        }

        /// <summary>
        /// Définit les images de la barre de progression avec un arrière-plan et un avant-plan.
        /// </summary>
        /// <param name="backImage">Image de fond.</param>
        /// <param name="frontImage">Image de remplissage.</param>
        /// <param name="offset">Décalage de l'image.</param>
        public void SetImages(Texture2D backImage, Texture2D frontImage, int offset = 0)
        {
            _pbtype = ProgressBarType.Image2Parts;
            if (_textures.Count == 0)
                _textures.Add(backImage);
            else
                _textures[0] = backImage;
            if (_textures.Count == 1)
                _textures.Add(frontImage);
            else
                _textures[1] = frontImage;
            Update2ImagesRectangle(offset);
        }
        /// <summary>
        /// Définit les images de la barre de progression avec trois parties (gauche, centre, droite).
        /// </summary>
        /// <param name="leftImage">Image de la partie gauche.</param>
        /// <param name="centerImage">Image de la partie centrale.</param>
        /// <param name="rightImage">Image de la partie droite.</param>
        public void SetImages(Texture2D leftImage, Texture2D centerImage, Texture2D rightImage)
        {
            _pbtype = ProgressBarType.Image3Parts;
            if (_textures.Count == 0)
                _textures.Add(leftImage);
            else
                _textures[0] = leftImage;
            if (_textures.Count == 1)
                _textures.Add(centerImage);
            else
                _textures[1] = centerImage;
            if (_textures.Count == 2)
                _textures.Add(rightImage);
            else
                _textures[2] = rightImage;
            Update3ImagesRectangle();
        }
        /// <summary>
        /// Définit les couleurs et la bordure de la barre de progression.
        /// </summary>
        /// <param name="frontColor">Couleur de remplissage.</param>
        /// <param name="borderColor">Couleur de bordure.</param>
        /// <param name="backColor">Couleur de fond.</param>
        /// <param name="borderThickness">Épaisseur de la bordure.</param>
        public void SetColors(Color frontColor, Color borderColor, Color backColor, int borderThickness = 1)
        {
            _pbtype = ProgressBarType.Color;
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
        /// <summary>
        /// Met à jour l'état de la barre de progression en fonction du temps écoulé et de son mode.
        /// </summary>
        /// <param name="gameTime">Le temps de jeu écoulé depuis la dernière mise à jour.</param>
        public void Update(GameTime gametime)
        {
            ArgumentNullException.ThrowIfNull(gametime);
            if (AutoIncrement)
            {
                _timer += Convert.ToSingle(gametime.ElapsedGameTime.TotalSeconds);
                if (_timer >= _delay)
                {
                    _timer = 0;
                    Value += _increment;
                }
            }
        }
        /// <summary>
        /// Dessine la barre de progression sur l'écran en fonction de son état actuel (valeur, images, couleurs, etc.).
        /// </summary>
        /// <param name="spriteBatch">Le spriteBatch utilisé pour dessiner la barre de progression.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (Visible)
            {
                switch (_pbtype)
                {
                    case ProgressBarType.Color:
                        if (_textures.Count == 0)
                        {
                            _textures.Add(new Texture2D(spritebatch.GraphicsDevice, 1, 1));
                            _textures[0].SetData(new[] { Color.White });
                        }
                        spritebatch.Draw(_textures[0], _rectangles[0], _borderColor);
                        spritebatch.Draw(_textures[0], _rectangles[1], _backColor);
                        spritebatch.Draw(_textures[0], _rectangles[2], _frontColor);
                        break;
                    case ProgressBarType.Image2Parts:
                        spritebatch.Draw(_textures[0], _rectangles[0], Color.White);
                        spritebatch.Draw(_textures[1], _rectangles[1], Color.White);
                        break;
                    case ProgressBarType.Image3Parts:
                        spritebatch.Draw(_textures[0], _rectangles[0], _rectanglesSource[0], Color.White);
                        spritebatch.Draw(_textures[1], _rectangles[1], _rectanglesSource[1], Color.White);
                        spritebatch.Draw(_textures[2], _rectangles[2], _rectanglesSource[2], Color.White);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Unknown type of ProgressBar");
                }
            }
        }

        /// <summary>
        /// Réinitialise la barre de progression à son état initial, en réinitialisant sa valeur de progression.
        /// </summary>
        public void Reset(float value = 0)
        {
            _timer = 0;
            Value = value;
        }

        /// <summary>
        /// Crée une copie de la barre de progression actuelle, avec la même valeur de progression.
        /// </summary>
        /// <returns>Une nouvelle instance de la barre de progression avec les mêmes valeurs.</returns>
        public ProgressBar Copy()
        {
            Rectangle[] copiedRectangles = new Rectangle[_rectangles.Length];
            Array.Copy(_rectangles, copiedRectangles, _rectangles.Length);
            Rectangle[] copiedRectanglesSource = new Rectangle[_rectangles.Length];
            Array.Copy(_rectanglesSource, copiedRectanglesSource, _rectanglesSource.Length);
            List<Texture2D> copiedTextures = new List<Texture2D>();
            foreach (Texture2D texture in _textures)
                copiedTextures.Add(texture);
            return new ProgressBar()
            {
                _autoIncrement = _autoIncrement,
                _backColor = _backColor,
                _borderColor = _borderColor,
                _borderThickness = _borderThickness,
                _delay = _delay,
                _frontColor = _frontColor,
                _imgOffset = _imgOffset,
                _increment = _increment,
                _maxValue = _maxValue,
                _mode = _mode,
                _pbtype = _pbtype,
                _rectangles = copiedRectangles,
                _rectanglesSource = copiedRectanglesSource,
                _textures = copiedTextures,
                _timer = _timer,
                _value = _value,
                _visible = _visible,
                AutoIncrement = AutoIncrement,
                Dimensions = Dimensions,
                MaxValue = MaxValue,
                Mode = Mode,
                Position = Position,
                Value = Value,
                Visible = Visible,
                ZOrder = ZOrder,
            };
        }
        private ProgressBar() { }
    }
}
