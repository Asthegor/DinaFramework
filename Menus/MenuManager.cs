using DinaFramework.Controls;
using DinaFramework.Core;
using DinaFramework.Core.Fixed;
using DinaFramework.Enums;
using DinaFramework.Interfaces;
using DinaFramework.Scenes;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DinaFramework.Menus
{
    public class MenuManager : IDraw, IUpdate, IVisible
    {
        private const int DEFAULT_SPACING = 5;
        private int _iconSpacing = DEFAULT_SPACING;
        private readonly List<IElement> _elements;
        private readonly List<object> _titles;
        private readonly Group _itemsGroup;
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
        private MouseState _oldMouseState;

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


        //------------------------------------------------------------------
        // Propriétés
        public Vector2 ItemsDimensions => _itemsGroup.Dimensions;
        public Vector2 ItemsPosition
        {
            get => _itemsGroup.Position;
            set => _itemsGroup.Position = value;
        }
        public int CurrentItemIndex => _currentitemindex;
        public MenuItem CurrentItem
        {
            get => (_currentitemindex == -1 || _currentitemindex >= _items.Count) ? null : _items[_currentitemindex];
            set => _currentitemindex = _items.IndexOf(value);
        }
        public IconMenuAlignment IconAlignment
        {
            get => _iconAlignment;
            set => _iconAlignment = value;
        }
        public bool IconsVisible
        {
            get => (_iconLeft != null && _iconLeft.Visible) || (_iconRight != null && _iconRight.Visible);
            set
            {
                _iconLeft?.SetVisible(value);
                _iconRight?.SetVisible(value);
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
        public void AddTitle(SpriteFont font, string text, Vector2 position, Color color, Color? shadowcolor = null, Vector2? shadowoffset = null, int zorder = 0)
        {
            var title = (shadowcolor.HasValue && shadowoffset.HasValue)
                        ? new ShadowText(font, text, color, position, shadowcolor.Value, shadowoffset.Value, zorder: zorder)
                        : (IElement)new Text(font, text, color, position, zorder: zorder);
            AddTitleToGroups(title);
        }
        public void CenterTitles(Vector2 screendimensions)
        {
            foreach (var title in _titles)
            {
                if (title is IText titleText)
                {
                    Vector2 titleTextDim = titleText.TextDimensions;
                    Vector2 titlePos = titleText.Position;
                    titlePos.X = (screendimensions.X - titleTextDim.X) / 2.0f;
                    titleText.Position = titlePos;
                }
            }
        }

        //------------------------------------------------------------------
        // Icones
        public void SetIconItems(Texture2D iconLeft = null, Texture2D iconRight = null,
                                 IconMenuAlignment iconAlignment = IconMenuAlignment.Left,
                                 int iconSpacing = DEFAULT_SPACING,
                                 bool resize = false)
        {
            IconAlignment = iconAlignment;
            _iconSpacing = iconSpacing;

            // Calcule les dimensions des icônes si le redimensionnement est activé
            Vector2 iconLeftDimensions = CalculateIconDimensions(iconLeft, resize);
            Vector2 iconRightDimensions = CalculateIconDimensions(iconRight, resize);

            // Crée les objets Sprite pour les icônes gauche et droite
            _iconLeft = CreateIconSprite(iconLeft, iconLeftDimensions);
            _iconRight = CreateIconSprite(iconRight, iconRightDimensions);

            // Met à jour les dimensions du fond si nécessaire
            if (_background != null)
            {
                Vector2 offset = Vector2.Zero;
                if (_iconLeft != null)
                    offset.X += _iconLeft.Dimensions.X + iconSpacing;
                if (_iconRight != null)
                    offset.X += _iconRight.Dimensions.X + iconSpacing;
                _background.Dimensions = _itemsGroup.Dimensions + offset;
            }
        }


        //------------------------------------------------------------------
        // Items
        public MenuItem AddItem(SpriteFont font, string text, Color color, Func<MenuItem, MenuItem> selection = null, Func<MenuItem, MenuItem> deselection = null, Func<MenuItem, MenuItem> activation = null, HorizontalAlignment halign = HorizontalAlignment.Left, VerticalAlignment valign = VerticalAlignment.Top)
        {
            return AddItemToGroups(new MenuItem(font, text, color, selection, deselection, activation, new Vector2(_itemsGroup.Position.X, GetNextItemYPosition()), halign, valign));
        }
        public MenuItem AddItem(object item, Func<MenuItem, MenuItem> selection = null, Func<MenuItem, MenuItem> deselection = null, Func<MenuItem, MenuItem> activation = null)
        {
            return AddItemToGroups(new MenuItem(item, selection, deselection, activation, new Vector2(_itemsGroup.Position.X, GetNextItemYPosition())));
        }
        public void CenterMenuItems(Vector2 screendimensions)
        {
            foreach (var item in _items)
            {
                if (item is MenuItem)
                {
                    Vector2 itemTextDim = item.TextDimensions;
                    Vector2 itemPos = item.Position;
                    itemPos.X = (screendimensions.X - itemTextDim.X) / 2.0f;
                    item.Position = itemPos;
                }
            }
        }

        //------------------------------------------------------------------
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


        //------------------------------------------------------------------
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
            _background?.SetVisible(visible);
        }

        //------------------------------------------------------------------
        public void Reset(int value = -1)
        {
            Reset(_items.ElementAtOrDefault(value));
        }
        public void Reset(MenuItem item)
        {
            CurrentItem?.Deselection?.Invoke(CurrentItem);
            CurrentItem = item;
            CurrentItem?.Selection?.Invoke(CurrentItem);
            _oldMouseState = Mouse.GetState();
        }
        public void Update(GameTime gameTime)
        {
            if (!Visible)
                return;

            HandleMouseInput();
            HandleKeyInput();

            foreach (var element in _elements)
            {
                if (element is IUpdate update)
                    update.Update(gameTime);
            }
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            foreach (var element in _elements)
            {
                if (element is IDraw draw)
                    draw.Draw(spritebatch);
            }

            DrawIcons(spritebatch);
        }


        //------------------------------------------------------------------
        // Méthodes privées
        private void AddTitleToGroups(IElement title)
        {
            _elements.Add(title);
            _titles.Add(title);
            SortElements();
        }
        private MenuItem AddItemToGroups(MenuItem menuitem)
        {
            _itemsGroup.Add(menuitem);
            _items.Add(menuitem);
            SortElements();
            if (_currentitemindex >= 0 && _currentitemindex == _items.Count - 1)
                menuitem.Selection?.Invoke(menuitem);
            return menuitem;
        }
        private Vector2 CalculateIconDimensions(Texture2D icon, bool resize)
        {
            if (icon == null)
                return Vector2.Zero;

            Vector2 iconDim = new Vector2(icon.Width, icon.Height);
            if (resize && _items.Count > 0)
            {
                Vector2 itemDim = _items[0].Dimensions;
                float ratio = iconDim.Y / itemDim.Y;
                return iconDim / ratio;
            }
            return iconDim;
        }
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
        private static Sprite CreateIconSprite(Texture2D texture, Vector2 dimensions)
        {
            return texture != null ? new Sprite(texture, Color.White) { Dimensions = dimensions } : null;
        }
        private void DrawIcons(SpriteBatch spritebatch)
        {
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
                    float iconRightPos = _background != null ? _background.Position.X + _background.Dimensions.X - _iconRight.Dimensions.X - _borderSpacing
                                                             : _itemsGroup.Position.X + _itemsGroup.Dimensions.X - _iconRight.Dimensions.X;

                    _iconRight.Position = new Vector2(iconRightPos,
                                                      _items[_currentitemindex].Position.Y + (_items[_currentitemindex].Dimensions.Y / 2.0f) - (_iconRight.Dimensions.Y / 2.0f));
                    _iconRight.Draw(spritebatch);
                }
            }

        }
        private float GetNextItemYPosition()
        {
            return _itemsGroup.Dimensions.Y + (_itemsGroup.Count() > 0 ? DEFAULT_SPACING : 0.0f);
        }
        private void HandleMouseInput()
        {
            MouseState ms = Mouse.GetState();
            SceneManager sm = SceneManager.GetInstance();
            if (sm != null && sm.IsMouseVisible == true)
            {
                foreach (MenuItem item in _items)
                {
                    Rectangle rect = new Rectangle(item.Position.ToPoint(), item.Dimensions.ToPoint());
                    if (_oldMouseState.Position != ms.Position && rect.Intersects(new Rectangle(new Point(ms.X, ms.Y), Point.Zero)))
                    {
                        if (CurrentItem != item)
                        {
                            CurrentItem?.Deselection?.Invoke(CurrentItem);
                            item?.Selection?.Invoke(item);
                            CurrentItem = item;
                        }
                    }
                    if (_oldMouseState.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Released && rect.Intersects(new Rectangle(new Point(ms.X, ms.Y), Point.Zero)))
                    {
                        item?.Activation.Invoke(item);
                        _oldMouseState = ms;
                        return;
                    }
                }
                if (_oldMouseState.RightButton == ButtonState.Pressed && ms.RightButton == ButtonState.Released)
                {
                    if (Cancellation != null)
                    {
                        Reset();
                        Visible = false;
                        Cancellation.Invoke();
                        _oldMouseState = ms;
                        return;
                    }
                }
            }
            _oldMouseState = ms;
        }
        private void HandleKeyInput()
        {
            if (_cancel_menu_key != null && _cancel_menu_key.IsPressed() == 1)
            {
                if (Cancellation != null)
                {
                    Reset();
                    Visible = false;
                    Cancellation.Invoke();
                    return;
                }
            }
            if (_next_item_key != null && _next_item_key.IsPressed() == 1)
                ChangeCurrentItem(1);
            if (_previous_item_key != null && _previous_item_key.IsPressed() == 1)
                ChangeCurrentItem(-1);

            if (_active_item_key != null && _active_item_key.IsPressed() == 1)
            {
                if (_currentitemindex >= 0 && _currentitemindex < _items.Count)
                    _items[_currentitemindex].Activation?.Invoke(_items[_currentitemindex]);
            }
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
