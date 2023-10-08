using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace DinaFramework.Core.Fixed
{
    public class Panel : Base, IDraw, IVisible
    {
        private readonly List<Texture2D> _images = new List<Texture2D>();
        private readonly List<Vector2> _positions = new List<Vector2>();
        private Texture2D _texture;
        private Rectangle _rectangleBackground;
        private Rectangle _rectangleBorder;
        private int _thickness = 0;
        private Color _borderColor;
        private bool _visible;

        public Panel(Vector2 position, Vector2 dimensions, Color backgroundcolor, int zorder = 0) : base(position, dimensions, zorder)
        {
            BackgroundColor = backgroundcolor;
            BorderColor = backgroundcolor;
            Thickness = 0;
            CheckVisibility();
        }
        public Panel(Vector2 position, Vector2 dimensions, Color backgroundcolor, Color bordercolor, int thickness, int zorder = 0) : this(position, dimensions, backgroundcolor, zorder)
        {
            BorderColor = bordercolor;
            Thickness = thickness;
        }
        public Panel(Vector2 position, Vector2 dimensions, Texture2D image, int borderThickness, int zorder = 0) : base(position, dimensions, zorder)
        {
            _images.Add(image);
            _positions.Add(position);
            _thickness = borderThickness;
            CheckVisibility();
        }
        public Panel(Vector2 position, Vector2 dimensions, Texture2D cornerTopLeft, Texture2D top, Texture2D cornerTopRight, Texture2D right, Texture2D cornerBottomRight, Texture2D bottom, Texture2D cornerBottomLeft, Texture2D left, Texture2D center, int zorder = 0) : base(position, dimensions, zorder)
        {
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
        public new Vector2 Position
        {
            get => base.Position;
            set
            {
                Vector2 offset = value - base.Position;
                for (int index = 0; index < _positions.Count; index++)
                {
                    _positions[index] = new Vector2(_positions[index].X + offset.X, _positions[index].Y + offset.Y);
                }
                base.Position = value;
            }

        }

        public Color BackgroundColor { get; set; }
        public Color BorderColor
        {
            get { return _borderColor; }
            set
            {
                _borderColor = value;
                AdjustRectangles();
            }
        }
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
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            if (Visible && Dimensions != default)
            {
                switch (_images.Count)
                {
                    case 0:
                        if (_texture == null)
                        {
                            _texture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                            _texture.SetData(new[] { Color.White });
                        }
                        if (_thickness > 0 && BorderColor != BackgroundColor)
                            spriteBatch.Draw(_texture, _rectangleBorder, null, BorderColor);
                        spriteBatch.Draw(_texture, _rectangleBackground, null, BackgroundColor);
                        break;
                    case 1:
                        if (_thickness == 0)
                        {
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y), Convert.ToInt32(Dimensions.X), Convert.ToInt32(Dimensions.Y)),
                                             new Rectangle(0, 0, _images[0].Width, _images[0].Height),
                                             Color.White);
                        }
                        else
                        {
                            // Corner Top Left
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y), _thickness, _thickness),
                                             new Rectangle(0, 0, _thickness, _thickness),
                                             Color.White);
                            // Top
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + _thickness), Convert.ToInt32(_positions[0].Y), Convert.ToInt32(Dimensions.X - _thickness * 2.0f), _thickness),
                                             new Rectangle(_thickness, 0, _images[0].Width - _thickness * 2, _thickness),
                                             Color.White);
                            // Corner Top Right
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + Dimensions.X - _thickness), Convert.ToInt32(_positions[0].Y), _thickness, _thickness),
                                             new Rectangle(_images[0].Width - _thickness, 0, _thickness, _thickness),
                                             Color.White);
                            // Right
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + Dimensions.X - _thickness), Convert.ToInt32(_positions[0].Y + _thickness), _thickness, Convert.ToInt32(Dimensions.Y - _thickness * 2.0f)),
                                             new Rectangle(_images[0].Width - _thickness, _thickness, _thickness, _images[0].Height - _thickness * 2),
                                             Color.White);
                            // Corner Bottom Right
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + Dimensions.X - _thickness), Convert.ToInt32(_positions[0].Y + Dimensions.Y - _thickness), _thickness, _thickness),
                                             new Rectangle(_images[0].Width - _thickness, _images[0].Height - _thickness, _thickness, _thickness),
                                             Color.White);
                            // Bottom
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + _thickness), Convert.ToInt32(_positions[0].Y + Dimensions.Y - _thickness), Convert.ToInt32(Dimensions.X - _thickness * 2.0f), _thickness),
                                             new Rectangle(_thickness, _images[0].Height - _thickness, _images[0].Width - _thickness * 2, _thickness),
                                             Color.White);
                            // Corner Bottom Left
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y + Dimensions.Y - _thickness), _thickness, _thickness),
                                             new Rectangle(0, _images[0].Height - _thickness, _thickness, _thickness),
                                             Color.White);
                            // Left
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X), Convert.ToInt32(_positions[0].Y + _thickness), _thickness, Convert.ToInt32(Dimensions.Y - _thickness * 2.0f)),
                                             new Rectangle(0, _thickness, _thickness, _images[0].Height - _thickness * 2),
                                             Color.White);
                            // Center
                            spriteBatch.Draw(_images[0],
                                             new Rectangle(Convert.ToInt32(_positions[0].X + _thickness), Convert.ToInt32(_positions[0].Y + _thickness), Convert.ToInt32(Dimensions.X - _thickness * 2.0f), Convert.ToInt32(Dimensions.Y - _thickness * 2.0f)),
                                             new Rectangle(_thickness, _thickness, _images[0].Width - _thickness * 2, _images[0].Height - _thickness * 2),
                                             Color.White);

                        }
                        break;
                    case 9:
                        // Corner Top Left
                        spriteBatch.Draw(_images[0], _positions[0], Color.White);
                        // Top
                        spriteBatch.Draw(_images[1],
                                         new Rectangle(Convert.ToInt32(_positions[1].X),
                                                       Convert.ToInt32(_positions[1].Y),
                                                       Convert.ToInt32(Dimensions.X - _images[0].Width - _images[2].Width),
                                                       _images[1].Height),
                                         new Rectangle(0, 0, _images[1].Width, _images[1].Height),
                                         Color.White);
                        // Corner Top Right
                        spriteBatch.Draw(_images[2], _positions[2], Color.White);
                        // Right
                        spriteBatch.Draw(_images[3],
                                         new Rectangle(Convert.ToInt32(_positions[3].X),
                                                       Convert.ToInt32(_positions[3].Y),
                                                       Convert.ToInt32(_images[3].Width),
                                                       Convert.ToInt32(Dimensions.Y - _images[2].Height - _images[4].Height)),
                                         new Rectangle(0, 0, _images[3].Width, _images[3].Height),
                                         Color.White);
                        // Corner Bottom Right
                        spriteBatch.Draw(_images[4], _positions[4], Color.White);
                        // Bottom
                        spriteBatch.Draw(_images[5],
                                         new Rectangle(Convert.ToInt32(_positions[5].X),
                                                       Convert.ToInt32(_positions[5].Y),
                                                       Convert.ToInt32(Dimensions.X - _images[4].Width - _images[6].Width),
                                                       _images[5].Height),
                                         new Rectangle(0, 0, _images[5].Width, _images[5].Height),
                                         Color.White);
                        // Corner Bottom Left
                        spriteBatch.Draw(_images[6], _positions[6], Color.White);
                        // Left
                        spriteBatch.Draw(_images[7],
                                         new Rectangle(Convert.ToInt32(_positions[7].X),
                                                       Convert.ToInt32(_positions[7].Y),
                                                       Convert.ToInt32(_images[7].Width),
                                                       Convert.ToInt32(Dimensions.Y - _images[0].Height - _images[6].Height)),
                                         new Rectangle(0, 0, _images[7].Width, _images[7].Height),
                                         Color.White);
                        // Center
                        spriteBatch.Draw(_images[8],
                                         new Rectangle(Convert.ToInt32(_positions[8].X),
                                                       Convert.ToInt32(_positions[8].Y),
                                                       Convert.ToInt32(Dimensions.X - _images[0].Width - _images[2].Width),
                                                       Convert.ToInt32(Dimensions.Y - _images[0].Height - _images[6].Height)),
                                         new Rectangle(0, 0, _images[8].Width, _images[8].Height),
                                         Color.White);
                        break;
                    default:
                        throw new Exception("Images missing for the panel.");
                }
            }
        }
    }
}
