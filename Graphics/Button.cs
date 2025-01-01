using DinaFramework.Core;
using DinaFramework.Interfaces;
using DinaFramework.Menus;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Classe représentant un bouton graphique interactif.
    /// </summary>
    public class Button : Base, IUpdate, IDraw, IPosition, ICopyable<Button>, ILocked
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
        /// <summary>
        /// Initialise une nouvelle instance de la classe Button avec un texte et un fond coloré.
        /// </summary>
        /// <param name="position">Position du bouton sur l'écran.</param>
        /// <param name="dimensions">Dimensions du bouton.</param>
        /// <param name="font">Police utilisée pour le texte du bouton.</param>
        /// <param name="content">Texte affiché sur le bouton.</param>
        /// <param name="textColor">Couleur du texte.</param>
        /// <param name="action">Action à exécuter lors d'un clic sur le bouton.</param>
        /// <param name="margin">Marge entre le texte et les bords du bouton.</param>
        /// <param name="onHover">Fonction appelée lors du survol du bouton.</param>
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
            _lockedColor = Color.Transparent;
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Button avec un texte et une image de fond.
        /// </summary>
        /// <param name="position">Position du bouton sur l'écran.</param>
        /// <param name="dimensions">Dimensions du bouton.</param>
        /// <param name="font">Police utilisée pour le texte du bouton.</param>
        /// <param name="content">Texte affiché sur le bouton.</param>
        /// <param name="textColor">Couleur du texte.</param>
        /// <param name="backgroundimage">Texture utilisée comme image de fond.</param>
        /// <param name="action">Action à exécuter lors d'un clic sur le bouton.</param>
        /// <param name="margin">Marge entre le texte et les bords du bouton.</param>
        /// <param name="onHover">Fonction appelée lors du survol du bouton.</param>
        public Button(Vector2 position, Vector2 dimensions, SpriteFont font, string content, Color textColor, Texture2D backgroundimage, Action action, Vector2 margin = default, Func<Button, Button> onHover = null)
            : this(position, dimensions, font, content, textColor, action, margin, onHover)
        {
            _background = new Panel(Position, Dimensions, backgroundimage, 0);
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Button avec une image de fond uniquement.
        /// </summary>
        /// <param name="position">Position du bouton sur l'écran.</param>
        /// <param name="backgroundimage">Texture utilisée comme image de fond.</param>
        /// <param name="action">Action à exécuter lors d'un clic sur le bouton.</param>
        /// <param name="onHover">Fonction appelée lors du survol du bouton.</param>
        public Button(Vector2 position, Texture2D backgroundimage, Action action, Func<Button, Button> onHover = null)
        {
            ArgumentNullException.ThrowIfNull(backgroundimage);

            _backup = null;
            Dimensions = new Vector2(backgroundimage.Width, backgroundimage.Height);
            Position = position;
            _action = action;
            _onHover = onHover;
            _background = new Panel(Position, Dimensions, backgroundimage, 0);
            _lockedColor = Color.Transparent;

        }
        /// <summary>
        /// Action à exécuter lorsque le bouton est cliqué.
        /// </summary>
        public Action Action { get => _action; set => _action = value; }
        /// <summary>
        /// Fonction appelée lorsque le curseur survole le bouton.
        /// </summary>
        public Func<Button, Button> OnHover { get => _onHover; set => _onHover = value; }
        /// <summary>
        /// Texte affiché sur le bouton.
        /// </summary>
        public string Content { get => _text.Content; set => _text.Content = value; }
        /// <summary>
        /// Position du bouton sur l'écran.
        /// </summary>
        public new Vector2 Position
        {
            get => base.Position;
            set
            {
                if (_background != null)
                    _background.Position = value;
                if (_lockedSprite != null)
                    _lockedSprite.Position = value;
                if (_text != null && _background != null)
                {
                    _text.Position = new Vector2(_background.Position.X + (_background.Dimensions.X - _text.TextDimensions.X) / 2,
                                                 _background.Position.Y + (_background.Dimensions.Y - _text.TextDimensions.Y) / 2);
                }
                base.Position = value;
            }
        }

        /// <summary>
        /// Couleur du texte du bouton.
        /// </summary>
        public Color TextColor { get => _text.Color; set => _text.Color = value; }
        /// <summary>
        /// Couleur de fond du bouton.
        /// </summary>
        public Color BackgroundColor { get => _background.BackgroundColor; set => _background.BackgroundColor = value; }
        /// <summary>
        /// Indique si le bouton est verrouillé.
        /// </summary>
        public bool Locked { get => _locked; set => _locked = value; }
        /// <summary>
        /// Police utilisée pour afficher le texte.
        /// </summary>
        public SpriteFont Font { get => _text.Font; set => _text.Font = value; }

        /// <summary>
        /// Définit l'image et la couleur utilisées lorsque le bouton est verrouillé.
        /// </summary>
        /// <param name="lockedTexture">Texture utilisée pour le verrouillage.</param>
        /// <param name="lockedColor">Couleur appliquée lorsque le bouton est verrouillé.</param>
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
        /// <summary>
        /// Met à jour l'état du bouton (gestion du survol, clic, etc.).
        /// </summary>
        /// <param name="gametime">Temps de jeu écoulé depuis la dernière mise à jour.</param>
        public void Update(GameTime gametime)
        {
            _background?.Update(gametime);
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
        /// <summary>
        /// Affiche le bouton à l'écran.
        /// </summary>
        /// <param name="spritebatch">Objet SpriteBatch utilisé pour dessiner le bouton.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            Color backupColor = Color.White;
            if (_background != null)
                backupColor = _background.BackgroundColor;

            if (Locked && _lockedColor != Color.Transparent)
                _background.BackgroundColor = _lockedColor;

            _background?.Draw(spritebatch);

            if (Locked)
            {
                _lockedSprite?.Draw(spritebatch);
                if (_background != null)
                    _background.BackgroundColor = backupColor;
            }

            _text?.Draw(spritebatch);
        }
        /// <summary>
        /// Définit une nouvelle texture de fond pour le bouton.
        /// </summary>
        /// <param name="backgroundimg">Texture de fond à appliquer.</param>
        public void SetBackground(Texture2D backgroundimg)
        {
            _background = new Panel(_background.Position, _background.Dimensions, backgroundimg, 0);
        }

        /// <summary>
        /// Crée une copie du bouton actuel.
        /// </summary>
        /// <returns>Retourne une nouvelle instance de Button avec les mêmes propriétés.</returns>
        public Button Copy()
        {
            return new Button()
            {
                _action = _action,
                _background = _background?.Copy(),
                _locked = _locked,
                _lockedColor = _lockedColor,
                _lockedSprite = _lockedSprite?.Copy(),
                _margin = _margin,
                _text = _text?.Copy(),
                BackgroundColor = BackgroundColor,
                Content = Content,
                Dimensions = Dimensions,
                Position = Position,
                TextColor = TextColor,
                ZOrder = ZOrder,
            };
        }
        private Button() { }
        private void SaveState()
        {
            _backup ??= Copy();
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
