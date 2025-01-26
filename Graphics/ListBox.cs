using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Linq;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente une boîte de liste graphique permettant d'afficher une collection d'éléments avec des fonctionnalités de sélection et de mise à jour.
    /// </summary>
    public class ListBox : Base, IPosition, IDimensions, IUpdate, IDraw, IElement
    {
        private const float OFFSET_LIST_Y = 2;
        private Vector2 OFFSET_PANEL = new Vector2(5, 5);

        private List<IElement> _elements = [];

        private Group _listGroup = new Group();
        private Panel _backgroundPanel;

        private List<Panel> _listPanels = [];
        private List<IElement> _listVisibleItems = [];

        private IElement _leftSelectedItem;
        private IElement _rightSelectedItem;

        private Vector2 _maxElementDimensions;

        private int _nbVisibleItems;
        private int _startIndex;

        private Color _defaultSelectionColor;

        /// <summary>
        /// Initialise une nouvelle instance de la classe ListBox avec les éléments spécifiés.
        /// </summary>
        /// <param name="items">Liste des éléments à afficher dans la boîte de liste.</param>
        /// <param name="position">Position initiale de la boîte de liste.</param>
        /// <param name="nbvisibleelements">Nombre d'éléments visibles dans la boîte de liste. Par défaut, tous les éléments sont visibles.</param>
        public ListBox(List<IElement> items, Vector2 position = default, int nbvisibleelements = -1)
        {
            ArgumentNullException.ThrowIfNull(items);
            foreach (var item in items)
            {
                AddElement(item);
            }
            //_elements.AddRange(items);

            _leftSelectedItem = null;
            _startIndex = 0;

            _defaultSelectionColor = Color.White * 0.65f;

            _nbVisibleItems = nbvisibleelements <= 0 ? items.Count : nbvisibleelements;

            HarmonizeElementsDimensions();

            for (int index = _startIndex; index < _nbVisibleItems && index < _elements.Count; index++)
            {
                var item = _elements[index];
                item.Position = OFFSET_PANEL + new Vector2(0, (_maxElementDimensions.Y + OFFSET_LIST_Y) * (index - _startIndex));
                _listPanels.Add(new Panel(item.Position, item.Dimensions, Color.Transparent));
                _listVisibleItems.Add(item);
            }

            Vector2 bkgpanelDim = new Vector2(_maxElementDimensions.X, (_maxElementDimensions.Y + OFFSET_LIST_Y) * _nbVisibleItems) + OFFSET_PANEL * 2;
            _backgroundPanel = new Panel(default, bkgpanelDim, Color.White * 0.5f);
            _listGroup.Add(_backgroundPanel);

            Dimensions = bkgpanelDim;
            Position = position;
        }

        private void HarmonizeElementsDimensions()
        {
            _maxElementDimensions = Vector2.Zero;
            foreach (IElement element in _elements)
            {
                if (element is IDimensions elemDim)
                {
                    if (elemDim.Dimensions.X > _maxElementDimensions.X)
                        _maxElementDimensions.X = elemDim.Dimensions.X;
                    if (elemDim.Dimensions.Y > _maxElementDimensions.Y)
                        _maxElementDimensions.Y = elemDim.Dimensions.Y;
                }
            }
            foreach (IElement element in _elements)
            {
                if (element is IDimensions elemDim)
                    elemDim.Dimensions = _maxElementDimensions;
            }
        }

        /// <summary>
        /// Obtient ou définit la position de la boîte de liste.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                Vector2 offset = value - base.Position;

                if (_backgroundPanel != null)
                    _backgroundPanel.Position += offset;

                foreach (Panel p in _listPanels)
                    p.Position += offset;

                foreach (IElement t in _elements)
                    t.Position += offset;

                base.Position = value;
            }
        }

        /// <summary>
        /// Obtient ou définit les dimensions de la boîte de liste.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                //Vector2 offset = base.Dimensions - value;

                //foreach (IElement t in _elements)
                //    t.Dimensions = new Vector2(t.Dimensions.X, t.Dimensions.Y + offset.Y);
                base.Dimensions = value;
            }
        }
        /// <summary>
        /// Obtient ou définit l'élément sélectionné dans la boîte de liste par son index.
        /// </summary>
        public int Value
        {
            get => _elements.IndexOf(_leftSelectedItem);
            set
            {
                _leftSelectedItem = (value < 0 || value >= _listVisibleItems.Count) ? null : _leftSelectedItem = _elements.FirstOrDefault(_listVisibleItems[value]);

                int selectedIndex = _listVisibleItems.IndexOf(_leftSelectedItem);
                for (int index = 0; index < _listPanels.Count; index++)
                    _listPanels[index].BackgroundColor = (index == selectedIndex) ? _defaultSelectionColor : Color.Transparent;
            }
        }
        /// <summary>
        /// Obtient l'index de l'élément sélectionné via un clic droit. Retourne -1 si aucun élément n'est sélectionné.
        /// </summary>
        public int ContextMenuItemIndex
        {
            get => _elements.IndexOf(_rightSelectedItem);
            set
            {
                if (value < 0 || value >= _listVisibleItems.Count)
                    _rightSelectedItem = null;
                else
                    _rightSelectedItem = _elements.FirstOrDefault(_listVisibleItems[value]);
            }
        }

        /// <summary>
        /// Met à jour l'état de la boîte de liste, y compris la gestion des clics sur les éléments.
        /// </summary>
        /// <param name="gametime">Temps de jeu actuel.</param>
        public void Update(GameTime gametime)
        {
            for (int index = 0; index < _listPanels.Count; index++)
            {
                Panel p = _listPanels[index];
                p.Update(gametime);

                if (p.IsLeftClicked())
                {
                    UpdateLeftSelectedItem(p, index);
                    _rightSelectedItem = null;
                    ContextMenuItemIndex = -1;
                }
                else if (p.IsRightClicked())
                {
                    UpdateLeftSelectedItem(p, index, true);
                    _rightSelectedItem = _listVisibleItems[index];
                    ContextMenuItemIndex = index;
                }
            }
        }
        private void UpdateLeftSelectedItem(Panel p, int index, bool removeselection = true)
        {
            if (_leftSelectedItem != null)
            {
                int previousIndex = _listVisibleItems.IndexOf(_leftSelectedItem);
                _listPanels[previousIndex].BackgroundColor = Color.Transparent;
            }

            if (removeselection && _leftSelectedItem == _listVisibleItems[index])
            {
                p.BackgroundColor = Color.Transparent;
                _leftSelectedItem = null;
            }
            else
            {
                p.BackgroundColor = _defaultSelectionColor;
                _leftSelectedItem = _listVisibleItems[index];
            }
        }
        /// <summary>
        /// Dessine la boîte de liste et ses éléments visibles sur un SpriteBatch.
        /// </summary>
        /// <param name="spritebatch">L'objet SpriteBatch utilisé pour dessiner les éléments.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            _backgroundPanel.Draw(spritebatch);
            for (int index = 0; index < _listVisibleItems.Count; index++)
            {
                Panel p = _listPanels[index];
                IElement item = _listVisibleItems[index];
                if (item is IDraw drawItem)
                {
                    p.Draw(spritebatch);
                    drawItem.Draw(spritebatch);
                }
            }
        }
        /// <summary>
        /// Ajoute un nouvel élément à la boîte de liste.
        /// </summary>
        /// <param name="element">Élément à ajouter à la boîte de liste.</param>
        public void AddElement(IElement element)
        {
            _elements.Add(element);
            if (_elements.Count < _nbVisibleItems)
            {
                _listVisibleItems.Add(element);
                //UpdateVisibleItemPosition(element);
            }
        }
        /// <summary>
        /// Supprime un élément spécifique de la boîte de liste.
        /// </summary>
        /// <param name="element">Élément à supprimer de la boîte de liste.</param>
        /// <remarks>
        /// Si l'élément supprimé est visible, un nouvel élément est ajouté à la liste des éléments visibles pour maintenir le nombre d'éléments affichés.
        /// </remarks>
        public void RemoveElement(IElement element)
        {
            bool addnew = _listVisibleItems.Remove(element);
            _elements.Remove(element);
            if (addnew)
            {
                IElement lastelement = _listVisibleItems.Last();
                int indexlast = _elements.LastIndexOf(lastelement);
                if (indexlast != -1 && indexlast < _elements.Count - 1)
                    _listVisibleItems.Add(_elements[indexlast + 1]);
            }
        }


        // Servira quand j'implémenterais les barres de défilement
        private void ChangeVisibleItemList(int startindex)
        {
            for (int index = startindex; index < _nbVisibleItems; index++)
            {
                var item = _elements[index];
                item.Position = OFFSET_PANEL + new Vector2(0, (_maxElementDimensions.Y + OFFSET_LIST_Y) * (index - startindex));
                _listVisibleItems.Add(item);
            }
        }
    }
}
