using DinaFramework.Core;
using DinaFramework.Events;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Classe représentant un bouton graphique interactif avec gestion d'état via flags et événements.
    /// </summary>
    public class Button : Base, IUpdate, IDraw, ILocked, IVisible, IClickable<ButtonEventArgs>//, IHovered<ButtonEventArgs>
    {
        private readonly Text? _text;
        private Panel _background;
        private Panel? _hover;
        private Sprite? _lockedSprite;
        private Vector2 _margin = new Vector2(10, 10);
        private Color _lockedColor = Color.Transparent;

        private bool _locked;

        private bool _isHovered;
        private bool _visible;

        /// <summary>
        /// Initialise un nouveau bouton avec texte et fond coloré.
        /// </summary>
        public Button(Vector2 position, Vector2 dimensions, SpriteFont font, string content, Color textColor, Action<Button>? onClick = null,
                      Vector2 margin = default, Action<Button>? onHover = null, bool withroundcorner = false, int cornerradius = 0)
        {
            ArgumentNullException.ThrowIfNull(font);

            _text = new Text(font, content, textColor, horizontalalignment: Enums.HorizontalAlignment.Center, verticalalignment: Enums.VerticalAlignment.Center);
            _margin = margin != default ? margin : _margin;

            Vector2 backgroundDim = _text.TextDimensions + _margin * 2;
            float width = Math.Max(backgroundDim.X, dimensions.X);
            float height = Math.Max(backgroundDim.Y, dimensions.Y);
            Dimensions = new Vector2(width, height);
            Position = position;

            _background ??= new Panel(Position, Dimensions, Color.Transparent, Color.Transparent, 1, withroundcorner, cornerradius);

            LinkPanelEvents();

            RegisterOnClick(onClick);
            RegisterOnHover(onHover);

            UpdateTextPosition();
            _text.Dimensions = Dimensions;
            Visible = true;
        }

        /// <summary>
        /// Initialise un nouveau bouton avec texte et image de fond.
        /// </summary>
        public Button(Vector2 position, Vector2 dimensions, SpriteFont font, string content, Color textColor, Texture2D backgroundImage,
                      Action<Button>? onClick = null, Vector2 margin = default, Action<Button>? onHover = null)
            : this(position, dimensions, font, content, textColor, onClick, margin, onHover)
        {
            _background = new Panel(Position, Dimensions, backgroundImage, 0);
            LinkPanelEvents();
        }

        /// <summary>
        /// Initialise un nouveau bouton avec uniquement une image de fond.
        /// </summary>
        public Button(Vector2 position, Texture2D backgroundImage, Action<Button>? onClick = null, Action<Button>? onHover = null)
        {
            ArgumentNullException.ThrowIfNull(backgroundImage);
            ArgumentNullException.ThrowIfNull(onClick);

            Position = position;
            Dimensions = new Vector2(backgroundImage.Width, backgroundImage.Height);
            _background = new Panel(Position, Dimensions, backgroundImage, 0);
            LinkPanelEvents();
            RegisterOnClick(onClick);
            RegisterOnHover(onHover);
            Visible = true;
        }

        /// <summary>
        /// Position du bouton.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                Vector2 offset = value - base.Position;
                base.Position = value;

                if (_background != null)
                    _background.Position += offset;
                if (_lockedSprite != null)
                    _lockedSprite.Position += offset;
                if (_hover != null)
                    _hover.Position += offset;
                if (_text != null)
                    _text.Position += offset;
            }
        }
        /// <summary>
        /// Dimensions du bouton.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                Vector2 offset = value - base.Dimensions;
                base.Dimensions = value;
                if (_background != null)
                    _background.Dimensions += offset;
                if (_text != null)
                    _text.Dimensions += offset;
                if (_hover != null)
                    _hover.Dimensions += offset;
            }
        }

        /// <summary>
        /// Couleur du texte.
        /// </summary>
        public Color TextColor
        {
            get => _text?.Color ?? Color.White;
            set
            {
                if (_text != null)
                    _text.Color = value;
            }
        }

        /// <summary>
        /// Couleur de fond du bouton.
        /// </summary>
        public Color BackgroundColor
        {
            get => _background?.BackgroundColor ?? Color.Transparent;
            set
            {
                if (_background != null)
                    _background.BackgroundColor = value;
            }
        }

        /// <summary>
        /// Texte affiché sur le bouton.
        /// </summary>
        public string Content
        {
            get => _text?.Content ?? string.Empty;
            set
            {
                if (_text != null)
                    _text.Content = value;
            }
        }

        /// <summary>
        /// Indique si le bouton est verrouillé.
        /// </summary>
        public bool Locked
        {
            get => _locked;
            set => _locked = value;
        }
        /// <summary>
        /// Couleur de la bordure du bouton.
        /// </summary>
        public Color BorderColor
        {
            get => _background?.BorderColor ?? Color.Transparent;
            set
            {
                if (_background != null)
                    _background.BorderColor = value;
            }
        }
        /// <summary>
        /// Épaisseur de la bordure du bouton (par défaut : 0).
        /// </summary>
        public int BorderThickness
        {
            get => _background?.Thickness ?? 0;
            set
            {
                if (_background != null)
                    _background.Thickness = value;
            }
        }
        /// <summary>
        /// Visibilité du bouton.
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }

        /// <summary>
        /// Définit la texture et la couleur affichées quand le bouton est verrouillé.
        /// </summary>
        public void SetLockedImage(Texture2D lockedTexture, Color lockedColor)
        {
            if (lockedTexture != null)
            {
                _lockedSprite ??= new Sprite(lockedTexture, Color.White, Position);

                _lockedSprite.Texture = lockedTexture;
                _lockedSprite.Dimensions = Dimensions;
            }
            else
            {
                _lockedSprite = null;
            }

            _lockedColor = lockedColor;
        }

        /// <summary>
        /// Met à jour l'état du bouton.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (_background == null)
                return;

            if (Locked)
            {
                _isHovered = false;
                return;
            }

            _background.Update(gametime);
        }
        /// <summary>
        /// Dessine le bouton.
        /// </summary>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!_visible)
                return;

            if (_background == null)
                return;

            Color backupColor = _background.BackgroundColor;

            if (Locked && _lockedColor != Color.Transparent)
                _background.BackgroundColor = _lockedColor;

            if (_background.IsHovered() && _hover != null)
                _hover.Draw(spritebatch);
            else
                _background.Draw(spritebatch);

            if (Locked)
            {
                _lockedSprite?.Draw(spritebatch);
                _background.BackgroundColor = backupColor;
            }

            _text?.Draw(spritebatch);
        }

        /// <summary>
        /// Définir une nouvelle image de fond.
        /// </summary>
        public void SetBackground(Texture2D backgroundImage)
        {
            if (backgroundImage == null)
                return;
            _background = new Panel(Position, Dimensions, backgroundImage, 0);
        }
        /// <summary>
        /// Définit les images pour le fond. les images du centre et du milieu pourront être agrandies en hauteur ou largeur.
        /// </summary>
        public void SetBackgroundImages(params Texture2D[] textures)
        {
            ArgumentNullException.ThrowIfNull(textures, nameof(textures));
            _background = CreatePanel(textures);
        }
        /// <summary>
        /// Permet de définir une image lors du survol de la souris.
        /// Si null, on enlève l'image lors du survol.
        /// </summary>
        /// <param name="hoverImage">Image à afficher lors du survol</param>
        /// <param name="dimensions">Dimensions à appliquer pour l'image</param>
        public void SetHoverImage(Texture2D hoverImage, Vector2? dimensions = null)
        {
            if (hoverImage == null)
            {
                _hover = null;
                return;
            }
            if (_hover == null)
                _hover = new Panel(default, dimensions ?? Dimensions, hoverImage, 0);
            else
                _hover.SetImage(hoverImage);
            UpdateHoverImagePosition();
        }
        /// <summary>
        /// Permet de définir les images lors du survol de la souris.
        /// </summary>
        /// <param name="textures">Liste des 9 textures dans cet ordre précis :
        /// Top    Left, Top    Center, Top    Right,
        /// Middle Left, Middle Center, Middle Right,
        /// Bottom Left, Bottom Center, Bottom Right</param>
        public void SetHoverImages(params Texture2D[] textures)
        {
            ArgumentNullException.ThrowIfNull(textures, nameof(textures));
            _hover = CreatePanel(textures);
        }
        /// <summary>
        /// Permet de définir la couleur de fond lors du survol de la souris.
        /// </summary>
        /// <param name="color">Couleur à appliquer lors du survol de la souris.</param>
        public void SetHoverColor(Color color)
        {
            if (_hover != null)
                _hover.BackgroundColor = color;
        }
        /// <summary>
        /// Événement déclenché quand le bouton est cliqué.
        /// </summary>
        public event EventHandler<ButtonEventArgs>? OnClicked;


        /// <summary>
        /// Événement déclenché quand le bouton est survolé.
        /// </summary>
        public event EventHandler<ButtonEventArgs>? OnHovered;

        private void UpdateTextPosition()
        {
            if (_text == null || _background == null)
                return;
            _text.Position = _background.Position;
        }
        private void UpdateHoverImagePosition()
        {
            if (_hover == null)
                return;
            Vector2 offset = Dimensions - _hover.Dimensions;
            _hover.Position = Position + offset / 2;
        }
        private Panel CreatePanel(Texture2D[] textures)
        {
            if (textures.Length != 9)
                throw new ArgumentException("Nine textures required for a 9-slice panel.");

            return new Panel(Position, Dimensions,
                textures[0], textures[1], textures[2],
                textures[3], textures[4], textures[5],
                textures[6], textures[7], textures[8]);
        }
        private void RegisterOnClick(Action<Button>? onClick)
        {
            if (onClick != null)
                OnClicked += (sender, e) => onClick(e.Button);
        }
        private void RegisterOnHover(Action<Button>? onHover)
        {
            if (onHover != null)
                OnHovered += (sender, e) => onHover(e.Button);
        }
        private void LinkPanelEvents()
        {
            if (_background == null)
                return;
            _background.OnHovered += (s, e) => OnHovered?.Invoke(this, new ButtonEventArgs(this));
            _background.OnClicked += (s, e) => OnClicked?.Invoke(this, new ButtonEventArgs(this));
        }

        /// <summary>
        /// Indique si le bouton a été cliqué.
        /// </summary>
        /// <returns></returns>
        public bool IsClicked()
        {
            return _background.IsClicked();
        }
        /// <summary>
        /// Indique si le bouton est survolé.
        /// </summary>
        /// <returns></returns>
        public bool IsHovered()
        {
            return _background.IsHovered();
        }

    }
}
