using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente un champ de saisie de texte interactif, permettant à l'utilisateur de saisir du texte.
    /// </summary>
    public class InputText : IUpdate, IDraw, IVisible, IElement, IColor, IPosition, IDimensions
    {
        private const float DELAY_KEY_STROKE = 0.5f;
        private const float REPEAT_INTERVAL = 0.25f;  // Intervalle entre les répétitions

        private readonly Group _inputTextGroup = [];
        private readonly Panel _panel;
        private readonly Text _placeHolder;
        private readonly Text _text;
        private readonly Text _cursor;
        private bool _visible;
        private bool _isActive;

        private readonly bool _onlyDigit;

        private MouseState _oldMouseState;
        private KeyboardState _oldKeyboardState;

        private readonly Dictionary<Keys, float> _keyTimers = [];

        /// <summary>
        /// Visibilité du champ de texte.
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }

        /// <summary>
        /// Initialise une nouvelle instance de la classe InputText avec les éléments spécifiés.
        /// </summary>
        /// <param name="font">Police utilisée pour afficher le texte.</param>
        /// <param name="content">Contenu du texte.</param>
        /// <param name="color">Couleur du texte.</param>
        /// <param name="position">Position de la zone de saisie sur l'écran.</param>
        /// <param name="dimensions">Taille de la zone de saisie sur l'écran.</param>
        /// <param name="offset">Offset du cadre par rapport au texte.</param>
        /// <param name="backgroundcolor">Couleur du fond.</param>
        /// <param name="bordercolor">Couleur de la bordure.</param>
        /// <param name="thickness">Épaisseur de la bordure.</param>
        /// <param name="placeholder">Texte de remplacement.</param>
        /// <param name="placeholdercolor">Couleur du texte de remplacement.</param>
        /// <param name="onlyDigit">Indique si la zone de saisie ne doit contenir que des chiffres (par défaut : false).</param>
        public InputText(SpriteFont font, string content, Color color, string placeholder, Color placeholdercolor, 
                         Vector2 position, Vector2 dimensions, Vector2 offset,
                         Color backgroundcolor, Color bordercolor, int thickness = 1, 
                         bool onlyDigit = false)
        {
            if (string.IsNullOrEmpty(content) && string.IsNullOrEmpty(placeholder))
            {
                throw new ArgumentException("Au moins l’un des deux paramètres 'content' ou 'placeholder' doit être renseigné.");
            }
            _onlyDigit = onlyDigit;

            _panel = new Panel(Vector2.Zero, dimensions, backgroundcolor, bordercolor, thickness);
            _placeHolder = new Text(font, placeholder, placeholdercolor, offset);
            _text = new Text(font, content, color, offset);
            Vector2 textPos = offset + new Vector2(0, (dimensions.Y - _placeHolder.Dimensions.Y) / 2);
            _placeHolder.Position = _text.Position = textPos;

            if (dimensions == default)
            {
                _panel.Dimensions = _placeHolder.Dimensions + offset * 2;
                _text.Dimensions = _placeHolder.Dimensions;
                Dimensions = _panel.Dimensions;
            }

            if (string.IsNullOrEmpty(placeholder))
                _placeHolder.Visible = false;


            _cursor = new Text(font, "|", color);
            _cursor.SetTimers(0.5f, 1f, -1); //0.5s d'attente, 1s d'affichage, -1: boucle infinie
            _cursor.Visible = false;

            _inputTextGroup.Add(_panel);
            _inputTextGroup.Add(_text);
            _inputTextGroup.Add(_placeHolder);
            _inputTextGroup.Add(_cursor);

            _inputTextGroup.Position = position;

            _isActive = false;
        }
        /// <summary>
        /// Texte contenu dans le champ de texte.
        /// </summary>
        public string Content { get => _text.Content; set => _text.Content = value; }
        /// <summary>
        /// Couleur de fond du champ de texte.
        /// </summary>
        public Color BackgroundColor { get => _panel.BackgroundColor; set => _panel.BackgroundColor = value; }
        /// <summary>
        /// Couleur du texte du champ de texte.
        /// </summary>
        public Color Color { get => _text.Color; set => _text.Color = value; }
        /// <summary>
        /// Couleur de la bordure du champ de texte.
        /// </summary>
        public Color BorderColor { get => _panel.BorderColor; set => _panel.BorderColor = value; }
        /// <summary>
        /// Position du champ de texte.
        /// </summary>
        public Vector2 Position { get => _inputTextGroup.Position; set => _inputTextGroup.Position = value; }
        /// <summary>
        /// Dimensions du champ de texte.
        /// </summary>
        public Vector2 Dimensions { get => _inputTextGroup.Dimensions; set => _inputTextGroup.Dimensions = value; }

        /// <summary>
        /// Ordre d'affichage (Z-order) de la zone de saisie.
        /// </summary>
        public int ZOrder { get => _inputTextGroup.ZOrder; set => _inputTextGroup.ZOrder = value; }

        /// <summary>
        /// Met à jour l'état du champ de texte en fonction du temps écoulé et des entrées utilisateur.
        /// Gère le focus via clic souris, l'affichage du curseur, du texte d'attente (placeholder)
        /// et la saisie clavier avec prise en charge de la répétition automatique (key repeat).
        /// </summary>
        /// <param name="gametime">Temps écoulé depuis la dernière mise à jour de la frame.</param>
        public void Update(GameTime gametime)
        {
            ArgumentNullException.ThrowIfNull(gametime, nameof(gametime));

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

        /// <summary>
        /// Dessine la zone de saisie à l'écran.
        /// </summary>
        /// <param name="spritebatch">L'instance de SpriteBatch utilisée pour dessiner le texte.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!_visible)
                return;

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
                _text.Content = _text.Content[..^1];
            }
            UpdateCursorPosition();
        }
        private string KeyToString(Keys key, KeyboardState state)
        {
            if ((key >= Keys.D0 && key <= Keys.D9) || (key >= Keys.NumPad0 && key <= Keys.NumPad9))
            {
                return key.ToString()[^1].ToString();
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
