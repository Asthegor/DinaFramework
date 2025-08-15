using DinaFramework.Core;
using DinaFramework.Extensions;
using DinaFramework.Interfaces;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente un panneau graphique pouvant être dessiné et interactif.
    /// </summary>
    public class Panel : Base, IClickable, IColor, IDraw, IUpdate, IVisible, ICopyable<Panel>
    {
        private Dictionary<string, object> _originalValues = new Dictionary<string, object>();
        private List<string> _modifiedValues = [];
        private bool hasBeenRestored;
        private bool saveOriginalValueCalled;
        private bool saveCalled;

        private List<Vector2> _positions = [];
        private List<Texture2D> _images = [];
        private Texture2D _texture;
        private Rectangle _rectangleBackground;
        private Rectangle _rectangleBorder;
        private int _thickness;
        private Color _borderColor;
        private bool _visible;
        private MouseState _oldMouseState;
        private bool _leftClicked;
        private bool _rightClicked;
        private bool _hover;
        private bool _withRoundCorner;
        private int _radiusCorner;

        private Panel() { } // ne sert qu'à la copie d'une instance
        /// <summary>
        /// Initialise une nouvelle instance de la classe Panel avec des paramètres par défaut.
        /// </summary>
        /// <param name="position">Position du panneau.</param>
        /// <param name="dimensions">Dimensions du panneau.</param>
        /// <param name="backgroundcolor">Couleur d'arrière-plan.</param>
        /// <param name="zorder">Ordre de superposition (facultatif).</param>
        public Panel(Vector2 position, Vector2 dimensions, Color backgroundcolor, int zorder = 0) :
            base(position, dimensions, zorder)
        {
            BackgroundColor = backgroundcolor;
            BorderColor = backgroundcolor;
            Thickness = 0;
            CheckVisibility();
            _oldMouseState = Mouse.GetState();
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Panel avec une bordure spécifiée.
        /// </summary>
        /// <param name="position">Position du panneau.</param>
        /// <param name="dimensions">Dimensions du panneau.</param>
        /// <param name="backgroundcolor">Couleur d'arrière-plan.</param>
        /// <param name="bordercolor">Couleur de la bordure.</param>
        /// <param name="thickness">Épaisseur de la bordure.</param>
        /// <param name="withroundcorner"></param>
        /// <param name="radius"></param>
        /// <param name="zorder">Ordre de superposition (facultatif).</param>
        public Panel(Vector2 position, Vector2 dimensions, Color backgroundcolor, Color bordercolor, int thickness, bool withroundcorner = false, int radius = 0, int zorder = 0) :
            this(position, dimensions, backgroundcolor, zorder)
        {
            BorderColor = bordercolor;
            Thickness = thickness;
            CheckVisibility();
            _withRoundCorner = withroundcorner;
            _radiusCorner = radius;
            _oldMouseState = Mouse.GetState();
        }
        /// <summary>
        /// Initialise un nouveau panneau avec une image de fond et une épaisseur de bordure.
        /// </summary>
        /// <param name="position">La position du panneau.</param>
        /// <param name="dimensions">Les dimensions du panneau.</param>
        /// <param name="image">L'image de fond du panneau.</param>
        /// <param name="borderThickness">L'épaisseur de la bordure.</param>
        /// <param name="withroundcorner">Indique si on veut des coins arrondis.</param>
        /// <param name="radius">Rayon de l'arrondi.</param>
        /// <param name="zorder">L'ordre Z du panneau (par défaut 0).</param>
        public Panel(Vector2 position, Vector2 dimensions, Texture2D image, int borderThickness, bool withroundcorner = false, int radius = 0, int zorder = 0) :
            base(position, dimensions, zorder)
        {
            ArgumentNullException.ThrowIfNull(image);

            BackgroundColor = Color.White;
            _images.Add(image);
            _positions.Add(position);
            _thickness = borderThickness;
            if (Dimensions == default)
                Dimensions = new Vector2(image.Width, image.Height);
            CheckVisibility();
            _withRoundCorner = withroundcorner;
            _radiusCorner = radius;
            _oldMouseState = Mouse.GetState();
        }
        /// <summary>
        /// Initialise un panneau avec une combinaison de textures pour les coins, bords et centre du panneau.
        /// </summary>
        /// <param name="position">La position du panneau.</param>
        /// <param name="dimensions">Les dimensions du panneau.</param>
        /// <param name="cornerTopLeft">Texture du coin supérieur gauche.</param>
        /// <param name="top">Texture du bord supérieur.</param>
        /// <param name="cornerTopRight">Texture du coin supérieur droit.</param>
        /// <param name="right">Texture du bord droit.</param>
        /// <param name="cornerBottomRight">Texture du coin inférieur droit.</param>
        /// <param name="bottom">Texture du bord inférieur.</param>
        /// <param name="cornerBottomLeft">Texture du coin inférieur gauche.</param>
        /// <param name="left">Texture du bord gauche.</param>
        /// <param name="center">Texture du centre du panneau.</param>
        /// <param name="zorder">L'ordre Z du panneau (par défaut 0).</param>
        public Panel(Vector2 position, Vector2 dimensions, Texture2D cornerTopLeft, Texture2D top, Texture2D cornerTopRight, Texture2D right, Texture2D cornerBottomRight, Texture2D bottom, Texture2D cornerBottomLeft, Texture2D left, Texture2D center, int zorder = 0) : base(position, dimensions, zorder)
        {
            ArgumentNullException.ThrowIfNull(cornerTopLeft);
            ArgumentNullException.ThrowIfNull(top);
            ArgumentNullException.ThrowIfNull(cornerTopRight);
            ArgumentNullException.ThrowIfNull(right);
            ArgumentNullException.ThrowIfNull(cornerBottomRight);
            ArgumentNullException.ThrowIfNull(bottom);
            ArgumentNullException.ThrowIfNull(cornerBottomLeft);
            ArgumentNullException.ThrowIfNull(left);
            ArgumentNullException.ThrowIfNull(center);

            BackgroundColor = Color.White;
            _images.Add(cornerTopLeft);
            _positions.Add(position);

            _images.Add(top);
            _positions.Add(new Vector2(position.X + cornerTopLeft.Width, position.Y));

            _images.Add(cornerTopRight);
            _positions.Add(new Vector2(position.X + dimensions.X - cornerTopRight.Width, position.Y));

            _images.Add(right);
            _positions.Add(new Vector2(position.X + dimensions.X - right.Width, position.Y + cornerTopRight.Height));

            _images.Add(cornerBottomRight);
            _positions.Add(new Vector2(position.X + dimensions.X - cornerBottomRight.Width, position.Y + dimensions.Y - cornerBottomRight.Height));

            _images.Add(bottom);
            _positions.Add(new Vector2(position.X + cornerBottomLeft.Width, position.Y + dimensions.Y - bottom.Height));

            _images.Add(cornerBottomLeft);
            _positions.Add(new Vector2(position.X, position.Y + dimensions.Y - cornerBottomLeft.Height));

            _images.Add(left);
            _positions.Add(new Vector2(position.X, position.Y + cornerTopLeft.Height));

            _images.Add(center);
            _positions.Add(new Vector2(position.X + cornerBottomLeft.Width, position.Y + cornerBottomLeft.Height));

            CheckVisibility();
            _oldMouseState = Mouse.GetState();
        }
        /// <summary>
        /// Position du panneau.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                Vector2 offset = value - base.Position;
                for (int index = 0; index < _positions.Count; index++)
                    _positions[index] = new Vector2(_positions[index].X + offset.X, _positions[index].Y + offset.Y);
                base.Position = value;
                AdjustRectangles();
            }

        }
        /// <summary>
        /// Dimensions du panneau.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                AdjustRectangles();
            }
        }
        /// <summary>
        /// Obtient ou définit la couleur d'arrière-plan du panneau.
        /// </summary>
        public Color BackgroundColor { get; set; }
        /// <summary>
        /// Obtient ou définit la couleur de la bordure du panneau.
        /// </summary>
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                AdjustRectangles();
            }
        }
        /// <summary>
        /// Obtient ou définit l'épaisseur de la bordure.
        /// </summary>
        public int Thickness
        {
            get { return _thickness; }
            set
            {
                _thickness = value;
                AdjustRectangles();
            }
        }
        private void SetBackgroundRectangle()
        {
            _rectangleBackground = new Rectangle(Convert.ToInt32(Position.X), Convert.ToInt32(Position.Y), Convert.ToInt32(Dimensions.X), Convert.ToInt32(Dimensions.Y));
        }
        private void AdjustRectangles()
        {
            SetBackgroundRectangle();
            if (_thickness > 0 && BorderColor != BackgroundColor)
            {
                _rectangleBorder = new Rectangle(Convert.ToInt32(Position.X - _thickness), Convert.ToInt32(Position.Y - _thickness), Convert.ToInt32(Dimensions.X + _thickness * 2), Convert.ToInt32(Dimensions.Y + _thickness * 2));
                _rectangleBackground.Location += new Point(Convert.ToInt32(Math.Ceiling(_thickness / 2.0f)), Convert.ToInt32(Math.Ceiling(_thickness / 2.0f)));
                _rectangleBackground.Size -= new Point(_thickness, _thickness);
            }
        }
        private void CheckVisibility()
        {
            if (Dimensions != default)
                Visible = true;
        }
        /// <summary>
        /// Obtient ou définit une valeur indiquant si le panneau est visible.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// Obtient ou définit la couleur associée au panneau (mappée sur BackgroundColor).
        /// </summary>
        public Color Color
        {
            get { return BackgroundColor; }
            set { BackgroundColor = value; }
        }
        /// <summary>
        /// Dessine le panneau à l'aide d'un SpriteBatch.
        /// </summary>
        /// <param name="spritebatch">Instance de SpriteBatch utilisée pour le rendu.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (Visible && Dimensions != default)
            {
                switch (_images.Count)
                {
                    case 0:
                        Texture2D texture = ServiceLocator.Get<Texture2D>(ServiceKey.Texture1px);

                        if (_withRoundCorner)
                        {
                            Point pos = Position.ToPoint();
                            Point dim = Dimensions.ToPoint();
                            Rectangle rect1 = new Rectangle(new Point((int)pos.X + _radiusCorner, pos.Y), new Point(dim.X - _radiusCorner * 2, dim.Y));
                            spritebatch.Draw(texture, rect1, BackgroundColor);
                            Rectangle rect2 = new Rectangle(new Point((int)pos.X, pos.Y + _radiusCorner), new Point(dim.X, dim.Y - _radiusCorner * 2));
                            spritebatch.Draw(texture, rect2, BackgroundColor);
                            spritebatch.MaskCorners(Position, Dimensions, _radiusCorner, BackgroundColor);

                            //// Dessin des arcs pour les coins arrondis
                            //Point dim = Dimensions.ToPoint();
                            ////dim -= new Point(_thickness * 2, _thickness * 2);
                            //spritebatch.DrawArc(BorderColor, new Rectangle(Position.ToPoint(), dim), _radiusCorner, 90, 270);
                            //spritebatch.DrawArc(BorderColor, new Rectangle(Position.ToPoint(), dim), _radiusCorner, 180, 90);
                            //spritebatch.DrawArc(BorderColor, new Rectangle(Position.ToPoint(), dim), _radiusCorner, 270, 180);
                            //spritebatch.DrawArc(BorderColor, new Rectangle(Position.ToPoint(), dim), _radiusCorner, 0, 90);

                            //// Dessin des lignes entre les arcs
                            //spritebatch.DrawLine(texture, BorderColor, Position + new Vector2(_radiusCorner - _thickness, 0), Position + new Vector2(Dimensions.X - _radiusCorner, 0), _thickness);
                            //spritebatch.DrawLine(texture, BorderColor, Position + new Vector2(Dimensions.X - _thickness, _radiusCorner - _thickness), Position + Dimensions - new Vector2(_thickness, _radiusCorner), _thickness);
                            //spritebatch.DrawLine(texture, BorderColor, Position + new Vector2(_radiusCorner - _thickness, Dimensions.Y - _thickness), Position + Dimensions - new Vector2(_radiusCorner, _thickness), _thickness);
                            //spritebatch.DrawLine(texture, BorderColor, Position + new Vector2(0, _radiusCorner - _thickness), Position + new Vector2(0, Dimensions.Y - _radiusCorner), _thickness);

                            // Dessin du rectangle intérieur (texture ou couleur de fond)
                            Rectangle innerRect = new Rectangle((int)Position.X + _thickness / 2, (int)Position.Y + _thickness, (int)(Dimensions.X - 2 * _thickness), (int)(Dimensions.Y - 2 * _thickness));
                            spritebatch.Draw(texture, innerRect, BackgroundColor);
                        }
                        else
                        {
                            spritebatch.Draw(texture, _rectangleBackground, BackgroundColor);
                            if (_thickness > 0 && BorderColor != BackgroundColor)
                                spritebatch.DrawRectangle(texture, _rectangleBackground, BorderColor, _thickness);
                        }

                        break;
                    case 1:
                        if (_thickness == 0)
                        {
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y), Convert.ToInt32(Dimensions.X), Convert.ToInt32(Dimensions.Y)),
                                             new Rectangle(0, 0, _images[0].Width, _images[0].Height),
                                             BackgroundColor);
                        }
                        else
                        {
                            var img = _images[0];
                            int x = (int)_positions[0].X;
                            int y = (int)_positions[0].Y;
                            int w = (int)Dimensions.X;
                            int h = (int)Dimensions.Y;
                            int iw = img.Width;
                            int ih = img.Height;
                            int t = _thickness;

                            // Top Left
                            spritebatch.Draw(img, new Rectangle(x, y, t, t), new Rectangle(0, 0, t, t), BackgroundColor);

                            // Top
                            spritebatch.Draw(img, new Rectangle(x + t, y, w - 2 * t, t), new Rectangle(t, 0, iw - 2 * t, t), BackgroundColor);

                            // Top Right
                            spritebatch.Draw(img, new Rectangle(x + w - t, y, t, t), new Rectangle(iw - t, 0, t, t), BackgroundColor);

                            // Right
                            spritebatch.Draw(img, new Rectangle(x + w - t, y + t, t, h - 2 * t), new Rectangle(iw - t, t, t, ih - 2 * t), BackgroundColor);

                            // Bottom Right
                            spritebatch.Draw(img, new Rectangle(x + w - t, y + h - t, t, t), new Rectangle(iw - t, ih - t, t, t), BackgroundColor);

                            // Bottom
                            spritebatch.Draw(img, new Rectangle(x + t, y + h - t, w - 2 * t, t), new Rectangle(t, ih - t, iw - 2 * t, t), BackgroundColor);

                            // Bottom Left
                            spritebatch.Draw(img, new Rectangle(x, y + h - t, t, t), new Rectangle(0, ih - t, t, t), BackgroundColor);

                            // Left
                            spritebatch.Draw(img, new Rectangle(x, y + t, t, h - 2 * t), new Rectangle(0, t, t, ih - 2 * t), BackgroundColor);

                            // Center
                            spritebatch.Draw(img, new Rectangle(x + t, y + t, w - 2 * t, h - 2 * t), new Rectangle(t, t, iw - 2 * t, ih - 2 * t), BackgroundColor);
                        }
                        break;
                    case 9:
                        // Corner Top Left
                        spritebatch.Draw(_images[0], _positions[0], BackgroundColor);
                        // Top
                        spritebatch.Draw(_images[1],
                                         new Rectangle(Convert.ToInt32(_positions[1].X),
                                                       Convert.ToInt32(_positions[1].Y),
                                                       Convert.ToInt32(Dimensions.X - _images[0].Width - _images[2].Width),
                                                       _images[1].Height),
                                         new Rectangle(0, 0, _images[1].Width, _images[1].Height),
                                         BackgroundColor);
                        // Corner Top Right
                        spritebatch.Draw(_images[2], _positions[2], BackgroundColor);
                        // Right
                        spritebatch.Draw(_images[3],
                                         new Rectangle(Convert.ToInt32(_positions[3].X),
                                                       Convert.ToInt32(_positions[3].Y),
                                                       Convert.ToInt32(_images[3].Width),
                                                       Convert.ToInt32(Dimensions.Y - _images[2].Height - _images[4].Height)),
                                         new Rectangle(0, 0, _images[3].Width, _images[3].Height),
                                         BackgroundColor);
                        // Corner Bottom Right
                        spritebatch.Draw(_images[4], _positions[4], BackgroundColor);
                        // Bottom
                        spritebatch.Draw(_images[5],
                                         new Rectangle(Convert.ToInt32(_positions[5].X),
                                                       Convert.ToInt32(_positions[5].Y),
                                                       Convert.ToInt32(Dimensions.X - _images[4].Width - _images[6].Width),
                                                       _images[5].Height),
                                         new Rectangle(0, 0, _images[5].Width, _images[5].Height),
                                         BackgroundColor);
                        // Corner Bottom Left
                        spritebatch.Draw(_images[6], _positions[6], BackgroundColor);
                        // Left
                        spritebatch.Draw(_images[7],
                                         new Rectangle(Convert.ToInt32(_positions[7].X),
                                                       Convert.ToInt32(_positions[7].Y),
                                                       Convert.ToInt32(_images[7].Width),
                                                       Convert.ToInt32(Dimensions.Y - _images[0].Height - _images[6].Height)),
                                         new Rectangle(0, 0, _images[7].Width, _images[7].Height),
                                         BackgroundColor);
                        // Center
                        spritebatch.Draw(_images[8],
                                         new Rectangle(Convert.ToInt32(_positions[8].X),
                                                       Convert.ToInt32(_positions[8].Y),
                                                       Convert.ToInt32(Dimensions.X - _images[0].Width - _images[2].Width),
                                                       Convert.ToInt32(Dimensions.Y - _images[0].Height - _images[6].Height)),
                                         new Rectangle(0, 0, _images[8].Width, _images[8].Height),
                                         BackgroundColor);
                        break;
                    default:
                        throw new InvalidEnumArgumentException("Images missing for the obj.");
                }
            }
        }

        /// <summary>
        /// Met à jour l'état du panneau en fonction des interactions utilisateur.
        /// </summary>
        /// <param name="gametime">Temps de jeu actuel.</param>
        public void Update(GameTime gametime)
        {
            MouseState currentMouseState = Mouse.GetState();

            _hover = _rectangleBackground.Contains(currentMouseState.Position);
            if (_hover)
            {
                saveCalled = false;
                if (!saveOriginalValueCalled)
                {
                    _originalValues = SaveValues();
                    saveOriginalValueCalled = true;
                    saveCalled = true;
                }
                OnHovered?.Invoke(this);
                if (saveCalled)
                {
                    _modifiedValues.Clear();
                    // Compare les valeurs originales avec les valeurs modifiées
                    _modifiedValues = _originalValues.GetModifiedKeys(SaveValues());
                }
                hasBeenRestored = false;
            }
            else
            {
                if (!hasBeenRestored)
                {
                    RestoreOriginalValues(_modifiedValues);
                    hasBeenRestored = true;
                    saveOriginalValueCalled = false;
                }
            }

            // Vérifie si le clic a eu lieu
            _leftClicked = _hover && currentMouseState.LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed;
            if (_leftClicked)
                OnClicked?.Invoke(this);

            _rightClicked = _hover && currentMouseState.RightButton == ButtonState.Released && _oldMouseState.RightButton == ButtonState.Pressed;
            if (_rightClicked)
                OnRightClicked?.Invoke(this);

            _oldMouseState = currentMouseState;
        }
        /// <summary>
        /// Détermine si le panneau a été cliqué (clic droit ou gauche).
        /// </summary>
        /// <returns>True si cliqué, sinon false.</returns>
        public bool IsClicked() => _leftClicked || _rightClicked;
        /// <summary>
        /// Détermine si le panneau a été cliqué avec le bouton gauche.
        /// </summary>
        /// <returns>True si cliqué, sinon false.</returns>
        public bool IsLeftClicked() => _leftClicked;
        /// <summary>
        /// Détermine si le panneau a été cliqué avec le bouton droit.
        /// </summary>
        /// <returns>True si cliqué, sinon false.</returns>
        public bool IsRightClicked() => _rightClicked;
        /// <summary>
        /// Détermine si le panneau est survolé par la souris.
        /// </summary>
        /// <returns>True si survolé, sinon false.</returns>
        public bool IsHovered() => _hover;

        /// <summary>
        /// Déclenche les événements lorsque le panneau est survolé.
        /// </summary>
        public event Action<Panel> OnHovered;
        /// <summary>
        /// Déclenche les événements lorsque le panneau est cliqué avec le bouton gauche.
        /// </summary>
        public event Action<Panel> OnClicked;
        /// <summary>
        /// Déclenche les événements lorsque le panneau est cliqué avec le bouton gauche.
        /// </summary>
        public event Action<Panel> OnRightClicked;

        internal void SetVisible(bool visible)
        {
            _visible = visible;
        }
        /// <summary>
        /// Crée une copie du panneau actuel.
        /// </summary>
        /// <returns>Une nouvelle instance de Panel avec les mêmes propriétés.</returns>
        public Panel Copy()
        {
            List<Texture2D> copiedTextures = [];
            foreach (Texture2D texture in _images)
                copiedTextures.Add(texture);
            return new Panel()
            {
                _borderColor = _borderColor,
                _leftClicked = _leftClicked,
                _rightClicked = _rightClicked,
                _images = copiedTextures,
                _positions = [.. _positions],
                _oldMouseState = _oldMouseState,
                _rectangleBackground = _rectangleBackground,
                _texture = _texture,
                _rectangleBorder = _rectangleBorder,
                _thickness = _thickness,
                _visible = _visible,
                BackgroundColor = BackgroundColor,
                BorderColor = BorderColor,
                Dimensions = Dimensions,
                Position = Position,
                Thickness = Thickness,
                ZOrder = ZOrder
            };
        }

        private Dictionary<string, object> SaveValues()
        {
            Dictionary<string, object> values = new Dictionary<string, object>();
            // Récupère toutes les propriétés publiques du Panel et les enregistre dans le dictionnaire.
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                values[property.Name] = property.GetValue(this);
            }
            return values;
        }
        private void RestoreOriginalValues(List<string> modifiedKeys = default)
        {
            if (modifiedKeys.Count == 0)
            {
                foreach (KeyValuePair<string, object> pair in _originalValues)
                {
                    // Utilise la réflexion pour définir la valeur de la propriété correspondante.
                    PropertyInfo property = this.GetType().GetProperty(pair.Key);
                    if (property != null)
                    {
                        property.SetValue(this, pair.Value);
                    }
                }
            }
            else
            {
                foreach (string key in modifiedKeys)
                {
                    if (_originalValues.ContainsKey(key))
                    {
                        PropertyInfo property = this.GetType().GetProperty(key);
                        if (property != null)
                        {
                            property.SetValue(this, _originalValues[key]);
                        }
                    }
                }
            }
        }

    }
}
