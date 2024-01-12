using DinaFramework.Enums;
using DinaFramework.Interfaces;
using DinaFramework.Translation;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Core.Fixed
{
    public class ShadowText : Base, IUpdate, IDraw, IColor, IVisible, IText
    {
        readonly Text _text;
        readonly Text _shadow;
        Vector2 _offset;
        public ShadowText(SpriteFont font, string content, Color color, Vector2 position, Color shadowcolor, Vector2 offset,
                          HorizontalAlignment halign = default, VerticalAlignment valign = default, int zorder = 0)
        {
            _text = new Text(font, content, color, position, halign, valign, zorder);
            _shadow = new Text(font, content, shadowcolor, position + offset, halign, valign, zorder - 1);
            Offset = offset;
        }
        public ShadowText(ShadowText shadowtext)
        {
            ArgumentNullException.ThrowIfNull(shadowtext);

            _text = new Text(shadowtext._text);
            _shadow = new Text(shadowtext._shadow);
            Offset = shadowtext.Offset;
        }
        public Color Color
        {
            get { return _text.Color; }
            set { _text.Color = value; }
        }
        public Color ShadowColor
        {
            get { return _shadow.Color; }
            set { _shadow.Color = value; }
        }
        public void SetTimers(float waitTime = -1.0f, float displayTime = -1.0f, int nbLoops = -1)
        {
            _shadow.SetTimers(waitTime, displayTime, nbLoops);
            _text.SetTimers(waitTime, displayTime, nbLoops);
        }
        public string Content
        {
            get { return TranslationManager.GetTranslation(_text.Content); }
            set
            {
                _shadow.Content = value;
                _text.Content = value;
            }
        }
        public void Update(GameTime gameTime)
        {
            _shadow.Update(gameTime);
            _text.Update(gameTime);
        }
        public void Draw(SpriteBatch spritebatch)
        {
            _shadow.Draw(spritebatch);
            _text.Draw(spritebatch);
        }
        public override Vector2 Position
        {
            get { return _text.Position; }
            set
            {
                if (_text != null)
                    _text.Position = value;
                if (_shadow != null)
                    _shadow.Position = value + Offset;
            }
        }
        public override Vector2 Dimensions
        {
            get { return _text.Dimensions + Offset; }
            set
            {
                if (_text != null)
                    _text.Dimensions = value;
                if (_shadow != null)
                    _shadow.Dimensions = value;
            }
        }
        public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
        public new int ZOrder
        {
            get { return _text.ZOrder; }
            set
            {
                _text.ZOrder = value;
                _shadow.ZOrder = value - 1;
            }
        }
        public bool Visible
        {
            get { return _text.Visible; }
            set
            {
                _text.Visible = value;
                _shadow.Visible = value;
            }
        }
    }
}
