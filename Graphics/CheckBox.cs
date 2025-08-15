using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Interfaces;

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
        private Color _checkedColor;
        private Color _uncheckedColor;
        private static Texture2D _pixelTexture;
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
            _checkedColor = checkedColor;
            _uncheckedColor = uncheckedColor;
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
        /// État actuel de la case à cocher (cochée ou non cochée).
        /// </summary>
        public CheckBoxState State { get; set; }

        /// <summary>
        /// Événement déclenché lorsqu'on clique sur la case à cocher. Utile pour réagir immédiatement au changement d'état par l'utilisateur.
        /// </summary>
        public event Action OnClicked;
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
                        OnClicked?.Invoke();
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
            _pixelTexture.SetData([Color.White]);
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

        /// <summary>
        /// Crée une copie de la case à cocher actuelle.
        /// </summary>
        /// <returns>Nouvelle instance de CheckBox avec les mêmes propriétés.</returns>
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
