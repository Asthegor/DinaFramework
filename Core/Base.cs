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
        int _zorder;
        Vector2 _position;
        Vector2 _dimensions;

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
        /// <summary>
        /// Initialise une nouvelle instance de la classe Base avec les valeurs spécifiées.
        /// </summary>
        /// <param name="position">Position initiale de l'élément. Par défaut, (0,0).</param>
        /// <param name="dimensions">Dimensions initiales de l'élément. Par défaut, (0,0).</param>
        /// <param name="zorder">Ordre d'affichage initial de l'élément. Par défaut, 0.</param>
        protected Base(Vector2 position = new Vector2(), Vector2 dimensions = new Vector2(), int zorder = 0)
        {
            this.Position = position;
            this.Dimensions = dimensions;
            this.ZOrder = zorder;
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Base en copiant les propriétés d'un autre élément.
        /// </summary>
        /// <param name="base">Élément dont les propriétés doivent être copiées.</param>
        /// <exception cref="ArgumentNullException">Lance une exception si l'élément fourni est null.</exception>
        protected Base(Base @base)
        {
            ArgumentNullException.ThrowIfNull(@base);

            this.Position = @base.Position;
            this.Dimensions = @base.Dimensions;
            this.ZOrder = @base.ZOrder;
        }
    }
}
