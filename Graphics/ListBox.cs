using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

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

        private IElement _selectedItem;

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
            _elements.AddRange(items);

            _selectedItem = null;
            _startIndex = 0;

            _defaultSelectionColor = Color.White * 0.65f;

            _nbVisibleItems = nbvisibleelements <= 0 ? items.Count : nbvisibleelements;

            HarmonizeElementsDimensions();

            for (int index = _startIndex; index < _nbVisibleItems; index++)
            {
                var item = _elements[index];
                item.Position = OFFSET_PANEL + new Vector2(0, (_maxElementDimensions.Y + OFFSET_LIST_Y) * (index - _startIndex));
                _listPanels.Add(new Panel(item.Position, item.Dimensions, Color.Transparent));
                _listVisibleItems.Add(item);
            }

            Vector2 bkgpanelDim = new Vector2(_maxElementDimensions.X, (_maxElementDimensions.Y + OFFSET_LIST_Y) * _nbVisibleItems) + OFFSET_PANEL * 2;
            _backgroundPanel = new Panel(default, bkgpanelDim, Color.White * 0.5f);
            _listGroup.Add(_backgroundPanel);

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
                base.Position = value;

                if (_backgroundPanel != null)
                    _backgroundPanel.Position += offset;

                foreach (Panel p in _listPanels)
                    p.Position += offset;

                foreach (IElement t in _elements)
                    t.Position += offset;
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
                Vector2 offset = base.Dimensions - value;
                base.Dimensions = value;

                foreach (IElement t in _elements)
                    t.Dimensions = new Vector2(t.Dimensions.X, t.Dimensions.Y + offset.Y);
            }
        }
        /// <summary>
        /// Obtient ou définit l'élément sélectionné dans la boîte de liste par son index.
        /// </summary>
        public int Value
        {
            get => _elements.IndexOf(_selectedItem);
            set
            {
                _selectedItem = (value < 0 || value >= _elements.Count) ? null : _selectedItem = _elements[value];

                int selectedIndex = _listVisibleItems.IndexOf(_selectedItem);
                for (int index = 0; index < _listPanels.Count; index++)
                    _listPanels[index].BackgroundColor = (index == selectedIndex) ? _defaultSelectionColor : Color.Transparent;
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

                if (p.IsClicked())
                {
                    if (_selectedItem != null)
                    {
                        int previousIndex = _listVisibleItems.IndexOf(_selectedItem);
                        _listPanels[previousIndex].BackgroundColor = Color.Transparent;
                    }

                    p.BackgroundColor = _defaultSelectionColor;
                    _selectedItem = _listVisibleItems[index];
                }
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
