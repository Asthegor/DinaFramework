using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections;
using System.Collections.Generic;

namespace DinaFramework.Core
{
    public class Group : Base, IDraw, IVisible, IEnumerator, IEnumerable, ICollide, IUpdate, IClickable
    {
        readonly List<IElement> _elements = new List<IElement>();
        private int index;
        private Rectangle _rect;
        private bool _visible;

        public Group(Vector2 position = default, Vector2 dimensions = default, int zorder = 0) : base(position, dimensions, zorder)
        {
        }
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
        }

        public object Current => _elements[index];
        public Rectangle Rectangle => _rect;

        public void Add(IElement element)
        {
            _elements.Add(element);
            if (element is IDimensions)
                UpdateDimensions();
            SortElements();
        }
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
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
        }
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

        public int Count() => _elements.Count;
        public bool MoveNext()
        {
            return (++index < _elements.Count);
        }
        public void Reset() => index = -1;
        public IEnumerator GetEnumerator()
        {
            Reset();
            return this;
        }
        public bool Collide(ICollide item)
        {
            if (item == null)
                return false;
            return Rectangle.Intersects(item.Rectangle);
        }

        public void Draw(SpriteBatch spritebatch)
        {
            foreach (var element in _elements)
            {
                if (element is IDraw draw)
                    draw.Draw(spritebatch);
            }
        }
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
                    if (w < elemPos.X + elemDim.X * cfvx)
                        w = elemPos.X + elemDim.X * cfvx;
                    float cfvy = flip.Y > 0 ? 1 : 0;
                    if (h < elemPos.Y + elemDim.Y * cfvy)
                        h = elemPos.Y + elemDim.Y * cfvy;
                }
            } //foreach
            if (x < float.MaxValue && y < float.MaxValue && w > -1 && h > -1)
                Dimensions = new Vector2(w - x, h - y);

        }
        public void Update(GameTime gameTime)
        {
            foreach (var elem in _elements)
            {
                if (elem is IUpdate uelem)
                    uelem.Update(gameTime);
            }
        }
    }
}
