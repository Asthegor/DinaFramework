using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente un panneau graphique pouvant être dessiné et interactif.
    /// </summary>
    public class Panel : Base, IClickable, IColor, IDraw, IUpdate, IVisible, ICopyable<Panel>
    {
        private List<Vector2> _positions = new List<Vector2>();
        private List<Texture2D> _images = new List<Texture2D>();
        private Texture2D _texture;
        private Rectangle _rectangleBackground;
        private Rectangle _rectangleBorder;
        private int _thickness;
        private Color _borderColor;
        private bool _visible;
        private MouseState _oldMouseState;
        private bool _clicked;
        private bool _hover;
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
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Panel avec une bordure spécifiée.
        /// </summary>
        /// <param name="position">Position du panneau.</param>
        /// <param name="dimensions">Dimensions du panneau.</param>
        /// <param name="backgroundcolor">Couleur d'arrière-plan.</param>
        /// <param name="bordercolor">Couleur de la bordure.</param>
        /// <param name="thickness">Épaisseur de la bordure.</param>
        /// <param name="zorder">Ordre de superposition (facultatif).</param>
        public Panel(Vector2 position, Vector2 dimensions, Color backgroundcolor, Color bordercolor, int thickness, int zorder = 0) :
            this(position, dimensions, backgroundcolor, zorder)
        {
            BorderColor = bordercolor;
            Thickness = thickness;
            CheckVisibility();
        }
        /// <summary>
        /// Initialise un nouveau panneau avec une image de fond et une épaisseur de bordure.
        /// </summary>
        /// <param name="position">La position du panneau.</param>
        /// <param name="dimensions">Les dimensions du panneau.</param>
        /// <param name="image">L'image de fond du panneau.</param>
        /// <param name="borderThickness">L'épaisseur de la bordure.</param>
        /// <param name="zorder">L'ordre Z du panneau (par défaut 0).</param>
        public Panel(Vector2 position, Vector2 dimensions, Texture2D image, int borderThickness, int zorder = 0) : 
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
                        _texture ??= new Texture2D(spritebatch.GraphicsDevice, 1, 1);
                        _texture.SetData(new[] { Color.White });
                        if (_thickness > 0 && BorderColor != BackgroundColor)
                            spritebatch.Draw(_texture, _rectangleBorder, null, BorderColor);
                        spritebatch.Draw(_texture, _rectangleBackground, null, BackgroundColor);
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
                            // Corner Top Left
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y), _thickness, _thickness),
                                             new Rectangle(0, 0, _thickness, _thickness),
                                             BackgroundColor);
                            // Top
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + _thickness), Convert.ToInt32(_positions[0].Y), Convert.ToInt32(Dimensions.X - _thickness * 2.0f), _thickness),
                                             new Rectangle(_thickness, 0, _images[0].Width - _thickness * 2, _thickness),
                                             BackgroundColor);
                            // Corner Top Right
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + Dimensions.X - _thickness), Convert.ToInt32(_positions[0].Y), _thickness, _thickness),
                                             new Rectangle(_images[0].Width - _thickness, 0, _thickness, _thickness),
                                             BackgroundColor);
                            // Right
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + Dimensions.X - _thickness), Convert.ToInt32(_positions[0].Y + _thickness), _thickness, Convert.ToInt32(Dimensions.Y - _thickness * 2.0f)),
                                             new Rectangle(_images[0].Width - _thickness, _thickness, _thickness, _images[0].Height - _thickness * 2),
                                             BackgroundColor);
                            // Corner Bottom Right
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + Dimensions.X - _thickness), Convert.ToInt32(_positions[0].Y + Dimensions.Y - _thickness), _thickness, _thickness),
                                             new Rectangle(_images[0].Width - _thickness, _images[0].Height - _thickness, _thickness, _thickness),
                                             BackgroundColor);
                            // Bottom
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + _thickness), Convert.ToInt32(_positions[0].Y + Dimensions.Y - _thickness), Convert.ToInt32(Dimensions.X - _thickness * 2.0f), _thickness),
                                             new Rectangle(_thickness, _images[0].Height - _thickness, _images[0].Width - _thickness * 2, _thickness),
                                             BackgroundColor);
                            // Corner Bottom Left
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y + Dimensions.Y - _thickness), _thickness, _thickness),
                                             new Rectangle(0, _images[0].Height - _thickness, _thickness, _thickness),
                                             BackgroundColor);
                            // Left
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y + _thickness), _thickness, Convert.ToInt32(Dimensions.Y - _thickness * 2.0f)),
                                             new Rectangle(0, _thickness, _thickness, _images[0].Height - _thickness * 2),
                                             BackgroundColor);
                            // Center
                            spritebatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + _thickness), Convert.ToInt32(_positions[0].Y + _thickness), Convert.ToInt32(Dimensions.X - _thickness * 2.0f), Convert.ToInt32(Dimensions.Y - _thickness * 2.0f)),
                                             new Rectangle(_thickness, _thickness, _images[0].Width - _thickness * 2, _images[0].Height - _thickness * 2),
                                             BackgroundColor);

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
        /// Libère les ressources utilisées par le panneau.
        /// </summary>
        public void Dispose()
        {
            _texture.Dispose();
        }

        /// <summary>
        /// Met à jour l'état du panneau en fonction des interactions utilisateur.
        /// </summary>
        /// <param name="gametime">Temps de jeu actuel.</param>
        public void Update(GameTime gametime)
        {
            MouseState mouseState = Mouse.GetState();
            _clicked = false;
            if (_rectangleBackground.Contains(mouseState.Position))
            {
                _hover = true;
                if (_oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
                    _clicked = true;
            }
            else
                _hover = false;
            _oldMouseState = mouseState;
        }
        /// <summary>
        /// Détermine si le panneau a été cliqué.
        /// </summary>
        /// <returns>True si cliqué, sinon false.</returns>
        public bool IsClicked() => _clicked;
        /// <summary>
        /// Détermine si le panneau est survolé par la souris.
        /// </summary>
        /// <returns>True si survolé, sinon false.</returns>
        public bool IsHovered() => _hover;
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
            List<Texture2D> copiedTextures = new List<Texture2D>();
            foreach (Texture2D texture in _images)
                copiedTextures.Add(texture);
            return new Panel()
            {
                _borderColor = _borderColor,
                _clicked = _clicked,
                _images = copiedTextures,
                _positions = _positions,
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
    }
}
