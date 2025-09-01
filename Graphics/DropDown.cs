#nullable enable
using DinaFramework.Core;
using DinaFramework.Interfaces;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Classe représentant un menu déroulant interactif avec des options sélectionnables.
    /// </summary>
    public class DropDown : Base, IUpdate, IDraw, IVisible, IPosition
    {
        private const int OFFSET_OPTIONS = 2;

        private readonly List<string> _options;
        private int _selectedIndex;

        private bool _isExpanded;
        private bool _visible;

        private readonly Panel _panel;
        private readonly Text _text;
        private readonly Panel _panelArrow;
        private readonly List<Text> _listOptions = [];
        private readonly Panel _panelOptions;
        
        private readonly int _nbVisibleOptions;

        private MouseState _oldMouseState;
        private Color _selectedOptionColor;

        /// <summary>
        /// Initialise une nouvelle instance de la classe DropDown.
        /// </summary>
        /// <param name="font">Police à utiliser pour afficher le texte.</param>
        /// <param name="arrowTexture">Texture de la flèche pour afficher l'option déroulante.</param>
        /// <param name="position">Position de l'élément dans l'espace 2D.</param>
        /// <param name="dimensions">Dimensions du menu déroulant.</param>
        /// <param name="options">Liste des options du menu déroulant.</param>
        /// <param name="textcolor">Couleur du texte des options.</param>
        /// <param name="selectedoptioncolor">Couleur de l'option sélectionnée.</param>
        /// <param name="backgroundcolor">Couleur de fond du menu déroulant.</param>
        /// <param name="bordercolor">Couleur de la bordure du menu déroulant.</param>
        /// <param name="thickness">Épaisseur de la bordure.</param>
        /// <param name="nbvisibleoptions">Nombre d'options visibles à afficher par défaut.</param>
        public DropDown(SpriteFont font, Vector2 position, Vector2 dimensions, IReadOnlyList<string> options, Color textcolor, Color selectedoptioncolor, Color backgroundcolor, Color bordercolor, Texture2D? arrowTexture = null, int thickness = 1, int nbvisibleoptions = 5)
            : base(position, dimensions)
        {
            ArgumentNullException.ThrowIfNull(arrowTexture);
            ArgumentNullException.ThrowIfNull(options);

            _panel = new Panel(position, dimensions, backgroundcolor, bordercolor, thickness);

            _options = [.. options];
            _selectedOptionColor = selectedoptioncolor;
            _nbVisibleOptions = nbvisibleoptions;

            Vector2 panelDim = dimensions;
            _text = new Text(font, "", textcolor, position + new Vector2(thickness, thickness));

            Vector2 panelArrowPos = position + new Vector2(panelDim.X - arrowTexture.Width - thickness, 0);
            _panelArrow = new Panel(panelArrowPos, new Vector2(arrowTexture.Width, Math.Max(dimensions.Y, arrowTexture.Height)), arrowTexture, 0);

            foreach (string option in options)
            {
                Text item = new Text(font, option, textcolor)
                {
                    Visible = false,
                    Dimensions = dimensions - new Vector2(thickness * 2, 0)
                };
                _listOptions.Add(item);
            }
            _panelOptions = new Panel(position + new Vector2(0, _panel.Dimensions.Y), new Vector2(dimensions.X, (dimensions.Y + OFFSET_OPTIONS) * Math.Min(nbvisibleoptions, _listOptions.Count) + thickness), backgroundcolor, bordercolor, thickness);

            // Gestion des clics de souris
            _panel.OnClicked += (sender, eventArgs) =>
            {
                _isExpanded = !_isExpanded;
                _panelOptions.Visible = _isExpanded;
            };

            Reset();
        }

        /// <summary>
        /// Position du menu déroulant.
        /// </summary>
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

        /// <summary>
        /// Visibilité du menu déroulant.
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }
        /// <summary>
        /// Option sélectionnée dans le menu déroulant.
        /// </summary>
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

        /// <summary>
        /// Ajoute une nouvelle option au menu déroulant à la position spécifiée.
        /// </summary>
        /// <param name="option">Option à ajouter.</param>
        /// <param name="position">Position où ajouter l'option (par défaut à la fin).</param>
        public void AddOption(string option, int position = -1)
        {
            if (position >= 0 && position < _options.Count)
                _options.Insert(position, option);
            else
                _options.Add(option);
        }
        /// <summary>
        /// Supprime une option du menu déroulant.
        /// </summary>
        /// <param name="option">Option à supprimer.</param>
        public void RemoveOption(string option)
        {
            _options.Remove(option);
        }
        /// <summary>
        /// Efface toutes les options du menu déroulant.
        /// </summary>
        public void Clear()
        {
            _options.Clear();
        }
        
        /// <summary>
        /// Réinitialise le menu déroulant à son état initial (fermé et sans option sélectionnée).
        /// </summary>
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
        /// <summary>
        /// Met à jour l'état du menu déroulant (ouvert/fermé, sélection d'option).
        /// </summary>
        /// <param name="gametime">Temps de jeu actuel.</param>
        public void Update(GameTime gametime)
        {
            _panel.Update(gametime);
            _panelArrow.Update(gametime);



            bool clickedAway;
            MouseState mouseState = Mouse.GetState();
            if (_oldMouseState.LeftButton == ButtonState.Pressed && mouseState.LeftButton == ButtonState.Released)
            {
                Point mousePosition = new Point(Mouse.GetState().X, Mouse.GetState().Y);
                clickedAway = true;

                _panelOptions.Visible = false;
                foreach (Text option in _listOptions)
                    option.Visible = false;

                // Si l'utilisateur clique sur la flèche, dépliez la liste déroulante
                //Rectangle arrowRect = new Rectangle(_panelArrow.Position.ToPoint(), _panelArrow.Dimensions.ToPoint());
                Rectangle rect = new Rectangle(_panel.Position.ToPoint(), _panel.Dimensions.ToPoint());
                //if (arrowRect.Contains(mousePosition))
                if (rect.Contains(mousePosition))
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

                        Rectangle rectOptions = new Rectangle(newOptionPos.ToPoint(), _listOptions[i + j].Dimensions.ToPoint());
                        if (rectOptions.Contains(mousePosition))
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
        /*
        private void HandleMouseClick()
        {

        }
        */
        /// <summary>
        /// Dessine le menu déroulant sur l'écran.
        /// </summary>
        /// <param name="spritebatch">Objet SpriteBatch pour dessiner les éléments.</param>
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
