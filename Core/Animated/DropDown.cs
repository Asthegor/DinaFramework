using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace DinaFramework.Core.Animated
{
    public class DropDown : Base, IUpdate, IDraw, IVisible
    {
        private SpriteFont _font;
        private Texture2D _arrowTexture;
        private List<string> _options;
        private int _selectedIndex;
        private Rectangle _dropdownRect;
        private bool _isExpanded;
        private bool _visible;

        public DropDown(SpriteFont font, Texture2D arrowTexture, List<string> options)
        {
            _font = font;
            _arrowTexture = arrowTexture;
            _options = options;
            _selectedIndex = 0;
            _isExpanded = false;

            // Taille de la liste déroulante
            int width = _arrowTexture.Width + 100; // Largeur suffisante pour contenir le texte
            int height = _arrowTexture.Height;

            // Position de la liste déroulante (à ajuster selon vos besoins)
            int x = 100;
            int y = 100;

            _dropdownRect = new Rectangle(x, y, width, height);
        }

        public bool Visible { get => _visible; set => _visible = value; }

        public void AddOption(string option, int position = -1)
        {
            if (position >= 0 && position < _options.Count)
                _options.Insert(position, option);
            else
                _options.Add(option);
        }
        public void RemoveOption(string option)
        {
            _options.Remove(option);
        }
        public void Update(GameTime gameTime)
        {
            if (Mouse.GetState().LeftButton == ButtonState.Pressed)
            {
                Point mousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);

                // Si l'utilisateur clique sur la flèche, dépliez la liste déroulante
                Rectangle arrowRect = new Rectangle(_dropdownRect.Right - _arrowTexture.Width, _dropdownRect.Y, _arrowTexture.Width, _arrowTexture.Height);
                if (arrowRect.Contains(mousePosition))
                {
                    _isExpanded = !_isExpanded;
                }

                // Si la liste déroulante est dépliée et que l'utilisateur clique sur une option, mettez à jour l'indice sélectionné
                if (_isExpanded)
                {
                    for (int i = 0; i < _options.Count; i++)
                    {
                        Rectangle optionRect = new Rectangle(_dropdownRect.X, _dropdownRect.Bottom + (i * _font.LineSpacing), _dropdownRect.Width, _font.LineSpacing);
                        if (optionRect.Contains(mousePosition))
                        {
                            _selectedIndex = i;
                            _isExpanded = false;
                            break;
                        }
                    }
                }
            }
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (Visible)
            {

                // Dessiner la liste déroulante
                spritebatch?.Draw(_arrowTexture, new Vector2(_dropdownRect.Right - _arrowTexture.Width, _dropdownRect.Y), Color.White);
                spritebatch?.DrawString(_font, _options[_selectedIndex], new Vector2(_dropdownRect.X, _dropdownRect.Y), Color.White);

                // Si la liste déroulante est dépliée, dessinez les options
                if (_isExpanded)
                {
                    for (int i = 0; i < _options.Count; i++)
                    {
                        spritebatch?.DrawString(_font, _options[i], new Vector2(_dropdownRect.X, _dropdownRect.Bottom + (i * _font.LineSpacing)), Color.White);
                    }
                }
            }
        }
    }
}
