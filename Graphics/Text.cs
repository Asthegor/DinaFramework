﻿using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Interfaces;
using DinaFramework.Translation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente un texte à afficher avec des options de temporisation et d'alignement.
    /// </summary>
    public class Text : Base, IUpdate, IDraw, IColor, IVisible, IText, ICopyable<Text>
    {
        SpriteFont _font;
        string _content;
        Color _color;
        bool _visible;

        HorizontalAlignment _halign;
        VerticalAlignment _valign;

        Vector2 _displayposition;

        float _waitTime;
        float _displayTime;
        int _nbLoops;
        float _timerWaitTime;
        float _timerDisplayTime;
        bool _wait;
        bool _displayed;

        /// <summary>
        /// Le contenu du texte.
        /// </summary>
        public string Content
        {
            get { return _content; }
            set
            {
                _content = value;
                Vector2 currentDim = Dimensions;
                Vector2 textDim = _font.MeasureString(TranslationManager.GetTranslation(value));
                if (currentDim.X < textDim.X)
                    currentDim.X = textDim.X;
                if (currentDim.Y < textDim.Y)
                    currentDim.Y = textDim.Y;
                Dimensions = currentDim;
            }
        }
        /// <summary>
        /// La couleur du texte.
        /// </summary>
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
        /// <summary>
        /// Indique si le texte est visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set
            {
                _visible = value;
                _timerWaitTime = 0;
                _timerDisplayTime = 0;
            }
        }
        /// <summary>
        /// La position du texte sur l'écran.
        /// </summary>
        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                UpdateDisplayPosition();
            }
        }
        /// <summary>
        /// Les dimensions du texte.
        /// </summary>
        public override Vector2 Dimensions
        {
            get { return base.Dimensions; }
            set
            {
                base.Dimensions = value;
                UpdateDisplayPosition();
            }
        }
        /// <summary>
        /// La police du texte.
        /// </summary>
        public SpriteFont Font
        {
            get => _font;
            set
            {
                _font = value;
                UpdateDisplayPosition();
            }
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Text.
        /// </summary>
        /// <param name="font">La police utilisée pour afficher le texte.</param>
        /// <param name="content">Le contenu du texte.</param>
        /// <param name="color">La couleur du texte.</param>
        /// <param name="position">La position du texte sur l'écran (optionnel).</param>
        /// <param name="horizontalalignment">L'alignement horizontal du texte (optionnel).</param>
        /// <param name="verticalalignment">L'alignement vertical du texte (optionnel).</param>
        /// <param name="zorder">L'ordre de superposition du texte (optionnel).</param>
        public Text(SpriteFont font, string content, Color color, Vector2 position = default, HorizontalAlignment horizontalalignment = HorizontalAlignment.Left, VerticalAlignment verticalalignment = VerticalAlignment.Top, int zorder = 0)
        {
            _font = font;
            Content = content;
            _color = color;
            _wait = false;
            _displayposition = position;
            Position = position;
            Dimensions = _font.MeasureString(TranslationManager.GetTranslation(Content));
            SetAlignments(horizontalalignment, verticalalignment);
            ZOrder = zorder;
            _displayed = true;
            Visible = true;
        }
        /// <summary>
        /// Définit les temporisations d'attente, d'affichage et le nombre de boucles du texte.
        /// </summary>
        /// <param name="waitTime">Temps d'attente avant l'affichage du texte.</param>
        /// <param name="displayTime">Temps d'affichage du texte.</param>
        /// <param name="nbLoops">Nombre de boucles d'affichage.</param>
        public void SetTimers(float waitTime = -1.0f, float displayTime = -1.0f, int nbLoops = -1)
        {
            _waitTime = waitTime;
            _displayTime = displayTime;
            _nbLoops = nbLoops;

            _displayed = false;
            _wait = false;
            if (waitTime == 0.0f)
                _displayed = true;
            else if (waitTime > 0.0f)
                _wait = true;
        }
        /// <summary>
        /// Obtient les dimensions du texte à partir de la police et du contenu.
        /// </summary>
        public Vector2 TextDimensions => _font.MeasureString(TranslationManager.GetTranslation(Content));
        /// <summary>
        /// Définit les alignements horizontal et vertical du texte.
        /// </summary>
        /// <param name="halign">L'alignement horizontal du texte.</param>
        /// <param name="valign">L'alignement vertical du texte.</param>
        public void SetAlignments(HorizontalAlignment halign, VerticalAlignment valign)
        {
            _halign = halign;
            _valign = valign;
            UpdateDisplayPosition();
        }
        /// <summary>
        /// Dessine le texte à l'écran.
        /// </summary>
        /// <param name="spritebatch">L'instance de SpriteBatch utilisée pour dessiner le texte.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);
            if (_visible && _displayed)
                //Vector2 scale = TextDimensions / Dimensions;
                spritebatch.DrawString(_font, TranslationManager.GetTranslation(Content), _displayposition, _color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
        /// <summary>
        /// Met à jour l'état du texte en fonction du temps écoulé.
        /// </summary>
        /// <param name="gametime">Temps écoulé depuis la dernière mise à jour.</param>
        public void Update(GameTime gametime)
        {
            ArgumentNullException.ThrowIfNull(gametime);

            if (_visible)
            {
                float dt = (float)gametime.ElapsedGameTime.TotalSeconds;
                if (_wait)
                {
                    _timerWaitTime += dt;
                    if (_timerWaitTime > _waitTime)
                    {
                        _timerWaitTime = 0.0f;
                        _wait = false;
                        _displayed = true;
                    }
                }
                else if (_displayed)
                {
                    if (_nbLoops != 0)
                    {
                        _timerDisplayTime += dt;
                        if (_timerDisplayTime > _displayTime)
                        {
                            _timerDisplayTime = 0.0f;
                            _displayed = false;
                            _wait = true;
                            if (_nbLoops > 0)
                                _nbLoops--;
                            if (_nbLoops == 0)
                                _wait = false;
                        }
                    }
                }
            }
        }
        private void UpdateDisplayPosition()
        {
            Vector2 offset = new Vector2();

            if (_halign == HorizontalAlignment.Center)
                offset.X = (base.Dimensions.X - TextDimensions.X) / 2.0f;
            else if (_halign == HorizontalAlignment.Right)
                offset.X = base.Dimensions.X - TextDimensions.X;

            if (_valign == VerticalAlignment.Center)
                offset.Y = (base.Dimensions.Y - TextDimensions.Y) / 2.0f;
            else if (_valign == VerticalAlignment.Bottom)
                offset.Y = base.Dimensions.Y - TextDimensions.Y;

            _displayposition = base.Position + offset;
        }

        /// <summary>
        /// Crée une copie de l'objet actuel.
        /// </summary>
        /// <returns>Une nouvelle instance de Text avec les mêmes propriétés.</returns>
        public Text Copy()
        {
            return new Text()
            {
                _color = _color,
                _content = _content,
                _displayed = _displayed,
                _displayposition = _displayposition,
                _displayTime = _displayTime,
                _font = _font,
                _halign = _halign,
                _valign = _valign,
                _nbLoops = _nbLoops,
                _timerDisplayTime = _timerDisplayTime,
                _timerWaitTime = _timerWaitTime,
                _visible = _visible,
                _wait = _wait,
                _waitTime = _waitTime,
                Color = Color,
                Content = Content,
                Dimensions = Dimensions,
                Position = Position,
                Visible = Visible,
                ZOrder = ZOrder
            };
        }
        private Text() { }
    }
}
