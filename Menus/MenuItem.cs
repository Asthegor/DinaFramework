using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Graphics;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Menus
{
    /// <summary>
    /// Représente un élément de menu interactif qui peut être sélectionné, désélectionné et activé.
    /// Implémente les interfaces IDraw, IPosition, IDimensions, IElement, IVisible et IColor.
    /// </summary>
    public class MenuItem : IDraw, IPosition, IDimensions, IElement, IVisible, IColor, IUpdate
    {
        private bool _visible;
        private readonly object _item;
        private Func<MenuItem, MenuItem>? _selection;
        private Func<MenuItem, MenuItem>? _deselection;
        private Func<MenuItem, MenuItem>? _activation;
        private Color _disabledColor = Color.DarkGray;
        //private Dictionary<string, object> _originalValues = new Dictionary<string, object>();
        //private List<string> _modifiedValues = [];
        //private bool hasBeenRestored;
        //private bool saveOriginalValueCalled;
        //private bool saveCalled;
        private bool _isHovered;
        private bool _wasHovered;
        private bool _isClicked;
        private bool _wasClicked;

        /// <summary>
        /// Fonction exécutée lors de la sélection de l'élément de menu.
        /// </summary>
        public Func<MenuItem, MenuItem>? Selection
        {
            get { return _selection; }
            set { _selection = value!; }
        }
        /// <summary>
        /// Fonction exécutée lors de la désélection de l'élément de menu.
        /// </summary>
        public Func<MenuItem, MenuItem>? Deselection
        {
            get { return _deselection; }
            set { _deselection = value!; }
        }
        /// <summary>
        /// Fonction exécutée lors de l'activation (validation) de l'élément de menu.
        /// </summary>
        public Func<MenuItem, MenuItem>? Activation
        {
            get { return _activation; }
            set { _activation = value!; }
        }

        /// <summary>
        /// L'état actuel de l'élément de menu (Enable ou Disable).
        /// </summary>
        public MenuItemState State { get; set; }

        /// <summary>
        /// La position de l'élément de menu.
        /// </summary>
        public Vector2 Position
        {
            get
            {
                if (_item is IPosition posItem)
                    return posItem.Position;
                return default;
            }
            set
            {
                if (_item is IPosition posItem)
                    posItem.Position = value;
            }
        }
        /// <summary>
        /// Les dimensions de l'élément de menu (largeur et hauteur).
        /// </summary>
        public Vector2 Dimensions
        {
            get
            {
                if (_item is IDimensions dimItem)
                    return dimItem.Dimensions;
                return default;
            }
            set
            {
                if (_item is IDimensions dimItem)
                    dimItem.Dimensions = value;
            }
        }
        /// <summary>
        /// Les dimensions du texte associé à l'élément de menu.
        /// </summary>
        public Vector2 TextDimensions
        {
            get
            {
                if (_item is IText itemText)
                    return itemText.TextDimensions;
                return default;
            }
        }
        /// <summary>
        /// Le ZOrder de l'élément de menu (utilisé pour le tri de l'affichage).
        /// </summary>
        public int ZOrder
        {
            get
            {
                if (_item is Base baseitem)
                    return baseitem.ZOrder;
                return default;
            }
            set
            {
                if (_item is Base baseitem)
                    baseitem.ZOrder = value;
            }
        }
        /// <summary>
        /// La couleur de l'élément de menu.
        /// </summary>
        public Color Color
        {
            get
            {
                if (_item is IColor coloritem)
                    return coloritem.Color;
                return default;
            }
            set
            {
                if (_item is IColor coloritem)
                    coloritem.Color = value;
            }
        }
        /// <summary>
        /// Couleur lorsque le MenuItem est déactivé.
        /// </summary>
        public Color DisableColor
        {
            get => _disabledColor;
            set => _disabledColor = value;
        }
        /// <summary>
        /// Le contenu du texte associé à l'élément de menu.
        /// </summary>
        public string Content
        {
            get
            {
                if (_item is DFText textitem)
                    return textitem.Content;
                return string.Empty;
            }
            set
            {
                if (_item is DFText textitem)
                    textitem.Content = value;
            }
        }
        /// <summary>
        /// Indique si l'élément de menu est visible ou non.
        /// </summary>
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }

        /// <summary>
        /// La police utilisée pour afficher le texte de l'élément de menu.
        /// </summary>
        public SpriteFont Font
        {
            get
            {
                if (_item is DFText textitem)
                    return textitem.Font;
                throw new InvalidOperationException("L'item n'est pas un DFText");
            }
            set
            {
                if (_item is DFText textitem)
                    textitem.Font = value;
                else
                    throw new InvalidOperationException("L'item n'est pas un DFText");
            }
        }

        /// <summary>
        /// Constructeur pour initialiser un élément de menu avec un texte, une couleur, et des fonctions pour la sélection, désélection et activation.
        /// </summary>
        /// <param name="font">La police du texte.</param>
        /// <param name="text">Le texte à afficher.</param>
        /// <param name="color">La couleur du texte.</param>
        /// <param name="selection">Fonction de sélection (optionnelle).</param>
        /// <param name="deselection">Fonction de désélection (optionnelle).</param>
        /// <param name="activation">Fonction d'activation (optionnelle).</param>
        /// <param name="position">Position de l'élément (optionnelle).</param>
        /// <param name="halign">Alignement horizontal (optionnel).</param>
        /// <param name="valign">Alignement vertical (optionnel).</param>
        public MenuItem(SpriteFont font, string text, Color color,
                        Func<MenuItem, MenuItem>? selection = null,
                        Func<MenuItem, MenuItem>? deselection = null,
                        Func<MenuItem, MenuItem>? activation = null,
                        Vector2 position = default,
                        HorizontalAlignment halign = HorizontalAlignment.Left, VerticalAlignment valign = VerticalAlignment.Top) :
            this(new DFText(font, text, color, position, halign, valign, 0), selection, deselection, activation, position)
        {
        }
        /// <summary>
        /// Constructeur pour initialiser un élément de menu avec un objet personnalisé, ainsi que des fonctions pour la sélection, désélection et activation.
        /// </summary>
        /// <param name="item">L'objet de l'élément de menu.</param>
        /// <param name="selection">Fonction de sélection (optionnelle).</param>
        /// <param name="deselection">Fonction de désélection (optionnelle).</param>
        /// <param name="activation">Fonction d'activation (optionnelle).</param>
        /// <param name="position">Position de l'élément (optionnelle).</param>
        public MenuItem(object item,
                        Func<MenuItem, MenuItem>? selection = null,
                        Func<MenuItem, MenuItem>? deselection = null,
                        Func<MenuItem, MenuItem>? activation = null,
                        Vector2 position = default)
        {
            _item = item;
            if (_item is IPosition positem)
                positem.Position = position;
            _selection = selection;
            _deselection = deselection;
            _activation = activation;
            Visible = true;
            State = MenuItemState.Enable;
        }

        /// <summary>
        /// Dessine l'élément de menu à l'écran, en respectant sa visibilité et son état.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour dessiner l'élément.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!_visible || _item is not IDraw drawableItem)
                return;

            IColor? coloredItem = _item as IColor;
            Color? originalColor = coloredItem?.Color;

            if (coloredItem != null && State == MenuItemState.Disable)
                coloredItem.Color = _disabledColor;

            drawableItem.Draw(spritebatch);

            if (coloredItem != null && originalColor.HasValue)
                coloredItem.Color = originalColor.Value;
        }

        /// <summary>
        /// Met à jour l'état de l'élément de menu en fonction du temps de jeu.
        /// </summary>
        /// <param name="gametime"></param>
        public void Update(GameTime gametime)
        {
            if (State == MenuItemState.Disable || _item is not IUpdate updateItem)
                return;

            updateItem.Update(gametime);
            UpdateHover();
            UpdateClick();
        }
        private void UpdateHover()
        {
            if (_item is not IHovered hoveredItem)
                return;

            _isHovered = hoveredItem.IsHovered();

            if (_isHovered && !_wasHovered)
                Selection?.Invoke(this);
            else if (!_isHovered && _wasHovered)
                Deselection?.Invoke(this);

            _wasHovered = _isHovered;
        }

        private void UpdateClick()
        {
            if (_item is not IClickable clickableItem)
                return;

            _isClicked = clickableItem.IsClicked();

            if (_isClicked && !_wasClicked)
                Activation?.Invoke(this);

            _wasClicked = _isClicked;
        }



        //private Dictionary<string, object> SaveValues()
        //{
        //    Dictionary<string, object> values = [];
        //    // Récupère toutes les propriétés publiques du Panel et les enregistre dans le dictionnaire.
        //    Type type = _item.GetType();
        //    PropertyInfo[] properties = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

        //    foreach (PropertyInfo property in properties)
        //    {
        //        var value = property.GetValue(this);
        //        if (value != null)
        //            values[property.Name] = value;
        //    }
        //    return values;
        //}
        //private void RestoreOriginalValues(List<string> modifiedKeys)
        //{
        //    ArgumentNullException.ThrowIfNull(modifiedKeys, nameof(modifiedKeys));
        //    if (modifiedKeys.Count == 0)
        //    {
        //        foreach (KeyValuePair<string, object> pair in _originalValues)
        //        {
        //            // Utilise la réflexion pour définir la valeur de la propriété correspondante.
        //            PropertyInfo? property = this.GetType().GetProperty(pair.Key);
        //            if (property != null)
        //                property.SetValue(this, pair.Value);
        //        }
        //    }
        //    else
        //    {
        //        foreach (string key in modifiedKeys)
        //        {
        //            if (_originalValues.ContainsKey(key))
        //            {
        //                PropertyInfo? property = this.GetType().GetProperty(key);
        //                if (property != null)
        //                    property.SetValue(this, _originalValues[key]);
        //            }
        //        }
        //    }
        //}

    }
}
