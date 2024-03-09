using DinaFramework.Interfaces;
using DinaFramework.Menus;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Core.Fixed
{
    public class Button : Base, IUpdate, IDraw
    {
        Text _text;
        Panel _background;
        Action _action;
        Vector2 _margin = new Vector2(10, 10);

        public Button(Vector2 position, Vector2 dimensions, SpriteFont font, string content, Color color, Texture2D backgroundimage, Action action, Vector2 margin = default) : base(position, dimensions)
        {
            _text = new Text(font, content, color);

            if (margin != default)
                _margin = margin;
            // Calcul des dimensions du fond
            Vector2 backgroundDimensions = _text.TextDimensions + _margin;
            dimensions = Vector2.Max(backgroundDimensions, dimensions);

            _background = new Panel(position, dimensions, backgroundimage, 0);

            _text.Position = new Vector2(_background.Position.X + (_background.Dimensions.X - _text.TextDimensions.X) / 2,
                                         _background.Position.Y + (_background.Dimensions.Y - _text.TextDimensions.Y) / 2);
            _action = action;
        }
        public string Content
        {
            get => _text.Content;
            set => _text.Content = value;
        }
        public new Vector2 Position
        {
            get => base.Position;
            set
            {
                _background.Position = value;
                _text.Position = new Vector2(_background.Position.X + (_background.Dimensions.X - _text.TextDimensions.X) / 2,
                                             _background.Position.Y + (_background.Dimensions.Y - _text.TextDimensions.Y) / 2);
                base.Position = value;
            }
        }
        public Color TextColor
        {
            get => _text.Color;
            set => _text.Color = value;
        }
        public void Update(GameTime gameTime)
        {
            _background?.Update(gameTime);
            if (_background?.IsClicked() == true)
                _action();
        }
        public void Draw(SpriteBatch spritebatch)
        {
            _background?.Draw(spritebatch);
            _text?.Draw(spritebatch);
        }
        public void SetBackground(Texture2D backgroundimg)
        {
            _background = new Panel(_background.Position, _background.Dimensions, backgroundimg, 0);
        }
    }
}
