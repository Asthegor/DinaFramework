using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace DinaFramework.Graphics
{
    public class InputText : IUpdate, IDraw, IVisible
    {
        private const float DELAY_KEY_STROKE = 0.5f;
        private const float REPEAT_INTERVAL = 0.25f;  // Intervalle entre les répétitions

        private Group _inputTextGroup;
        private Panel _panel;
        private Text _placeHolder;
        private Text _text;
        private Text _cursor;
        private bool _visible;
        private bool _isActive;
        
        private bool _onlyDigit;

        private MouseState _oldMouseState;
        private KeyboardState _oldKeyboardState;

        private Dictionary<Keys, float> _keyTimers;

        public bool Visible { get => _visible; set => _visible = value; }

        public InputText(SpriteFont spritefont, string content, Color color, Vector2 position, Vector2 dimensions, Vector2 offset,
                         Color backgroundcolor, Color bordercolor, int thickness = 1, string placeholder = "", Color placeholdercolor = new Color(),
                         bool onlyDigit = false)
        {
            _onlyDigit = onlyDigit;

            _inputTextGroup = new Group();
            Vector2 pos = Vector2.Zero;

            _panel = new Panel(pos, dimensions, backgroundcolor, bordercolor, thickness);

            _text = new Text(spritefont, content, color, pos + offset / 2);
            _text.Dimensions = dimensions;

            _placeHolder = new Text(spritefont, placeholder, placeholdercolor, pos + offset / 2);
            _placeHolder.Dimensions = dimensions;
            if (string.IsNullOrEmpty(placeholder))
                _placeHolder.Visible = false;

            _cursor = new Text(spritefont, "|", color);
            _cursor.SetTimers(0.5f, 1f, -1); //0.5s d'attente, 1s d'affichage, -1: boucle infinie
            _cursor.Visible = false;

            _inputTextGroup.Add(_panel);
            _inputTextGroup.Add(_text);
            _inputTextGroup.Add(_placeHolder);
            _inputTextGroup.Add(_cursor);

            _inputTextGroup.Position = position;

            _keyTimers = new Dictionary<Keys, float>();
            _isActive = false;
        }
        public string Content
        {
            get => _text.Content;
            set => _text.Content = value;
        }
        public Color BackgroundColor
        {
            get => _panel.BackgroundColor;
            set => _panel.BackgroundColor = value;
        }
        public Color Color
        {
            get => _text.Color;
            set => _text.Color = value;
        }
        public Color BorderColor
        {
            get => _panel.BorderColor;
            set => _panel.BorderColor = value;
        }

        public void Update(GameTime gametime)
        {
            MouseState mouseState = Mouse.GetState();
            KeyboardState _keyboardState = Keyboard.GetState();
            
            double elapsedTime = gametime.ElapsedGameTime.TotalSeconds;

            _inputTextGroup?.Update(gametime);

            if (_oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                _cursor.Visible = false;
                _isActive = false;
                if (string.IsNullOrEmpty(_text.Content))
                    _placeHolder.Visible = true;

                if (_panel.IsClicked())
                {
                    UpdateCursorPosition();
                    _cursor.Visible = true;
                    _placeHolder.Visible = false;
                    _isActive = true;
                }
            }

            if (_isActive)
            {
                
                foreach (Keys key in Enum.GetValues<Keys>())
                {
                    bool isKeyPressed = _keyboardState.IsKeyDown(key);
                    bool wasKeyPressed = _oldKeyboardState.IsKeyDown(key);

                    if (isKeyPressed)
                    {
                        if (!_keyTimers.ContainsKey(key))
                        {
                            // Nouvelle touche pressée
                            _keyTimers[key] = 0f;
                            ProcessKey(key, _keyboardState);
                        }
                        else
                        {
                            // Mise à jour du timer
                            _keyTimers[key] += (float)elapsedTime;
                            if ((wasKeyPressed && _keyTimers[key] >= DELAY_KEY_STROKE) ||
                                _keyTimers[key] >= REPEAT_INTERVAL + DELAY_KEY_STROKE)
                            {
                                ProcessKey(key, _keyboardState);
                                _keyTimers[key] = DELAY_KEY_STROKE; // Réinitialisation pour répétition
                            }
                        }
                    }
                    else
                    {
                        // Touche relâchée
                        _keyTimers.Remove(key);
                    }
                }
            }

            _oldKeyboardState = _keyboardState;
            _oldMouseState = mouseState;
        }

        public void Draw(SpriteBatch spritebatch)
        {
            _inputTextGroup.Draw(spritebatch);
        }

        private void ProcessKey(Keys key, KeyboardState state)
        {
            string character = KeyToString(key, state);
            if (!string.IsNullOrEmpty(character))
            {
                _text.Content += character;
                _cursor.Position = _text.Position + new Vector2(_text.Dimensions.X + 1, 0);
            }
            else if (key == Keys.Back && _text.Content.Length > 0)
            {
                _text.Content = _text.Content.Substring(0, _text.Content.Length - 1);
            }
            UpdateCursorPosition();
        }
        private string KeyToString(Keys key, KeyboardState state)
        {
            if ((key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                return key.ToString()[key.ToString().Length - 1].ToString();
            }
            if (!_onlyDigit)
            {
                if (key >= Keys.A && key <= Keys.Z)
                {
                    // Vérifie si Shift est enfoncé
                    bool isShiftDown = state.IsKeyDown(Keys.LeftShift) || state.IsKeyDown(Keys.RightShift);
                    return isShiftDown ? key.ToString() : key.ToString().ToLower(System.Globalization.CultureInfo.CurrentCulture);
                }
                else if (key == Keys.Space)
                {
                    return " ";
                }
            }
            return string.Empty;
        }
        private void UpdateCursorPosition()
        {
            _cursor.Position = _text.Position + new Vector2(_text.TextDimensions.X + 1, 0);
        }
    }
}
