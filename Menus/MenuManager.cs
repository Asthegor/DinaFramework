using DinaFramework.Controls;
using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Graphics;
using DinaFramework.Interfaces;
using DinaFramework.Scenes;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DinaFramework.Menus
{
    /// <summary>
    /// Gère l'affichage et l'interaction avec le menu, incluant les éléments, les titres, les icônes et les interactions clavier/souris.
    /// </summary>
    public class MenuManager : IDraw, IUpdate, IVisible
    {
        private static Vector2 DEFAULT_SPACING = new Vector2(5, 5);
        private static SceneManager SceneManager => ServiceLocator.Get<SceneManager>(ServiceKey.SceneManager);
        private Vector2 _iconSpacing = DEFAULT_SPACING;
        private List<IElement> _elements = [];
        private List<IText> _titles = [];
        private Group _itemsGroup = [];
        private List<MenuItem> _items = [];
        private readonly Vector2 _itemspacing;
        private MenuItemDirection _direction;
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
        private Vector2 _borderSpacing = DEFAULT_SPACING;
        private MouseState _oldMouseState;
        private bool _centeredItems;
        private bool _centeredTitles;
        private Vector2 _titleScreenDimensions;
        private Vector2 _itemsScreenDimensions;

        // Construteurs
        /// <summary>
        /// Initialise une nouvelle instance de MenuManager.
        /// </summary>
        /// <param name="itemspacing">Espace entre les éléments du menu (par défaut : 5).</param>
        /// <param name="cancellation">Action à exécuter lors de l'annulation du menu.</param>
        /// <param name="currentitemindex">Index de l'élément sélectionné au départ (par défaut : -1).</param>
        /// <param name="direction">Sens du menu : vertical (haut/bas), horizontal (droite/gauche). Par défaut, Vertical.</param>
        public MenuManager(Vector2 itemspacing = new Vector2(), Action cancellation = null, int currentitemindex = -1, MenuItemDirection direction = MenuItemDirection.Vertical)
        {
            _itemspacing = itemspacing;
            _elements.Add(_itemsGroup);
            Visible = true;
            Cancellation = cancellation;
            _centeredItems = false;
            _centeredTitles = false;
            _titleScreenDimensions = Vector2.Zero;
            _itemsScreenDimensions = Vector2.Zero;
            _direction = direction;
            Reset(currentitemindex);
        }


        //------------------------------------------------------------------
        // Propriétés
        /// <summary>
        /// Obtient les dimensions des items du menu.
        /// </summary>
        public Vector2 ItemsDimensions => _itemsGroup.Dimensions;
        /// <summary>
        /// Obtient ou définit la position des items du menu.
        /// </summary>
        public Vector2 ItemsPosition
        {
            get => _itemsGroup.Position;
            set => _itemsGroup.Position = value;
        }
        /// <summary>
        /// Obtient l'indice de l'élément actuellement sélectionné.
        /// </summary>
        public int CurrentItemIndex => _currentitemindex;
        /// <summary>
        /// Obtient ou définit l'élément actuellement sélectionné.
        /// </summary>
        public MenuItem CurrentItem
        {
            get => (_currentitemindex == -1 || _currentitemindex >= _items.Count) ? null : _items[_currentitemindex];
            set
            {
                CurrentItem?.Deselection?.Invoke(CurrentItem);
                _currentitemindex = _items.IndexOf(value);
                CurrentItem?.Selection?.Invoke(CurrentItem);
            }
        }
        /// <summary>
        /// Obtient ou définit l'alignement des icônes dans le menu.
        /// </summary>
        public IconMenuAlignment IconAlignment
        {
            get => _iconAlignment;
            set => _iconAlignment = value;
        }
        /// <summary>
        /// Obtient ou définit la visibilité des icônes dans le menu.
        /// </summary>
        public bool IconsVisible
        {
            get => (_iconLeft != null && _iconLeft.Visible) || (_iconRight != null && _iconRight.Visible);
            set
            {
                _iconLeft?.SetVisible(value);
                _iconRight?.SetVisible(value);
            }
        }
        /// <summary>
        /// Obtient ou définit l'action d'annulation du menu (quitter le menu).
        /// </summary>
        public Action Cancellation
        {
            get => _cancellation;
            set => _cancellation = value;
        }
        /// <summary>
        /// Obtient ou définit la visibilité du menu.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }

        // Titres
        /// <summary>
        /// Ajoute un titre au menu.
        /// </summary>
        /// <param name="font">Police du titre.</param>
        /// <param name="text">Texte du titre.</param>
        /// <param name="position">Position du titre.</param>
        /// <param name="color">Couleur du titre.</param>
        /// <param name="shadowcolor">Couleur de l'ombre (facultatif).</param>
        /// <param name="shadowoffset">Décalage de l'ombre (facultatif).</param>
        /// <param name="zorder">Ordre de superposition du titre.</param>
        /// <returns>L'élément titre ajouté.</returns>
        public IText AddTitle(SpriteFont font, string text, Vector2 position, Color color, Color? shadowcolor = null, Vector2? shadowoffset = null, int zorder = 0)
        {
            IText title;
            if (shadowcolor.HasValue && shadowoffset.HasValue)
                title = new ShadowText(font, text, color, position, shadowcolor.Value, shadowoffset.Value, zorder: zorder);
            else
                title = new DFText(font, text, color, position, zorder: zorder);
            AddTitleToGroups(title);
            return title;
        }
        /// <summary>
        /// Ajoute un titre à partir d'un élément déjà créé.
        /// </summary>
        /// <param name="title">L'élément titre à ajouter.</param>
        /// <returns>L'élément titre ajouté.</returns>
        public IText AddTitle(IText title)
        {
            AddTitleToGroups(title);
            return title;
        }
        /// <summary>
        /// Centre les titres sur l'écran.
        /// </summary>
        /// <param name="screendimensions">Dimensions de l'écran.</param>
        public void CenterTitles(Vector2 screendimensions)
        {
            _centeredTitles = true;
            _titleScreenDimensions = screendimensions;
            foreach (var title in _titles)
            {
                if (title is IElement titleElement)
                {
                    Vector2 titleTextDim = (titleElement is IText titleText) ? titleText.TextDimensions : titleElement.Dimensions;
                    Vector2 titlePos = titleElement.Position;
                    titlePos.X = (screendimensions.X - titleTextDim.X) / 2.0f;
                    titleElement.Position = titlePos;
                }
            }
        }
        /// <summary>
        /// Modifie la police des titres.
        /// </summary>
        /// <param name="font">La police à utiliser.</param>
        public void SetTitleFont(SpriteFont font)
        {
            foreach (var title in _titles)
            {
                if (title is DFText titletext)
                    titletext.Font = font;
                if (title is ShadowText titleshadowtext)
                    titleshadowtext.Font = font;
            }
            if (_centeredTitles)
                CenterTitles(_titleScreenDimensions);
        }

        //------------------------------------------------------------------
        // Icones
        /// <summary>
        /// Définit les icônes du menu, incluant leur alignement et leur espacement.
        /// </summary>
        /// <param name="iconLeft">Icône gauche.</param>
        /// <param name="iconRight">Icône droite.</param>
        /// <param name="iconAlignment">Alignement des icônes.</param>
        /// <param name="iconSpacing">Espacement entre les icônes.</param>
        /// <param name="resize">Indique si les icônes doivent être redimensionnées.</param>
        public void SetIconItems(Texture2D iconLeft = null, Texture2D iconRight = null,
                                 IconMenuAlignment iconAlignment = IconMenuAlignment.Left,
                                 Vector2 iconSpacing = new Vector2(),
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
                float totalOffset = 0f;
                float leftIconOffset = 0f;

                if (_direction == MenuItemDirection.Vertical)
                {
                    if (_iconLeft != null)
                    {
                        totalOffset += _iconLeft.Dimensions.X + iconSpacing.X;
                        leftIconOffset = _iconLeft.Dimensions.X;
                    }
                    if (_iconRight != null)
                        totalOffset += _iconRight.Dimensions.X + iconSpacing.X;

                    _background.Dimensions += new Vector2(totalOffset, 0);
                    _background.Position -= new Vector2(leftIconOffset, 0);
                }
                else if (_direction == MenuItemDirection.Horizontal)
                {
                    if (_iconLeft != null)
                    {
                        totalOffset += _iconLeft.Dimensions.Y + iconSpacing.Y;
                        leftIconOffset = _iconLeft.Dimensions.X;
                    }
                    if (_iconRight != null)
                        totalOffset += _iconRight.Dimensions.Y + iconSpacing.Y;

                    _background.Dimensions += new Vector2(0, totalOffset);
                    _background.Position -= new Vector2(leftIconOffset, 0);
                }
            }
        }


        //------------------------------------------------------------------
        // Items
        /// <summary>
        /// Ajoute un élément au menu.
        /// </summary>
        /// <param name="font">Police du texte de l'élément.</param>
        /// <param name="text">Texte de l'élément.</param>
        /// <param name="color">Couleur du texte.</param>
        /// <param name="selection">Fonction appelée lors de la sélection.</param>
        /// <param name="deselection">Fonction appelée lors de la désélection.</param>
        /// <param name="activation">Fonction appelée lors de l'activation.</param>
        /// <param name="halign">Alignement horizontal de l'élément.</param>
        /// <param name="valign">Alignement vertical de l'élément.</param>
        /// <returns>L'élément menu ajouté.</returns>
        public MenuItem AddItem(SpriteFont font, string text, Color color, Func<MenuItem, MenuItem> selection = null, Func<MenuItem, MenuItem> deselection = null, Func<MenuItem, MenuItem> activation = null, HorizontalAlignment halign = HorizontalAlignment.Left, VerticalAlignment valign = VerticalAlignment.Top)
        {
            Vector2 pos = Vector2.Zero;
            if (_direction == MenuItemDirection.Vertical)
                pos = new Vector2(_itemsGroup.Position.X, GetNextItemYPosition());
            else
            {
                pos = new Vector2(GetNextItemXPosition(), _itemsGroup.Position.Y);
            }
            return AddItemToGroups(new MenuItem(font, text, color, selection, deselection, activation, pos, halign, valign));
        }
        /// <summary>
        /// Ajoute un élément de menu à partir d'un objet déjà créé.
        /// </summary>
        /// <param name="item">L'élément à ajouter.</param>
        /// <param name="selection">Fonction appelée lors de la sélection de l'élément.</param>
        /// <param name="deselection">Fonction appelée lors de la désélection de l'élément.</param>
        /// <param name="activation">Fonction appelée lors de l'activation de l'élément.</param>
        /// <returns>L'élément de menu ajouté.</returns>
        public MenuItem AddItem(object item, Func<MenuItem, MenuItem> selection = null, Func<MenuItem, MenuItem> deselection = null, Func<MenuItem, MenuItem> activation = null)
        {
            ArgumentNullException.ThrowIfNull(item, "Parameter 'item' must not be null.");

            return AddItemToGroups(new MenuItem(item, selection, deselection, activation, (item as IPosition).Position));
        }
        /// <summary>
        /// Centre les éléments de menu sur l'écran et 
        /// ajuste tous les items du menu à être centrés :
        /// - soit horizontalement si la direction est verticale
        /// - soit verticalement si la direction est horizontale
        /// </summary>
        /// <param name="screendimensions">Dimensions de l'écran pour le centrage.</param>
        public void CenterMenuItems(Vector2 screendimensions)
        {
            _centeredItems = true;
            _itemsScreenDimensions = screendimensions;
            _itemsGroup.Position = (screendimensions - _itemsGroup.Dimensions) / 2;
            foreach (MenuItem item in _items)
            {
                if (item is null)
                    continue;

                Vector2 itemTextDim = item.Dimensions;
                Vector2 itemPos = item.Position;
                if (_direction == MenuItemDirection.Horizontal)
                    itemPos.Y = (screendimensions.Y - itemTextDim.Y) / 2.0f;
                else
                    itemPos.X = (screendimensions.X - itemTextDim.X) / 2.0f;
                item.Position = itemPos;
            }
        }
        /// <summary>
        /// Définit la police des éléments de menu.
        /// </summary>
        /// <param name="font">La police à appliquer aux éléments de menu.</param>
        public void SetItemFont(SpriteFont font)
        {
            foreach (MenuItem item in _items)
            {
                if (item is null)
                    continue;
                item.Font = font;
            }
            if (_centeredItems)
                CenterMenuItems(_itemsScreenDimensions);
        }

        //------------------------------------------------------------------
        // Touches
        /// <summary>
        /// Définit les touches permettant de naviguer dans le menu.
        /// </summary>
        /// <param name="nextItemKey">Touche pour sélectionner l'élément suivant.</param>
        /// <param name="prevItemKey">Touche pour sélectionner l'élément précédent.</param>
        /// <param name="activateItemKey">Touche pour activer l'élément sélectionné.</param>
        /// <param name="cancelKey">Touche pour annuler le menu (facultatif).</param>
        public void SetKeys(ControllerKey nextItemKey, ControllerKey prevItemKey, ControllerKey activateItemKey, ControllerKey cancelKey = null)
        {
            SetNextItemKey(nextItemKey);
            SetPreviousItemKey(prevItemKey);
            SetActivateItemKey(activateItemKey);
            SetCancelMenuKey(cancelKey);
        }
        /// <summary>
        /// Définit la touche pour sélectionner l'élément suivant.
        /// </summary>
        /// <param name="key">Touche à associer à la sélection de l'élément suivant.</param>
        public void SetNextItemKey(ControllerKey key) { _next_item_key = key; }
        /// <summary>
        /// Définit la touche pour sélectionner l'élément précédent.
        /// </summary>
        /// <param name="key">Touche à associer à la sélection de l'élément précédent.</param>
        public void SetPreviousItemKey(ControllerKey key) { _previous_item_key = key; }
        /// <summary>
        /// Définit la touche pour activer/valider l'élément sélectionné.
        /// </summary>
        /// <param name="key">Touche à associer à l'activation (validation) de l'élément sélectionné.</param>
        public void SetActivateItemKey(ControllerKey key) { _active_item_key = key; }
        /// <summary>
        /// Définit la touche pour annuler le menu (facultatif).
        /// </summary>
        /// <param name="key">Touche à associer à l'annulation du menu.</param>
        public void SetCancelMenuKey(ControllerKey key) { _cancel_menu_key = key; }


        //------------------------------------------------------------------
        // Items Background
        /// <summary>
        /// Définit l'arrière-plan des éléments du menu.
        /// </summary>
        /// <param name="panel">Panneau à utiliser comme fond pour les éléments.</param>
        /// <param name="borderSpacing">Espacement des bordures autour de l'arrière-plan.</param>
        public void SetItemsBackground(Panel panel, Vector2 borderSpacing)
        {
            //bool bkgnotexist = (_background == null);
            _background = panel;
            _borderSpacing = borderSpacing;
            _background.Visible = true;

            // Mise à jour de la position du panneau
            _background.Position = _itemsGroup.Position - new Vector2(borderSpacing.X, borderSpacing.Y);
            if (_iconLeft != null)
                _background.Position -= new Vector2(_iconLeft.Dimensions.X, 0);

            // Mise à jour des dimensions du panneau
            _background.Dimensions = _itemsGroup.Dimensions + borderSpacing * 2.0f;

            if (_iconRight != null)
                _background.Dimensions += new Vector2(_iconRight.Dimensions.X, 0);

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
                }
                index++;
            }
            _background.ZOrder = min_zorder - 1;
            _itemsGroup.SortElements();
        }
        /// <summary>
        /// Définit la visibilité de l'arrière-plan des éléments.
        /// </summary>
        /// <param name="visible">Si true, rend l'arrière-plan visible ; sinon, invisible.</param>
        public void SetBackgroundVisible(bool visible)
        {
            _background?.SetVisible(visible);
        }

        //------------------------------------------------------------------
        /// <summary>
        /// Réinitialise l'état de l'élément de menu courant à partir d'un index spécifique.
        /// Si l'index est invalide (par défaut -1), la réinitialisation se fait à un élément par défaut.
        /// </summary>
        /// <param name="value">L'index de l'élément à sélectionner pour réinitialiser. Par défaut -1, ce qui signifie sélectionner l'élément par défaut.</param>
        /// <remarks>
        /// Cette méthode permet de remettre à zéro l'élément sélectionné actuel, en appelant la méthode de sélection/désélection sur l'élément précédent et sur le nouvel élément.
        /// </remarks>
        public void Reset(int value = -1)
        {
            Reset(_items.ElementAtOrDefault(value));
        }
        /// <summary>
        /// Réinitialise l'état de l'élément de menu courant à partir d'un élément spécifique.
        /// Effectue la sélection et la désélection de l'élément courant, en utilisant les actions définies dans l'élément.
        /// </summary>
        /// <param name="item">L'élément de menu à sélectionner.</param>
        /// <remarks>
        /// Cette méthode met à jour l'élément sélectionné actuel, en invoquant les actions de sélection et de désélection définies sur les éléments.
        /// </remarks>
        public void Reset(MenuItem item)
        {
            CurrentItem?.Deselection?.Invoke(CurrentItem);
            CurrentItem = item;
            CurrentItem?.Selection?.Invoke(CurrentItem);
            _oldMouseState = Mouse.GetState();
        }
        /// <summary>
        /// Met à jour l'état du menu en fonction des entrées utilisateur et des éléments d'interface.
        /// Elle gère les entrées de souris et de clavier, et met à jour chaque élément nécessitant une mise à jour.
        /// </summary>
        /// <param name="gametime">Les informations sur le temps écoulé depuis la dernière mise à jour (GameTime).</param>
        /// <remarks>
        /// Cette méthode vérifie si le menu est visible, puis gère les interactions via la souris et le clavier.
        /// Elle met également à jour tous les éléments qui implémentent l'interface IUpdate.
        /// </remarks>
        public void Update(GameTime gametime)
        {
            if (!Visible)
                return;

            HandleMouseInput();
            HandleKeyInput();

            foreach (var element in _elements)
            {
                if (element is IUpdate update)
                    update.Update(gametime);
            }
        }
        /// <summary>
        /// Affiche le menu à l'écran.
        /// </summary>
        /// <param name="spritebatch">Objet utilisé pour dessiner le menu.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            _background?.Draw(spritebatch);

            foreach (var element in _elements)
            {
                if (element is IDraw draw)
                    draw.Draw(spritebatch);
            }

            DrawIcons(spritebatch);
        }


        //------------------------------------------------------------------
        // Méthodes privées
        private void AddTitleToGroups(IText title)
        {
            _elements.Add((IElement)title);
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

            int startindex = _currentitemindex;
            // Changement de l'index de l'item
            do
            {
                _currentitemindex += offset;
                if (_currentitemindex >= _items.Count)
                    _currentitemindex = 0;
                else if (_currentitemindex < 0)
                    _currentitemindex = _items.Count - 1;
                if (_currentitemindex == startindex)
                    break;
            }
            while (_items[_currentitemindex].State == MenuItemState.Disable);

            // Sélection du nouvel item
            _items[_currentitemindex].Selection?.Invoke(_items[_currentitemindex]);
        }
        private static Sprite CreateIconSprite(Texture2D texture, Vector2 dimensions)
        {
            return texture != null ? new Sprite(texture, Color.White, Vector2.Zero, new Rectangle(Point.Zero, dimensions.ToPoint()), Vector2.Zero, Vector2.One, 0, Vector2.One, SpriteEffects.None, 0) : null;
        }
        private void DrawIcons(SpriteBatch spritebatch)
        {
            if (_currentitemindex >= 0)
            {
                if (_iconLeft != null && (IconAlignment == IconMenuAlignment.Left || IconAlignment == IconMenuAlignment.Both))
                {
                    _iconLeft.Position = new Vector2(_items[_currentitemindex].Position.X - _iconLeft.Dimensions.X - _iconSpacing.X,
                                                     _items[_currentitemindex].Position.Y + (_items[_currentitemindex].Dimensions.Y / 2.0f) - (_iconLeft.Dimensions.Y / 2.0f));
                    _iconLeft.Draw(spritebatch);
                }
                if (_iconRight != null && (IconAlignment == IconMenuAlignment.Right || IconAlignment == IconMenuAlignment.Both))
                {
                    _iconRight.Position = new Vector2(_items[_currentitemindex].Position.X + _items[_currentitemindex].TextDimensions.X + _iconSpacing.X,
                                                      _items[_currentitemindex].Position.Y + (_items[_currentitemindex].TextDimensions.Y - _iconRight.Dimensions.Y) / 2.0f);
                    _iconRight.Draw(spritebatch);
                }
            }

        }
        private float GetNextItemYPosition()
        {
            return _itemsGroup.Dimensions.Y + (_itemsGroup.Count() > 0 ? _itemspacing.Y : 0.0f);
        }
        private float GetNextItemXPosition()
        {
            return _itemsGroup.Dimensions.X + (_itemsGroup.Count() > 0 ? _itemspacing.X : 0.0f);
        }
        private void HandleMouseInput()
        {
            MouseState ms = Mouse.GetState();

            bool mouseVisible = SceneManager?.IsMouseVisible == true;
            bool mouseMoved = ms != _oldMouseState;
            if (!mouseVisible && !mouseMoved)
            {
                _oldMouseState = ms;
                return;
            }

            Point mousePos = ms.Position;
            Rectangle mouseRect = new(mousePos, Point.Zero);

            Rectangle menuItemsGroupRect = new Rectangle(_itemsGroup.Position.ToPoint(), _itemsGroup.Dimensions.ToPoint());
            MenuItem hoveredItem = null;
            if (menuItemsGroupRect.Contains(mousePos))
            {
                foreach (MenuItem item in _items)
                {
                    Rectangle itemRect = new(item.Position.ToPoint(), item.Dimensions.ToPoint());
                    if (item.State != MenuItemState.Disable && itemRect.Intersects(mouseRect))
                    {
                        hoveredItem = item;
                        break;
                    }
                }
            }

            if (CurrentItem != hoveredItem)
            {
                CurrentItem?.Deselection?.Invoke(CurrentItem);
                if (hoveredItem != null)
                    hoveredItem.Selection?.Invoke(hoveredItem);
                CurrentItem = hoveredItem;
            }

            bool clicked = _oldMouseState.LeftButton == ButtonState.Pressed && ms.LeftButton == ButtonState.Released;
            if (clicked)
            {
                CurrentItem?.Activation?.Invoke(CurrentItem);
                _oldMouseState = ms;
                return;
            }

            bool rightClick = _oldMouseState.RightButton == ButtonState.Pressed && ms.RightButton == ButtonState.Released;
            if (rightClick)
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

            _oldMouseState = ms;
        }

        private void HandleKeyInput()
        {
            if (_cancel_menu_key?.IsPressed() > 0)
            {
                if (Cancellation != null)
                {
                    Reset();
                    Visible = false;
                    Cancellation.Invoke();
                    return;
                }
            }
            if (_next_item_key?.IsPressed() > 0)
                ChangeCurrentItem(1);
            if (_previous_item_key?.IsPressed() > 0)
                ChangeCurrentItem(-1);

            if (_active_item_key?.IsPressed() > 0 && _currentitemindex >= 0 && _currentitemindex < _items.Count)
                _items[_currentitemindex].Activation?.Invoke(_items[_currentitemindex]);
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
