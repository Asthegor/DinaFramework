using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DinaFramework.Graphics
{
    public class DropDown : Base, IUpdate, IDraw, IVisible, IPosition
    {
        private const int OFFSET_OPTIONS = 2;
        private SpriteFont _font;

        private List<string> _options;
        private int _selectedIndex;

        private bool _isExpanded;
        private bool _visible;

        private Panel _panel;
        private Text _text;
        private Panel _panelArrow;
        private List<Text> _listOptions;
        private Panel _panelOptions;


        private int _nbVisibleOptions;

        private MouseState _oldMouseState;
        private Color _selectedOptionColor;

        public DropDown(SpriteFont font, Texture2D arrowTexture, Vector2 position, Vector2 dimensions, List<string> options, Color textcolor, Color selectedoptioncolor, Color backgroundcolor, Color bordercolor, int thickness = 1, int nbvisibleoptions = 5)
            : base(position, dimensions)
        {
            ArgumentNullException.ThrowIfNull(arrowTexture);
            ArgumentNullException.ThrowIfNull(options);

            _font = font;

            _options = options;
            _selectedOptionColor = selectedoptioncolor;
            _nbVisibleOptions = nbvisibleoptions;

            Vector2 panelDim = dimensions;
            _panel = new Panel(position, panelDim, backgroundcolor, bordercolor, thickness);
            _text = new Text(font, "", textcolor, position + new Vector2(thickness, thickness));

            Vector2 panelArrowPos = position + new Vector2(panelDim.X - arrowTexture.Width - thickness, 0);
            _panelArrow = new Panel(panelArrowPos, new Vector2(arrowTexture.Width, Math.Max(dimensions.Y, arrowTexture.Height)), arrowTexture, 0);

            _listOptions = new List<Text>();
            foreach (string option in options)
            {
                Text t = new Text(font, option, textcolor);
                t.Visible = false;
                t.Dimensions = dimensions - new Vector2(thickness * 2, 0);
                _listOptions.Add(t);
            }
            _panelOptions = new Panel(position + new Vector2(0, _panel.Dimensions.Y), new Vector2(dimensions.X, (dimensions.Y + OFFSET_OPTIONS) * Math.Min(nbvisibleoptions, _listOptions.Count) + thickness), backgroundcolor, bordercolor, thickness);

            Reset();
        }

        public new Vector2 Position
        {
            get => base.Position;
            set
            {
                _panel.Position = value;
                _text.Position = value + new Vector2(_panel.Thickness, _panel.Thickness);
                _panelArrow.Position = value + new Vector2(Dimensions.X - _panelArrow.Dimensions.X - _panel.Thickness, 0);
                _panelOptions.Position = value + new Vector2(0, _panel.Dimensions.Y);
                base.Position = value;
            }
        }

        public bool Visible { get => _visible; set => _visible = value; }
        public string Value {
            get => _text.Content;
            set
            {
                bool exists = false;
                int index = -1;
                foreach (Text t in _listOptions)
                {
                    index++;
                    if (t.Content == value)
                    {
                        exists = true;
                        break;
                    }
                }
                if (exists)
                {
                    _text.Content = value;
                    _selectedIndex = index;
                    _listOptions[_selectedIndex].Color = _selectedOptionColor;
                }
            }
        }

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
        public void Reset()
        {
            _isExpanded = false;
            _selectedIndex = -1;
            _text.Content = "";
            _panelOptions.Visible = false;

            foreach (Text tb in _listOptions)
            {
                tb.Visible = false;
                tb.Color = _text.Color;
            }
        }
        public void Update(GameTime gametime)
        {
            bool clickedAway = false;
            MouseState mouseState = Mouse.GetState();
            if (_oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                Point mousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                clickedAway = true;

                _panelOptions.Visible = false;
                foreach (Text option in _listOptions)
                    option.Visible = false;

                // Si l'utilisateur clique sur la flèche, dépliez la liste déroulante
                Rectangle arrowRect = new Rectangle(_panelArrow.Position.ToPoint(), _panelArrow.Dimensions.ToPoint());
                if (arrowRect.Contains(mousePosition))
                {
                    _isExpanded = !_isExpanded;
                    clickedAway = false;
                }

                // Si la liste déroulante est dépliée et que l'utilisateur clique sur une option, mettez à jour l'indice sélectionné
                if (_isExpanded)
                {
                    _panelOptions.Visible = true;

                    int i = _selectedIndex;
                    if (_listOptions.Count - _selectedIndex < _nbVisibleOptions)
                        i = _listOptions.Count - _nbVisibleOptions;
                    if (i < 0)
                        i = 0;
                    for (int j = 0; j < Math.Min(_nbVisibleOptions, _listOptions.Count); j++)
                    {
                        _listOptions[i + j].Visible = true;
                        Vector2 newOptionPos = _panel.Position + new Vector2(_panelOptions.Thickness + OFFSET_OPTIONS * 2, _panel.Dimensions.Y * (j + 1) + _panelOptions.Thickness + OFFSET_OPTIONS * 2);
                        _listOptions[i + j].Position = newOptionPos;

                        Rectangle rect = new Rectangle(newOptionPos.ToPoint(), _listOptions[i + j].Dimensions.ToPoint());
                        if (rect.Contains(mousePosition))
                        {
                            if (_selectedIndex >= 0)
                                _listOptions[_selectedIndex].Color = _text.Color;
                            _selectedIndex = i + j;
                            _text.Content = _listOptions[_selectedIndex].Content;
                            _listOptions[_selectedIndex].Color = _selectedOptionColor;
                            _isExpanded = false;
                            clickedAway = false;
                        }

                    }
                    if (clickedAway)
                        _isExpanded = false;
                }
            }
            _oldMouseState = mouseState;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            if (Visible)
            {
                _panel.Draw(spritebatch);
                _text.Draw(spritebatch);
                _panelArrow.Draw(spritebatch);

                if (_isExpanded)
                {
                    _panelOptions.Draw(spritebatch);
                    foreach (Text option in _listOptions)
                        option.Draw(spritebatch);
                }
            }
        }
    }
}
