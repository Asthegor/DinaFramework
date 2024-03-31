using DinaFramework.Interfaces;
using DinaFramework.Menus;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Core.Fixed
{
    public class Button : Base, IUpdate, IDraw, IPosition, ICopyable<Button>
    {
        Text _text;
        Panel _background;
        Action _action;
        Func<Button, Button> _onHover;
        bool _locked;
        Sprite _lockedSprite;
        Vector2 _margin = new Vector2(10, 10);
        Color _lockedColor;
        Button _backup;
        public Button(Vector2 position, Vector2 dimensions, SpriteFont font, string content, Color textColor, Action action, Vector2 margin = default, Func<Button, Button> onHover = null)
        {
            _backup = null;
            _text = new Text(font, content, textColor);

            if (margin != default)
                _margin = margin;
            // Calcul des dimensions du fond
            Vector2 backgroundDimensions = _text.TextDimensions + _margin * 2;
            float width = Math.Max(backgroundDimensions.X, dimensions.X);
            float height = Math.Max(backgroundDimensions.Y, dimensions.Y);
            Dimensions = new Vector2(width, height);
            Position = position;
            _action = action;
            _onHover = onHover;
            _text.Position = Position + (Dimensions - _text.TextDimensions) / 2;
            _background = new Panel(Position, Dimensions, Color.Transparent);
        }
        public Button(Vector2 position, Vector2 dimensions, SpriteFont font, string content, Color textColor, Texture2D backgroundimage, Action action, Vector2 margin = default, Func<Button, Button> onHover = null)
            : this(position, dimensions, font, content, textColor, action, margin)
        {
            _onHover = onHover;
            _background = new Panel(Position, Dimensions, backgroundimage, 0);
        }
        public Action Action { get => _action; set => _action = value; }
        public Func<Button, Button> OnHover { get => _onHover; set => _onHover = value; }
        public string Content { get => _text.Content; set => _text.Content = value; }
        public new Vector2 Position
        {
            get => base.Position;
            set
            {
                if (_background != null) _background.Position = value;
                if (_lockedSprite != null) _lockedSprite.Position = value;
                if (_text != null && _background != null)
                    _text.Position = new Vector2(_background.Position.X + (_background.Dimensions.X - _text.TextDimensions.X) / 2,
                                                 _background.Position.Y + (_background.Dimensions.Y - _text.TextDimensions.Y) / 2);
                base.Position = value;
            }
        }

        public Color TextColor { get => _text.Color; set => _text.Color = value; }
        public Color BackgroundColor { get => _background.BackgroundColor; set => _background.BackgroundColor = value; }
        public bool Locked { get => _locked; set => _locked = value; }
        public void LockedImage(Texture2D lockedTexture, Color lockedColor)
        {
            if (lockedTexture != null)
            {
                _lockedSprite ??= new Sprite(lockedTexture, Color.White, Position);
                _lockedSprite.Texture = lockedTexture;
                _lockedSprite.Dimensions = Dimensions;
            }
            else
                _lockedSprite = null;

            _lockedColor = lockedColor;
        }
        public void Update(GameTime gameTime)
        {
            _background?.Update(gameTime);
            if (Locked == false)
            {
                if (_onHover != null)
                {
                    if (_background?.IsHovered() == true)
                    {
                        SaveState();
                        _onHover?.Invoke(this);
                    }
                    else
                        RestoreState();
                }
                if (_background?.IsClicked() == true)
                    _action?.Invoke();
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            Color backupColor = Color.White;
            if (_background != null)
                backupColor = _background.BackgroundColor;

            if (Locked)
                _background.BackgroundColor = _lockedColor;

            _background?.Draw(spritebatch);
            _text?.Draw(spritebatch);

            if (Locked)
            {
                _lockedSprite?.Draw(spritebatch);
                if (_background != null)
                    _background.BackgroundColor = backupColor;
            }

        }
        public void SetBackground(Texture2D backgroundimg)
        {
            _background = new Panel(_background.Position, _background.Dimensions, backgroundimg, 0);
        }

        public Button Copy()
        {
            return new Button()
            {
                _action = this._action,
                _background = this._background?.Copy(),
                _locked = this._locked,
                _lockedColor = this._lockedColor,
                _lockedSprite = this._lockedSprite?.Copy(),
                _margin = this._margin,
                _text = this._text?.Copy(),
                BackgroundColor = this.BackgroundColor,
                Content = this.Content,
                Dimensions = this.Dimensions,
                Position = this.Position,
                TextColor = this.TextColor,
                ZOrder = this.ZOrder,
            };
        }
        private Button() { }
        private void SaveState()
        {
            _backup ??= this.Copy();
        }
        private void RestoreState()
        {
            if (_backup != null)
            {
                _action = _backup._action;
                _background = _backup._background?.Copy();
                _locked = _backup._locked;
                _lockedColor = _backup._lockedColor;
                _lockedSprite = _backup._lockedSprite?.Copy();
                _margin = _backup._margin;
                _text = _backup._text?.Copy();
                BackgroundColor = _backup.BackgroundColor;
                Content = _backup.Content;
                Dimensions = _backup.Dimensions;
                Position = _backup.Position;
                TextColor = _backup.TextColor;
                ZOrder = _backup.ZOrder;
                _backup = null;
            }
        }
    }
}
