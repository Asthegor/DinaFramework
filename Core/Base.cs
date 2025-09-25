using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;

using System;

namespace DinaFramework.Core
{
    /// <summary>
    /// Classe abstraite de base représentant un élément avec une position, des dimensions et un ordre d'affichage.
    /// </summary>
    public abstract class Base : IElement
    {
        private int _zorder;
        private Vector2 _position;
        private Vector2 _dimensions;

        /// <summary>
        /// Ordre d'affichage (Z-order) de l'élément.
        /// </summary>
        public int ZOrder
        {
            get { return _zorder; }
            set { _zorder = value; }
        }
        /// <summary>
        /// Position de l'élément dans l'espace.
        /// </summary>
        public virtual Vector2 Position
        {
            get { return _position; }
            set { _position = value; }
        }
        /// <summary>
        /// Dimensions de l'élément.
        /// </summary>
        public virtual Vector2 Dimensions
        {
            get { return _dimensions; }
            set { _dimensions = value; }
        }

        internal Base(Vector2 position = new Vector2(), Vector2 dimensions = new Vector2(), int zorder = 0)
        {
            Position = position;
            Dimensions = dimensions;
            ZOrder = zorder;
        }
        internal Base(Base @base)
        {
            ArgumentNullException.ThrowIfNull(@base);

            Position = @base.Position;
            Dimensions = @base.Dimensions;
            ZOrder = @base.ZOrder;
        }
    }
}
