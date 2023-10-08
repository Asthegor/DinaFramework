using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections;
using System.Collections.Generic;

namespace DinaFramework.Core
{
    public class Group : Base, IDraw, IVisible, IEnumerator, IEnumerable, ICollide
    {
        readonly List<IElement> _elements;
        private int index;
        private Rectangle _rect;
        private bool _visible;

        public Group(Vector2 position = default, Vector2 dimensions = default) : base(position, dimensions)
        {
            _elements = new List<IElement>();
        }
        public Group(Group group, bool duplicate = true)
        {
            _elements = new List<IElement>();
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
            if (element is IDimensions item)
            {
                Vector2 newDim = Dimensions;
                Vector2 dim = item.Dimensions;
                if (newDim.X < dim.X)
                    newDim.X = dim.X;
                if (newDim.Y < dim.Y)
                    newDim.Y = dim.Y;
                Dimensions = newDim;
            }
            SortElements();
        }
        public new Vector2 Position
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
        public new Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                float width = 0.0f;
                foreach (var element in _elements)
                {
                    if (element is IDimensions item)
                        width = item.Dimensions.X;
                }
                if (width < base.Dimensions.X)
                    width = base.Dimensions.X;
                foreach (var element in _elements)
                {
                    if (element is IDimensions item)
                        item.Dimensions = new Vector2(width, item.Dimensions.Y);
                }
                if (value.X < width)
                    value.X = width;
                base.Dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        public bool Visible
        {
            get => _visible;
            set => _visible = value;
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
        public bool Collide(ICollide item) => Rectangle.Intersects(item.Rectangle);
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

    }
}
