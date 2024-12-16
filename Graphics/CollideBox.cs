﻿using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;

using System;

namespace DinaFramework.Graphics
{
    public class CollideBox : IPosition, IDimensions, ICollide, IElement, ICopyable<CollideBox>
    {
        Vector2 _position;
        Vector2 _dimensions;
        Rectangle _rect;
        public Vector2 Position
        {
            get { return _position; }
            set
            {
                _position = value;
                _rect.Location = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        public Vector2 Dimensions
        {
            get { return _dimensions; }
            set
            {
                _dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        public int ZOrder { get; set; }

        public Rectangle Rectangle { get { return _rect; } }
        public CollideBox(Vector2 position = default, Vector2 dimensions = default)
        {
            Position = position;
            Dimensions = dimensions;
        }
        public bool Collide(ICollide item)
        {
            ArgumentNullException.ThrowIfNull(item);

            return Rectangle.Intersects(item.Rectangle);
        }

        public CollideBox Copy()
        {
            return new CollideBox()
            {
                _dimensions = _dimensions,
                _position = _position,
                _rect = _rect,
                Dimensions = Dimensions,
                Position = Position,
                ZOrder = ZOrder
            };
        }
        private CollideBox() { }
    }
}