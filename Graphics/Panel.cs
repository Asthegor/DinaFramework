using DinaFramework.Core;
using DinaFramework.Events;
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
        private Dictionary<string, object> _originalValues = [];
        private List<string> _modifiedHoverValues = [];
        private List<string> _modifiedClickValues = [];
        private bool _hoverOriginalSaved;

        private List<Vector2> _positions = [];
        private List<Texture2D> _images = [];
        private Texture2D? _texture;
        private Rectangle _rectangleBackground;
        private Rectangle _rectangleBorder;
        private int _thickness;
        private Color _borderColor;
        private bool _visible;
        private MouseState _oldMouseState;
        private bool _leftClicked;
        private bool _rightClicked;
        private bool _hover;
        private bool _hoverInvoked;
        private readonly bool _withRoundCorner;
        private readonly int _radiusCorner;

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
        /// <param name="topLeft">Texture du coin supérieur gauche.</param>
        /// <param name="topCenter">Texture du bord supérieur.</param>
        /// <param name="topRight">Texture du coin supérieur droit.</param>
        /// <param name="middleRight">Texture du bord droit.</param>
        /// <param name="bottomRight">Texture du coin inférieur droit.</param>
        /// <param name="bottomCenter">Texture du bord inférieur.</param>
        /// <param name="bottomLeft">Texture du coin inférieur gauche.</param>
        /// <param name="middleLeft">Texture du bord gauche.</param>
        /// <param name="middleCenter">Texture du centre du panneau.</param>
        /// <param name="zorder">L'ordre Z du panneau (par défaut 0).</param>
        public Panel(Vector2 position, Vector2 dimensions, Texture2D topLeft, Texture2D topCenter, Texture2D topRight, Texture2D middleLeft, Texture2D middleCenter, Texture2D middleRight, Texture2D bottomLeft, Texture2D bottomCenter, Texture2D bottomRight, int zorder = 0) : base(position, dimensions, zorder)
        {
            ArgumentNullException.ThrowIfNull(topLeft);
            ArgumentNullException.ThrowIfNull(topCenter);
            ArgumentNullException.ThrowIfNull(topRight);
            ArgumentNullException.ThrowIfNull(middleLeft);
            ArgumentNullException.ThrowIfNull(middleCenter);
            ArgumentNullException.ThrowIfNull(middleRight);
            ArgumentNullException.ThrowIfNull(bottomLeft);
            ArgumentNullException.ThrowIfNull(bottomCenter);
            ArgumentNullException.ThrowIfNull(bottomRight);

            BackgroundColor = Color.White;

            _images.Add(topLeft);
            _positions.Add(position);
            _images.Add(topCenter);
            _positions.Add(new Vector2(position.X + topLeft.Width, position.Y));
            _images.Add(topRight);
            _positions.Add(new Vector2(position.X + dimensions.X - topRight.Width, position.Y));

            _images.Add(middleLeft);
            _positions.Add(new Vector2(position.X, position.Y + topLeft.Height));
            _images.Add(middleCenter);
            _positions.Add(new Vector2(position.X + bottomLeft.Width, position.Y + bottomLeft.Height));
            _images.Add(middleRight);
            _positions.Add(new Vector2(position.X + dimensions.X - middleRight.Width, position.Y + topRight.Height));

            _images.Add(bottomLeft);
            _positions.Add(new Vector2(position.X, position.Y + dimensions.Y - bottomLeft.Height));
            _images.Add(bottomCenter);
            _positions.Add(new Vector2(position.X + bottomLeft.Width, position.Y + dimensions.Y - bottomCenter.Height));
            _images.Add(bottomRight);
            _positions.Add(new Vector2(position.X + dimensions.X - bottomRight.Width, position.Y + dimensions.Y - bottomRight.Height));

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
        /// Redéfinit l'image du panneau
        /// </summary>
        /// <param name="image">Nouvelle image à afficher</param>
        public void SetImage(Texture2D image)
        {
            ArgumentNullException.ThrowIfNull(image, nameof(image));
            _images[0] = image;
        }
        /// <summary>
        /// Redéfinit les images du panneau.
        /// </summary>
        /// <param name="topLeft"></param>
        /// <param name="topCenter"></param>
        /// <param name="topRight"></param>
        /// <param name="middleLeft"></param>
        /// <param name="middleCenter"></param>
        /// <param name="middleRight"></param>
        /// <param name="bottomLeft"></param>
        /// <param name="bottomCenter"></param>
        /// <param name="bottomRight"></param>
        public void SetImages(Texture2D topLeft, Texture2D topCenter, Texture2D topRight,
                              Texture2D middleLeft, Texture2D middleCenter, Texture2D middleRight,
                              Texture2D bottomLeft, Texture2D bottomCenter, Texture2D bottomRight)
        {
            ArgumentNullException.ThrowIfNull(topLeft, nameof(topLeft));
            ArgumentNullException.ThrowIfNull(topCenter, nameof(topCenter));
            ArgumentNullException.ThrowIfNull(topRight, nameof(topRight));
            ArgumentNullException.ThrowIfNull(middleLeft, nameof(middleLeft));
            ArgumentNullException.ThrowIfNull(middleCenter, nameof(middleCenter));
            ArgumentNullException.ThrowIfNull(middleRight, nameof(middleRight));
            ArgumentNullException.ThrowIfNull(bottomLeft, nameof(bottomLeft));
            ArgumentNullException.ThrowIfNull(bottomCenter, nameof(bottomCenter));
            ArgumentNullException.ThrowIfNull(bottomRight, nameof(bottomRight));

            _images[0] = topLeft;
            _images[1] = topCenter;
            _images[2] = topRight;
            _images[3] = middleLeft;
            _images[4] = middleCenter;
            _images[5] = middleRight;
            _images[6] = bottomLeft;
            _images[7] = bottomCenter;
            _images[8] = bottomRight;
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
                        Texture2D? texture = ServiceLocator.Get<Texture2D>(ServiceKeys.Texture1px)
                            ?? throw new InvalidOperationException("Texture1px non enregistrée dans le ServiceLocator");

                        if (_withRoundCorner)
                        {
                            Point pos = Position.ToPoint();
                            Point dim = Dimensions.ToPoint();
                            Rectangle rect1 = new Rectangle(new Point((int)pos.X + _radiusCorner, pos.Y), new Point(dim.X - _radiusCorner * 2, dim.Y));
                            spritebatch.Draw(texture, rect1, BackgroundColor);
                            Rectangle rect2 = new Rectangle(new Point((int)pos.X, pos.Y + _radiusCorner), new Point(dim.X, dim.Y - _radiusCorner * 2));
                            spritebatch.Draw(texture, rect2, BackgroundColor);
                            spritebatch.MaskCorners(Position, Dimensions, _radiusCorner, BackgroundColor);

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

            bool hoveredNow = _rectangleBackground.Contains(currentMouseState.Position);
            if (hoveredNow)
            {
                _hover = true;
                if (!_hoverOriginalSaved)
                {
                    _originalValues = SaveValues();
                    _hoverOriginalSaved = true;
                }
                if (!_hoverInvoked)
                {
                    OnHovered?.Invoke(this, new PanelEventArgs(this));
                    _hoverInvoked = true;
                    _modifiedHoverValues = [.. _originalValues.GetModifiedKeys(SaveValues())];
                }

                // Vérifie si le clic a eu lieu
                _leftClicked = _hover && currentMouseState.LeftButton == ButtonState.Released && _oldMouseState.LeftButton == ButtonState.Pressed;
                if (_leftClicked)
                {
                    var beforeClickValues = SaveValues();
                    OnClicked?.Invoke(this, new PanelEventArgs(this));
                    _modifiedClickValues = [.. beforeClickValues.GetModifiedKeys(SaveValues())];

                    _modifiedHoverValues.RemoveAll(k => _modifiedClickValues.Contains(k));
                }

                _rightClicked = _hover && currentMouseState.RightButton == ButtonState.Released && _oldMouseState.RightButton == ButtonState.Pressed;
                if (_rightClicked)
                    OnRightClicked?.Invoke(this, new PanelEventArgs(this));
            }
            else
            {
                _leftClicked = false;
                _hover = false;
                if (_hoverOriginalSaved)
                {
                    var permanentState = SaveValues();
                    RestoreOriginalValues(_modifiedHoverValues);
                    ApplyValues(permanentState, _modifiedClickValues);

                    _hoverOriginalSaved = false;
                    _hoverInvoked = false;
                    _modifiedHoverValues.Clear();
                }
            }
            _oldMouseState = currentMouseState;
        }
        private void ApplyValues(Dictionary<string, object> reference, List<string> keys)
        {
            foreach (var key in keys)
            {
                if (reference.TryGetValue(key, out var value))
                {
                    PropertyInfo? prop = GetType().GetProperty(key);
                    if (prop != null)
                        prop.SetValue(this, value);
                }
            }
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
        public event EventHandler<PanelEventArgs>? OnHovered;
        /// <summary>
        /// Déclenche les événements lorsque le panneau est cliqué avec le bouton gauche.
        /// </summary>
        public event EventHandler<PanelEventArgs>? OnClicked;
        /// <summary>
        /// Déclenche les événements lorsque le panneau est cliqué avec le bouton gauche.
        /// </summary>
        public event EventHandler<PanelEventArgs>? OnRightClicked;

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
            return new Panel(Position, Dimensions, BackgroundColor, ZOrder)
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
                BorderColor = BorderColor,
                Thickness = Thickness,
            };
        }

        private Dictionary<string, object> SaveValues()
        {
            Dictionary<string, object> values = [];
            // Récupère toutes les propriétés publiques du Panel et les enregistre dans le dictionnaire.
            Type type = this.GetType();
            PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            foreach (PropertyInfo property in properties)
            {
                values[property.Name] = property.GetValue(this)!;
            }
            return values;
        }
        private void RestoreOriginalValues(List<string>? modifiedKeys = null)
        {
            if (modifiedKeys == null || modifiedKeys.Count == 0)
                return;
            foreach (string key in modifiedKeys)
            {
                if (_originalValues.ContainsKey(key))
                {
                    PropertyInfo? property = GetType().GetProperty(key);
                    if (property != null)
                    {
                        property.SetValue(this, _originalValues[key]);
                    }
                }
            }
        }

    }
}
