using DinaFramework.Controls;
using DinaFramework.Core;
using DinaFramework.Core.Fixed;
using DinaFramework.Enums;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace DinaFramework.Menus
{
    public class MenuManager : IDraw, IUpdate, IVisible
    {
        private const int DEFAULT_SPACING = 5;
        private int _iconSpacing = DEFAULT_SPACING;
        private readonly List<IElement> _elements;
        private readonly List<object> _titles;
        private readonly Group _itemsGroup = null;
        private readonly List<MenuItem> _items;
        private readonly int _itemspacing;
        private ControllerKey _next_item_key;
        private ControllerKey _previous_item_key;
        private ControllerKey _active_item_key;
        private ControllerKey _cancel_menu_key;
        private int _currentitemindex = -1;
        private Sprite _iconLeft;
        private Sprite _iconRight;
        private IconMenuAlignment _iconAlignment;
        private bool _visible;
        private Action _cancellation;
        private Panel _background;
        private int _borderSpacing = DEFAULT_SPACING;


        // Construteurs
        public MenuManager(int itemspacing = DEFAULT_SPACING, Action cancellation = null, int currentitemindex = -1)
        {
            _elements = new List<IElement>();
            _titles = new List<object>();
            _items = new List<MenuItem>();
            _itemspacing = itemspacing;
            _itemsGroup = new Group();
            _elements.Add(_itemsGroup);
            Visible = true;
            Cancellation = cancellation;
            Reset(currentitemindex);
        }


        // Propriétés
        public Vector2 ItemsDimensions => _itemsGroup.Dimensions;
        public Vector2 ItemsPosition
        {
            get => _itemsGroup.Position;
            set => _itemsGroup.Position = value;
        }
        public MenuItem CurrentItem
        {
            get => (_currentitemindex == -1 || _currentitemindex >= _items.Count) ? null : _items[_currentitemindex];
            set => _currentitemindex = _items.IndexOf(value);
        }
        public IconMenuAlignment IconAlignment { get => _iconAlignment; set => _iconAlignment = value; }
        public bool IconsVisible
        {
            get => (_iconLeft != null && _iconLeft.Visible) || (_iconRight != null && _iconRight.Visible);
            set
            {
                if (_iconLeft != null)
                    _iconLeft.Visible = value;
                if (_iconRight != null)
                    _iconRight.Visible = value;
                // Mise à jour des dimensions du groupe
                Vector2 groupDim = _itemsGroup.Dimensions;
                if (_iconLeft != null)
                    groupDim.X += (_iconLeft.Dimensions.X + _iconSpacing) * (value ? 1 : -1);
                if (_iconRight != null)
                    groupDim.X += (_iconRight.Dimensions.X + _iconSpacing) * (value ? 1 : -1);
                _itemsGroup.Dimensions = groupDim;
                // Mise à jour de la position des items
                foreach (var item in _items)
                {
                    Vector2 itemPos = item.Position;
                    if (_iconLeft != null)
                        itemPos.X += (_iconLeft.Dimensions.X + _iconSpacing) * (value ? 1 : -1);
                    item.Position = itemPos;
                }
            }
        }
        public Action Cancellation
        {
            get => _cancellation;
            set => _cancellation = value;
        }
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }


        // Titres
        public void AddTitle(SpriteFont font, string text, Vector2 position, Color color, int zorder = 0)
        {
            Text title = new Text(font, text, color, position, default, default, zorder);
            _elements.Add(title);
            _titles.Add(title);
            SortElements();
        }
        public void AddTitle(SpriteFont font, string text, Vector2 position, Color color, Color shadowcolor, Vector2 shadowOffset, int zorder = 0)
        {
            ShadowText title = new ShadowText(font, text, color, position, shadowcolor, shadowOffset, default, default, zorder);
            _elements.Add(title);
            _titles.Add(title);
            SortElements();
        }
        public void CenterTitles(Vector2 screendimensions)
        {
            foreach (var title in _titles)
            {
                Vector2 titleDim;
                Vector2 titlePos;
                if (title is Text titleText)
                {
                    titleDim = titleText.Dimensions;
                    titlePos = titleText.Position;
                    titlePos.X = (screendimensions.X - titleDim.X) / 2.0f;
                    titleText.Position = titlePos;
                }
                else if (title is ShadowText titleShadow)
                {
                    titleDim = titleShadow.Dimensions;
                    titlePos = titleShadow.Position;
                    titlePos.X = (screendimensions.X - titleDim.X) / 2.0f;
                    titleShadow.Position = titlePos;
                }
            }
        }

        // Icones
        public void SetIconItems(Texture2D iconLeft = null, Texture2D iconRight = null, IconMenuAlignment iconAlignment = IconMenuAlignment.Left, int iconSpacing = DEFAULT_SPACING)
        {
            IconAlignment = iconAlignment;
            _iconSpacing = iconSpacing;
            // Création des Sprites des icones et mise à jour des dimensions du groupe
            Vector2 groupDim = _itemsGroup.Dimensions;
            if (iconLeft != null)
            {
                _iconLeft = new Sprite(iconLeft, Color.White);
                groupDim.X += _iconLeft.Dimensions.X + _iconSpacing;
            }
            if (iconRight != null)
            {
                _iconRight = new Sprite(iconRight, Color.White);
                groupDim.X += _iconLeft.Dimensions.X + _iconSpacing;
            }
            _itemsGroup.Dimensions = groupDim;
            // Mise à jour des positions des items
            foreach (var item in _items)
            {
                Vector2 itemPos = item.Position;
                if (iconLeft != null)
                    itemPos.X += _iconLeft.Dimensions.X + _iconSpacing;
                item.Position = itemPos;
            }
            if (_background != null)
                _background.Dimensions = _itemsGroup.Dimensions;
        }


        // Items
        public MenuItem AddItem(SpriteFont font, string text, Color color, Func<MenuItem, MenuItem> selection = null, Func<MenuItem, MenuItem> deselection = null, Func<MenuItem, MenuItem> activation = null, HorizontalAlignment halign = HorizontalAlignment.Left, VerticalAlignment valign = VerticalAlignment.Top)
        {
            float item_pos_y = _itemsGroup.Dimensions.Y + (_itemsGroup.Count() > 0 ? _itemspacing : 0.0f);
            MenuItem menuitem = new MenuItem(font, text, color, selection, deselection, activation, new Vector2(_itemsGroup.Position.X, item_pos_y), halign, valign);
            _itemsGroup.Add(menuitem);
            _itemsGroup.Dimensions = new Vector2(_itemsGroup.Dimensions.X, item_pos_y + menuitem.Dimensions.Y);
            _items.Add(menuitem);
            SortElements();
            if (_currentitemindex >= 0 && _currentitemindex == _items.Count - 1)
                menuitem.Selection?.Invoke(menuitem);
            return menuitem;
        }


        // Touches
        public void SetKeys(ControllerKey nextItemKey, ControllerKey prevItemKey, ControllerKey activateItemKey, ControllerKey cancelKey = null)
        {
            SetNextItemKey(nextItemKey);
            SetPreviousItemKey(prevItemKey);
            SetActivateItemKey(activateItemKey);
            SetCancelMenuKey(cancelKey);
        }
        public void SetNextItemKey(ControllerKey key) { _next_item_key = key; }
        public void SetPreviousItemKey(ControllerKey key) { _previous_item_key = key; }
        public void SetActivateItemKey(ControllerKey key) { _active_item_key = key; }
        public void SetCancelMenuKey(ControllerKey key) { _cancel_menu_key = key; }


        // Items Background
        public void SetItemsBackground(Panel panel, int borderSpacing)
        {
            bool bkgnotexist = (_background == null);
            _background = panel;
            _borderSpacing = borderSpacing;
            if (bkgnotexist)
                _itemsGroup.Add(_background);
            _background.Visible = true;

            // Mise à jour de la position du panneau
            _background.Position = _itemsGroup.Position;

            // Mise à jour des dimensions du panneau
            _itemsGroup.Dimensions = new Vector2(_itemsGroup.Dimensions.X + borderSpacing * 2.0f, _itemsGroup.Dimensions.Y + borderSpacing * 2.0f);
            _background.Dimensions = _itemsGroup.Dimensions;

            // Mise à jour du ZOrder du panneau
            int index = 0;
            int min_zorder = 0;
            foreach (var item in _itemsGroup)
            {
                if (item.GetType() != typeof(Panel))
                {
                    MenuItem menuitem = (MenuItem)item;
                    if ((index == 0 || min_zorder > menuitem.ZOrder))
                        min_zorder = ((MenuItem)item).ZOrder;
                    // Mise à jour des positions des items
                    menuitem.Position = new Vector2(menuitem.Position.X + borderSpacing, menuitem.Position.Y + borderSpacing);
                }
                index++;
            }
            _background.ZOrder = min_zorder - 1;
            _itemsGroup.SortElements();
        }
        public void SetBackgroundVisible(bool visible)
        {
            if (_background != null)
                _background.Visible = visible;
        }


        public void Reset(int value = -1)
        {
            CurrentItem?.Deselection?.Invoke(CurrentItem);
            _currentitemindex = value;
            if (_currentitemindex >= 0)
                CurrentItem?.Selection?.Invoke(CurrentItem);
        }



        public void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            if (_cancel_menu_key != null && _cancel_menu_key.IsPressed())
            {
                Cancellation?.Invoke();
                Reset();
                Visible = false;
                return;
            }
            if (_next_item_key != null && _next_item_key.IsPressed())
                ChangeCurrentItem(1);
            if (_previous_item_key != null && _previous_item_key.IsPressed())
                ChangeCurrentItem(-1);

            if (_active_item_key != null && _active_item_key.IsPressed())
            {
                if (_currentitemindex >= 0 && _currentitemindex < _items.Count)
                    _items[_currentitemindex].Activation?.Invoke(_items[_currentitemindex]);
            }
            foreach (var element in _elements)
            {
                if (element is IUpdate update)
                    update.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (Visible)
            {
                foreach (var element in _elements)
                {
                    if (element is IDraw draw)
                        draw.Draw(spritebatch);
                }
                if (_currentitemindex >= 0)
                {
                    if (_iconLeft != null && (IconAlignment == IconMenuAlignment.Left || IconAlignment == IconMenuAlignment.Both))
                    {
                        _iconLeft.Position = new Vector2(_items[_currentitemindex].Position.X - _iconLeft.Dimensions.X - _iconSpacing,
                                                         _items[_currentitemindex].Position.Y + (_items[_currentitemindex].Dimensions.Y / 2.0f) - (_iconLeft.Dimensions.Y / 2.0f));
                        _iconLeft.Draw(spritebatch);
                    }
                    if (_iconRight != null && (IconAlignment == IconMenuAlignment.Right || IconAlignment == IconMenuAlignment.Both))
                    {
                        float iconRightPos;
                        if (_background != null)
                            iconRightPos = _background.Position.X + _background.Dimensions.X - _iconRight.Dimensions.X - _borderSpacing;
                        else
                            iconRightPos = _itemsGroup.Position.X + _itemsGroup.Dimensions.X - _iconRight.Dimensions.X;

                        _iconRight.Position = new Vector2(iconRightPos,
                                                          _items[_currentitemindex].Position.Y + (_items[_currentitemindex].Dimensions.Y / 2.0f) - (_iconRight.Dimensions.Y / 2.0f));
                        _iconRight.Draw(spritebatch);
                    }
                }
            }
        }

        // Méthodes privées
        private void ChangeCurrentItem(int offset)
        {
            // Désélection de l'ancien item
            if (_currentitemindex >= 0 && _currentitemindex < _items.Count)
                _items[_currentitemindex].Deselection?.Invoke(_items[_currentitemindex]);
            // Changement de l'index de l'item
            _currentitemindex += offset;
            if (_currentitemindex >= _items.Count)
                _currentitemindex = 0;
            else if (_currentitemindex < 0)
                _currentitemindex = _items.Count - 1;
            // Sélection du nouvel item
            _items[_currentitemindex].Selection?.Invoke(_items[_currentitemindex]);
        }
        private void SortElements()
        {
            _elements.Sort(delegate (IElement e1, IElement e2)
            {
                if (e1.ZOrder < e2.ZOrder)
                    return -1;
                if (e1.ZOrder > e2.ZOrder)
                    return 1;
                return 0;
            });
        }
    }
}
