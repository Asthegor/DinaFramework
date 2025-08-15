using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections;
using System.Collections.Generic;

namespace DinaFramework.Core
{
    /// <summary>
    /// Représente un groupe d'éléments, gérant leur affichage, visibilité, couleur et interactions.
    /// </summary>
    public class Group : Base, IDraw, IVisible, IEnumerator, IEnumerable, ICollide, IUpdate, IClickable, IColor
    {
        private List<IElement> _elements = [];
        private int index;
        private Rectangle _rect;
        private bool _visible;
        private Color _color;

        /// <summary>
        /// Initialise une nouvelle instance de la classe Group avec les propriétés spécifiées.
        /// </summary>
        /// <param name="position">Position initiale du groupe. Par défaut, (0,0).</param>
        /// <param name="dimensions">Dimensions initiales du groupe. Par défaut, (0,0).</param>
        /// <param name="zorder">Ordre d'affichage initial du groupe. Par défaut, 0.</param>
        public Group(Vector2 position = default, Vector2 dimensions = default, int zorder = 0) : base(position, dimensions, zorder)
        {
            _color = Color.White;
            Visible = true;
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Group en copiant les éléments d'un autre groupe.
        /// </summary>
        /// <param name="group">Groupe source à copier.</param>
        /// <param name="duplicate">
        /// Indique si les éléments doivent être dupliqués. 
        /// Si false, les éléments sont simplement référencés.
        /// </param>
        /// <exception cref="ArgumentNullException">Lance une exception si le groupe fourni est null.</exception>
        public Group(Group group, bool duplicate = true)
        {
            ArgumentNullException.ThrowIfNull(group);
            foreach (var item in group._elements)
            {
                if (duplicate)
                    _elements.Add((IElement)Activator.CreateInstance(item.GetType(), item));
                else
                    _elements.Add(item);
            }
            Position = group.Position;
            Dimensions = group.Dimensions;
            ZOrder = group.ZOrder;
            Visible = group.Visible;
            index = 0;
            _color = Color.White;
        }

        /// <summary>
        /// Obtient l'élément actuel lors de l'énumération du groupe.
        /// </summary>
        public object Current => _elements[index];
        /// <summary>
        /// Obtient le rectangle représentant la position et les dimensions du groupe.
        /// </summary>
        public Rectangle Rectangle => _rect;

        /// <summary>
        /// Ajoute un élément au groupe.
        /// </summary>
        /// <param name="element">Élément à ajouter.</param>
        public void Add(IElement element)
        {
            _elements.Add(element);
            if (element is IDimensions)
                UpdateDimensions();
            SortElements();
        }
        /// <summary>
        /// Obtient ou définit la position du groupe. La modification de la position déplace également ses éléments.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                Vector2 offset = value - base.Position;
                foreach (var element in _elements)
                {
                    if (element is IPosition item)
                        item.Position = new Vector2(item.Position.X + offset.X, item.Position.Y + offset.Y);
                }
                base.Position = value;
                _rect.Location = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        /// <summary>
        /// Obtient ou définit les dimensions du groupe.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        /// <summary>
        /// Obtient ou définit la visibilité du groupe et de ses éléments.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                foreach (var element in _elements)
                {
                    if (element is IVisible elemvisible)
                        elemvisible.Visible = value;
                }
                _visible = value;
            }
        }

        /// <summary>
        /// Obtient ou définit la couleur du groupe et de ses éléments.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                foreach(var element in _elements)
                {
                    if (element is IColor elemcolor)
                        elemcolor.Color = value;
                }
                _color = value;
            }
        }

        /// <summary>
        /// Vérifie si un élément du groupe est cliqué.
        /// </summary>
        /// <returns>True si un élément est cliqué, sinon false.</returns>
        public bool IsClicked()
        {
            foreach (var item in _elements)
            {
                if (item is IClickable itemclickable)
                    if (itemclickable.IsClicked() == true)
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Obtient le nombre d'éléments dans le groupe.
        /// </summary>
        /// <returns>Nombre d'éléments.</returns>
        public int Count() => _elements.Count;
        /// <summary>
        /// Passe à l'élément suivant lors de l'énumération.
        /// </summary>
        /// <returns>True si un élément suivant existe, sinon false.</returns>
        public bool MoveNext()
        {
            return (++index < _elements.Count);
        }
        /// <summary>
        /// Réinitialise l'énumération du groupe.
        /// </summary>
        public void Reset() => index = -1;
        /// <summary>
        /// Retourne un énumérateur pour parcourir les éléments du groupe.
        /// </summary>
        public IEnumerator GetEnumerator()
        {
            Reset();
            return this;
        }

        /// <summary>
        /// Vérifie si le groupe entre en collision avec un autre élément.
        /// </summary>
        /// <param name="item">Élément à tester pour la collision.</param>
        /// <returns>True si une collision est détectée, sinon false.</returns>
        public bool Collide(ICollide item)
        {
            if (item == null)
                return false;
            return Rectangle.Intersects(item.Rectangle);
        }

        /// <summary>
        /// Dessine les éléments du groupe.
        /// </summary>
        /// <param name="spritebatch">Objet SpriteBatch utilisé pour le rendu.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            foreach (var element in _elements)
            {
                if (element is IDraw draw)
                    draw.Draw(spritebatch);
            }
        }
        /// <summary>
        /// Trie les éléments du groupe par ordre d'affichage (Z-order).
        /// </summary>
        public void SortElements()
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
        /// <summary>
        /// Met à jour les dimensions du groupe en fonction de ses éléments.
        /// </summary>
        private void UpdateDimensions()
        {
            float x, y;
            float w, h;
            x = Position.X;
            y = Position.Y;
            w = -1;
            h = -1;
            foreach (var element in _elements)
            {
                if (element is IDimensions elemdim && element is IPosition elempos)
                {
                    Vector2 elemPos = elempos.Position;
                    Vector2 elemDim = elemdim.Dimensions;

                    if (elemPos.X < x)
                        x = elemPos.X;
                    if (elemPos.Y < y)
                        y = elemPos.Y;
                    Vector2 flip = Vector2.One;
                    if (element is IFlip eflip)
                    {
                        // TODO: à corriger dès que la classe Image sera implémentée
                        //flip = eflip.GetFlip();
                    }
                    float cfvx = flip.X > 0 ? 1 : 0;
                    float cfvy = flip.Y > 0 ? 1 : 0;
                    if (w < elemPos.X + elemDim.X * cfvx)
                        w = elemPos.X + elemDim.X * cfvx;
                    if (h < elemPos.Y + elemDim.Y * cfvy)
                        h = elemPos.Y + elemDim.Y * cfvy;
                    //if (w < Math.Abs(elemPos.X) + Math.Abs(elemDim.X * cfvx))
                    //    w = Math.Abs(elemPos.X) + Math.Abs(elemDim.X * cfvx);
                    //if (h < Math.Abs(elemPos.Y) + Math.Abs(elemDim.Y * cfvy))
                    //    h = Math.Abs(elemPos.Y) + Math.Abs(elemDim.Y * cfvy);
                }
            } //foreach
            if (x < float.MaxValue && y < float.MaxValue && w > -1 && h > -1)
            {
                Dimensions = new Vector2(w - (x < 0 ? 0 : x), h - (y < 0 ? 0 : y));
            }

        }
        /// <summary>
        /// Met à jour les éléments du groupe.
        /// </summary>
        /// <param name="gametime">Temps de jeu actuel.</param>
        public void Update(GameTime gametime)
        {
            foreach (var elem in _elements)
            {
                if (elem is IUpdate uelem)
                    uelem.Update(gametime);
            }
        }
    }
}
