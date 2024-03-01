using DinaFramework.Core;
using DinaFramework.Core.Fixed;
using DinaFramework.Enums;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Menus
{
    public class MenuItem : IDraw, IPosition, IDimensions, IElement, IVisible, IColor
    {
        private bool _visible;
        private readonly object _item;
        Func<MenuItem, MenuItem> _selection;
        Func<MenuItem, MenuItem> _deselection;
        Func<MenuItem, MenuItem> _activation;
        public Func<MenuItem, MenuItem> Selection
        {
            get { return _selection; }
            set { _selection = value; }
        }
        public Func<MenuItem, MenuItem> Deselection
        {
            get { return _deselection; }
            set { _deselection = value; }
        }
        public Func<MenuItem, MenuItem> Activation
        {
            get { return _activation; }
            set { _activation = value; }
        }

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
        public Vector2 TextDimensions
        {
            get
            {
                if (_item is IText itemText)
                    return itemText.TextDimensions;
                return default;
            }
        }
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
        public bool Visible
        {
            get { return _visible; }
            set { _visible = value; }
        }
        public MenuItem(SpriteFont font, string text, Color color,
                        Func<MenuItem, MenuItem> selection = null,
                        Func<MenuItem, MenuItem> deselection = null,
                        Func<MenuItem, MenuItem> activation = null,
                        Vector2 position = default,
                        HorizontalAlignment halign = HorizontalAlignment.Left, VerticalAlignment valign = VerticalAlignment.Top) :
            this(new Text(font, text, color, position, halign, valign, 0), selection, deselection, activation, position)
        {
        }
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
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (_visible)
            {
                if (_item is IDraw item)
                    item.Draw(spritebatch);
            }
        }
    }
}
