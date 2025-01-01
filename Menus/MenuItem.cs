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
    public class MenuItem : IDraw, IPosition, IDimensions, IElement, IVisible, IColor
    {
        private bool _visible;
        private readonly object _item;
        Func<MenuItem, MenuItem> _selection;
        Func<MenuItem, MenuItem> _deselection;
        Func<MenuItem, MenuItem> _activation;
        /// <summary>
        /// Fonction exécutée lors de la sélection de l'élément de menu.
        /// </summary>
        public Func<MenuItem, MenuItem> Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }
        /// <summary>
        /// Fonction exécutée lors de la désélection de l'élément de menu.
        /// </summary>
        public Func<MenuItem, MenuItem> Deselection
        {
            get { return _deselection; }
            set { _deselection = value; }
        }
        /// <summary>
        /// Fonction exécutée lors de l'activation (validation) de l'élément de menu.
        /// </summary>
        public Func<MenuItem, MenuItem> Activation
        {
            get { return _activation; }
            set { _activation = value; }
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
        /// Le contenu du texte associé à l'élément de menu.
        /// </summary>
        public string Content
        {
            get
            {
                if (_item is Text textitem)
                    return textitem.Content;
                return default;
            }
            set
            {
                if (_item is Text textitem)
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
                if (_item is Text textitem)
                    return textitem.Font;
                return null;
            }
            set
            {
                if (_item is Text textitem)
                    textitem.Font = value;
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
                        Func<MenuItem, MenuItem> selection = null,
                        Func<MenuItem, MenuItem> deselection = null,
                        Func<MenuItem, MenuItem> activation = null,
                        Vector2 position = default,
                        HorizontalAlignment halign = HorizontalAlignment.Left, VerticalAlignment valign = VerticalAlignment.Top) :
            this(new Text(font, text, color, position, halign, valign, 0), selection, deselection, activation, position)
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
                        Func<MenuItem, MenuItem> selection = null,
                        Func<MenuItem, MenuItem> deselection = null,
                        Func<MenuItem, MenuItem> activation = null,
                        Vector2 position = default)
        {
            _item = item;
            if (_item is IPosition positem)
                positem.Position = position;
            Selection = selection;
            Deselection = deselection;
            Activation = activation;
            Visible = true;
            State = MenuItemState.Enable;
        }
        //public void Draw(SpriteBatch spritebatch)
        //{
        //    if (_visible)
        //    {
        //        Color previousColor = Color.White;
        //        if (_item is IColor colorItem)
        //        {
        //            previousColor = colorItem.Color;
        //            if (State == MenuItemState.Disable)
        //                colorItem.Color = Color.DarkGray;
        //        }

        //        if (_item is IDraw item)
        //            item.Draw(spritebatch);

        //        if (_item is IColor colorItem2)
        //            colorItem2.Color = previousColor;
        //    }
        //}
        /// <summary>
        /// Dessine l'élément de menu à l'écran, en respectant sa visibilité et son état.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour dessiner l'élément.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!_visible || _item is not IDraw drawableItem)
                return;

            IColor colorItem = _item as IColor;
            Color? originalColor = colorItem?.Color;

            if (colorItem != null && State == MenuItemState.Disable)
                colorItem.Color = Color.DarkGray;

            drawableItem.Draw(spritebatch);

            if (colorItem != null && originalColor.HasValue)
                colorItem.Color = originalColor.Value;
        }
    }
}
