using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Interfaces
{
    public interface IElement
    {
        public abstract int ZOrder { get; set; }
    }
    public interface ILoad
    {
        public abstract void Load(ContentManager content);
    }
    public interface IUpdate
    {
        public abstract void Update(GameTime gameTime);
    }
    public interface IDraw
    {
        public abstract void Draw(SpriteBatch spritebatch);
    }
    public interface IPosition
    {
        public abstract Vector2 Position { get; set; }
    }
    public interface IDimensions
    {
        public abstract Vector2 Dimensions { get; set; }
    }
    public interface IValue
    {
        public abstract void AddValue(string name, Object value);
        public abstract T GetValue<T>(string name);
        public abstract void RemoveValue(string name);
    }
    public interface IColor
    {
        public abstract Color Color { get; set; }
    }
    public interface IVisible
    {
        public abstract bool Visible { get; set; }
    }
    public interface ICollide : IPosition, IDimensions
    {
        public abstract bool Collide(ICollide item);
        public Rectangle Rectangle { get; }
    }
    public interface IFlip
    {
        public abstract Vector2 Flip { get; set; }
    }
#pragma warning disable CA1040 // Éviter les interfaces vides
    public interface IText { }
#pragma warning restore CA1040 // Éviter les interfaces vides
}
