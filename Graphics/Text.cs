using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Interfaces;
using DinaFramework.Translation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
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
        public Color Color
        {
            get { return _color; }
            set { _color = value; }
        }
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
        public override Vector2 Position
        {
            get { return base.Position; }
            set
            {
                base.Position = value;
                UpdateDisplayPosition();
            }
        }
        public override Vector2 Dimensions
        {
            get { return base.Dimensions; }
            set
            {
                base.Dimensions = value;
                UpdateDisplayPosition();
            }
        }
        public SpriteFont Font
        {
            get => _font;
            set
            {
                _font = value;
                UpdateDisplayPosition();
            }
        }
        public Text(SpriteFont font, string content, Color color, Vector2 position = default,
                    HorizontalAlignment horizontalalignment = HorizontalAlignment.Left, VerticalAlignment verticalalignment = VerticalAlignment.Top, int zorder = 0)
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
        public Vector2 TextDimensions => _font.MeasureString(TranslationManager.GetTranslation(Content));
        public void SetAlignments(HorizontalAlignment halign, VerticalAlignment valign)
        {
            _halign = halign;
            _valign = valign;
            UpdateDisplayPosition();
        }
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);
            if (_visible && _displayed)
                //Vector2 scale = TextDimensions / Dimensions;
                spritebatch.DrawString(_font, TranslationManager.GetTranslation(Content), _displayposition, _color, 0, Vector2.Zero, Vector2.One, SpriteEffects.None, 0);
        }
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
        public void UpdateDisplayPosition()
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
