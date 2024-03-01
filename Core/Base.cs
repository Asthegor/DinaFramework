using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;

using System;

namespace DinaFramework.Core
{
    public abstract class Base : IElement
    {
        int _zorder;
        Vector2 _position;
        Vector2 _dimensions;

        public int ZOrder
        {
            get { return _zorder; }
            set { _zorder = value; }
        }
        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        public virtual Vector2 Dimensions
        {
            get { return _dimensions; }
            set { _dimensions = value; }
        }
        protected Base(Vector2 position = new Vector2(), Vector2 dimensions = new Vector2(), int zorder = 0)
        {
            Position = position;
            Dimensions = dimensions;
            ZOrder = zorder;
        }
        protected Base(Base @base)
        {
            ArgumentNullException.ThrowIfNull(@base);

            Position = @base.Position;
            Dimensions = @base.Dimensions;
            ZOrder = @base.ZOrder;
        }
    }
}
