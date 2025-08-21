using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Events;
using DinaFramework.Extensions;
using DinaFramework.Interfaces;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Classe représentant une case à cocher graphique interactive.
    /// </summary>
    public class CheckBox : Base, IUpdate, IDraw, IVisible, ICopyable<CheckBox>, ILocked
    {
        private Rectangle _checkBoxRect;
        private bool _useTextures;
        private Texture2D _checkedTexture;
        private Texture2D _uncheckedTexture;
        private MouseState _oldMouseState;

        /// <summary>
        /// Initialise une nouvelle instance de la classe CheckBox avec des couleurs.
        /// </summary>
        /// <param name="checkedColor">Couleur de la case cochée.</param>
        /// <param name="uncheckedColor">Couleur de la case non cochée.</param>
        /// <param name="position">Position de la case à cocher.</param>
        /// <param name="dimensions">Dimensions de la case à cocher.</param>
        /// <param name="zorder">Ordre de dessin de la case.</param>
        public CheckBox(Color checkedColor, Color uncheckedColor, Vector2 position, Vector2 dimensions, int zorder = 0) :
            base(position, dimensions, zorder)
        {
            CheckedColor = checkedColor;
            UncheckedColor = uncheckedColor;
            Position = position;
            Initialize();
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe CheckBox avec des textures.
        /// </summary>
        /// <param name="uncheckedTexture">Texture utilisée pour la case non cochée.</param>
        /// <param name="checkedTexture">Texture utilisée pour la case cochée.</param>
        /// <param name="position">Position de la case à cocher.</param>
        /// <param name="dimensions">Dimensions de la case à cocher.</param>
        /// <param name="zorder">Ordre de dessin de la case.</param>
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
            LockedColor = Color.White * 0.75f;
            Visible = true;
            State = CheckBoxState.Unchecked;
        }
        /// <summary>
        /// Position de la case à cocher.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _checkBoxRect = new Rectangle(base.Position.ToPoint(), base.Dimensions.ToPoint());
            }
        }
        /// <summary>
        /// Dimensions de la case à cocher.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _checkBoxRect = new Rectangle(base.Position.ToPoint(), base.Dimensions.ToPoint());
            }
        }

        /// <summary>
        /// Indique si la case à cocher est visible.
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// Indique si la case à cocher est verrouillée.
        /// </summary>
        public bool Locked { get; set; }
        /// <summary>
        /// Couleur à utiliser lorsque la case est verrouillée.
        /// </summary>
        public Color LockedColor { get; set; }
        /// <summary>
        /// État actuel de la case à cocher (cochée ou non cochée).
        /// </summary>
        public CheckBoxState State { get; set; }

        /// <summary>
        /// Événement déclenché lorsqu'on clique sur la case à cocher. Utile pour réagir immédiatement au changement d'état par l'utilisateur.
        /// </summary>
        public event EventHandler<CheckBoxEventArgs> OnClicked;
        /// <summary>
        /// Obtient ou définit l'état coché de la case. True si la case est cochée, false sinon.
        /// Cette propriété simplifie l'accès à l'état en évitant de manipuler l'énumération CheckBoxState directement.
        /// </summary>
        public bool IsChecked
        {
            get => State == CheckBoxState.Checked;
            set => State = value ? CheckBoxState.Checked : CheckBoxState.Unchecked;
        }
        /// <summary>
        /// Couleur quand la case est cochée.
        /// </summary>
        public Color CheckedColor { get; set; } = Color.White;
        /// <summary>
        /// Couleur quand la case est décochée.
        /// </summary>
        public Color UncheckedColor { get; set; } = Color.White;

        /// <summary>
        /// Met à jour l'état de la case à cocher (gestion des clics et des interactions).
        /// </summary>
        /// <param name="gametime">Temps de jeu écoulé depuis la dernière mise à jour.</param>
        public void Update(GameTime gametime)
        {
            MouseState ms = Mouse.GetState();
            if (!Locked)
            {
                if (_oldMouseState.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Released)
                {
                    if (_checkBoxRect.Contains(new Point(ms.X, ms.Y)))
                    {
                        IsChecked = !IsChecked; // on inverse l'état
                        OnClicked?.Invoke(this, new CheckBoxEventArgs(this));
                    }
                }
            }
            _oldMouseState = ms;
        }
        /// <summary>
        /// Dessine la case à cocher sur l'écran.
        /// </summary>
        /// <param name="spritebatch">Objet SpriteBatch utilisé pour dessiner la case.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (Visible)
            {
                if (_useTextures)
                {
                    if (State == CheckBoxState.Checked)
                    {
                        if (_checkedTexture != null)
                            spritebatch.Draw(_checkedTexture, _checkBoxRect, Locked ? LockedColor : CheckedColor);
                    }
                    else
                    {
                        if (_uncheckedTexture != null)
                            spritebatch.Draw(_uncheckedTexture, _checkBoxRect, Locked ? LockedColor : UncheckedColor);
                    }
                }
                else
                {
                    // Dessine un rectangle non plein
                    Texture2D pixel = ServiceLocator.Get<Texture2D>(ServiceKey.Texture1px);
                    if (State == CheckBoxState.Checked)
                        spritebatch.DrawRectangle(pixel, _checkBoxRect, CheckedColor, isFilled: true);
                    else
                        spritebatch.DrawRectangle(pixel, _checkBoxRect, UncheckedColor, isFilled: false);
                }
            }
        }


        /// <summary>
        /// Crée une copie de la case à cocher actuelle.
        /// </summary>
        /// <returns>Nouvelle instance de CheckBox avec les mêmes propriétés.</returns>
        public CheckBox Copy()
        {
            return new CheckBox()
            {
                _checkBoxRect = _checkBoxRect,
                CheckedColor = CheckedColor,
                _checkedTexture = _checkedTexture,
                UncheckedColor = UncheckedColor,
                _uncheckedTexture = _uncheckedTexture,
                _useTextures = _useTextures,
                Dimensions = Dimensions,
                Position = Position,
                State = State,
                Visible = Visible,
                ZOrder = ZOrder,
                Locked = Locked,
                LockedColor = LockedColor,
            };
        }
        private CheckBox() { }
    }
}
