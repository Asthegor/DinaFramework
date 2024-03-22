using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace DinaFramework.Core.Fixed
{
    public class CheckBox : Base, IUpdate, IDraw, IVisible, ICopyable<CheckBox>
    {
        private Rectangle _checkBoxRect;
        private bool _isChecked;
        private bool _useTextures;
        private Texture2D _checkedTexture;
        private Texture2D _uncheckedTexture;
        private Color _checkedColor;
        private Color _uncheckedColor;
        private Text _label;
        private static Texture2D _pixelTexture;
        private bool _visible;

        public CheckBox(Color checkedColor, Color uncheckedColor, Vector2 position, Vector2 dimensions, SpriteFont font, string label, int zorder = 0) : base(position, dimensions, zorder)
        {
            _checkedColor = checkedColor;
            _uncheckedColor = uncheckedColor;
            IsChecked = false;
            Position = position;
            _label = new Text(font, label, Color.White);
            _label.Position = Position + new Vector2(15, 0);
        }
        public CheckBox(Texture2D uncheckedTexture, Texture2D checkedTexture, Vector2 position, Vector2 dimensions, SpriteFont font, string label, int zorder = 0) : base(position, dimensions, zorder)
        {
            _uncheckedTexture = uncheckedTexture;
            _checkedTexture = checkedTexture;
            _useTextures = true;
            _isChecked = false;
            Position = position;
            _label = new Text(font, label, Color.White);
            _label.Position = Position + new Vector2(15, 0);
        }
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _checkBoxRect = new Rectangle(Position.ToPoint(), base.Dimensions.ToPoint());
            }
        }
        public override Vector2 Dimensions
        {
            get
            {
                Vector2 dim = base.Dimensions + new Vector2(15, 0);
                dim.X += _label.TextDimensions.X;
                dim.Y = Math.Max(dim.Y, _label.TextDimensions.Y);
                return dim;
            }
            set
            {
                base.Dimensions = value;
                _checkBoxRect = new Rectangle(Position.ToPoint(), value.ToPoint());
            }
        }

        public bool IsChecked { get => _isChecked; set => _isChecked = value; }
        public bool Visible { get => _visible; set => _visible = value; }

        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Point mousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                if (_checkBoxRect.Contains(mousePosition))
                {
                    IsChecked = !IsChecked;
                }
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (Visible)
            {
                if (_useTextures)
                {
                    if (IsChecked)
                    {
                        spritebatch.Draw(_checkedTexture, _checkBoxRect, Color.White);
                    }
                    else
                    {
                        spritebatch.Draw(_uncheckedTexture, _checkBoxRect, Color.White);
                    }
                }
                else
                {
                    // Dessine un rectangle non plein
                    if (IsChecked)
                        DrawRectangle(spritebatch, _checkBoxRect, _checkedColor, isFilled: true);
                    else
                        DrawRectangle(spritebatch, _checkBoxRect, _uncheckedColor, isFilled: false);
                }
            }
        }
        private static void DrawRectangle(SpriteBatch spritebatch, Rectangle rectangle, Color color, bool isFilled)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            _pixelTexture ??= new Texture2D(spritebatch.GraphicsDevice, 1, 1);
            _pixelTexture.SetData(new Color[] { Color.White });
            // Dessine un rectangle non plein
            if (isFilled)
            {
                spritebatch.Draw(_pixelTexture, rectangle, color);
            }
            else
            {
                // Lignes horizontales
                spritebatch.Draw(_pixelTexture, new Rectangle(rectangle.Left, rectangle.Top, rectangle.Width, 1), color);
                spritebatch.Draw(_pixelTexture, new Rectangle(rectangle.Left, rectangle.Bottom - 1, rectangle.Width, 1), color);

                // Lignes verticales
                spritebatch.Draw(_pixelTexture, new Rectangle(rectangle.Left, rectangle.Top, 1, rectangle.Height), color);
                spritebatch.Draw(_pixelTexture, new Rectangle(rectangle.Right - 1, rectangle.Top, 1, rectangle.Height), color);
            }
        }

        public CheckBox Copy()
        {
            return new CheckBox()
            {
                _checkBoxRect = this._checkBoxRect,
                _checkedColor = this._checkedColor,
                _checkedTexture = this._checkedTexture,
                _isChecked = this._isChecked,
                _label = this._label?.Copy(),
                _uncheckedColor = this._uncheckedColor,
                _uncheckedTexture = this._uncheckedTexture,
                _useTextures = this._useTextures,
                _visible = this._visible,
                Dimensions = this.Dimensions,
                IsChecked = this.IsChecked,
                Position = this.Position,
                Visible = this.Visible,
                ZOrder = this.ZOrder,
            };
        }
        private CheckBox() { }
    }
}
