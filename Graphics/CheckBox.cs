using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace DinaFramework.Graphics
{
    public enum CheckBoxState { Unchecked, Checked };
    public class CheckBox : Base, IUpdate, IDraw, IVisible, ICopyable<CheckBox>, ILocked
    {
        private Rectangle _checkBoxRect;
        private bool _useTextures;
        private Texture2D _checkedTexture;
        private Texture2D _uncheckedTexture;
        private Color _checkedColor;
        private Color _uncheckedColor;
        private static Texture2D _pixelTexture;
        private MouseState _oldMouseState;

        public CheckBox(Color checkedColor, Color uncheckedColor, Vector2 position, Vector2 dimensions, int zorder = 0) :
            base(position, dimensions, zorder)
        {
            _checkedColor = checkedColor;
            _uncheckedColor = uncheckedColor;
            Position = position;
            Initialize();
        }
        public CheckBox(Texture2D uncheckedTexture, Texture2D checkedTexture, Vector2 position, Vector2 dimensions, int zorder = 0) :
            base(position, dimensions, zorder)
        {
            _uncheckedTexture = uncheckedTexture;
            _checkedTexture = checkedTexture;
            _useTextures = true;
            Position = position;
            Initialize();
        }
        private void Initialize()
        {
            Visible = true;
            State = CheckBoxState.Unchecked;
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
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _checkBoxRect = new Rectangle(Position.ToPoint(), value.ToPoint());
            }
        }

        public bool Visible { get; set; }
        public bool Locked { get; set; }
        public CheckBoxState State { get; set; }

        public void Update(GameTime gametime)
        {
            MouseState ms = Mouse.GetState();
            if (!Locked)
            {
                if (_oldMouseState.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Released)
                {
                    if (_checkBoxRect.Contains(new Point(ms.X, ms.Y)))
                    {
                        if (State == CheckBoxState.Checked)
                            State = CheckBoxState.Unchecked;
                        else
                            State = CheckBoxState.Checked;
                    }
                }
            }
            _oldMouseState = ms;
        }
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (Visible)
            {
                float ratio = 1;
                if (Locked)
                    ratio = 0.75f;
                if (_useTextures)
                {
                    if (State == CheckBoxState.Checked)
                        spritebatch.Draw(_checkedTexture, _checkBoxRect, Color.White * ratio);
                    else
                    {
                        spritebatch.Draw(_uncheckedTexture, _checkBoxRect, Color.White * ratio);
                    }
                }
                else
                {
                    // Dessine un rectangle non plein
                    if (State == CheckBoxState.Checked)
                        DrawRectangle(spritebatch, _checkBoxRect, _checkedColor * ratio, isFilled: true);
                    else
                        DrawRectangle(spritebatch, _checkBoxRect, _uncheckedColor * ratio, isFilled: false);
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
                spritebatch.Draw(_pixelTexture, rectangle, color);
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
                _checkBoxRect = _checkBoxRect,
                _checkedColor = _checkedColor,
                _checkedTexture = _checkedTexture,
                _uncheckedColor = _uncheckedColor,
                _uncheckedTexture = _uncheckedTexture,
                _useTextures = _useTextures,
                Dimensions = Dimensions,
                Position = Position,
                State = State,
                Visible = Visible,
                ZOrder = ZOrder,
                Locked = Locked,
            };
        }
        private CheckBox() { }
    }
}
