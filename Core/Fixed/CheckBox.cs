using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DinaFramework.Interfaces;

namespace DinaFramework.Core.Fixed
{
    public class CheckBox : Base, IUpdate, IDraw
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

        public bool IsChecked { get => _isChecked; set => _isChecked = value; }
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
            if (spritebatch == null)
                return;
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
        private static void DrawRectangle(SpriteBatch spritebatch, Rectangle rectangle, Color color, bool isFilled)
        {
            if (spritebatch == null)
                return;
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
    }
}
