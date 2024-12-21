using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Runtime.CompilerServices;

namespace DinaFramework.Interfaces
{
    public interface IGameObject : ILoad, IReset, IUpdate, IDraw
    {}
    public interface IElement : IPosition, IDimensions, IZOrder
    {}
    public interface IZOrder
    {
        public abstract int ZOrder { get; set; }
    }
    public interface ILoad
    {
        public abstract void Load();
    }
    public interface IUpdate
    {
        public abstract void Update(GameTime gametime);
    }
    public interface IDraw
    {
        public abstract void Draw(SpriteBatch spritebatch);
    }
    public interface IReset
    {
        public abstract void Reset();
    }
    public interface IPosition
    {
        public abstract Vector2 Position { get; set; }
    }
    public interface IDimensions
    {
        public abstract Vector2 Dimensions { get; set; }
    }
    public interface IResource
    {
        public abstract void AddResource<T>(string resourceName, T resource);
        public abstract T GetResource<T>(string resourceName);
        public abstract void RemoveResource(string resourceName);
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
    public interface IClickable
    {
        public abstract bool IsClicked();
    }
    public interface IText : IPosition, IDimensions
    {
        public abstract Vector2 TextDimensions { get; }
    }
    public interface ILoadingScreen
    {
        public abstract float Progress { get; set; }
        public abstract string Text { get; set; }
    }
    public interface ICopyable<T>
    {
        public abstract T Copy();
    }
    public interface ILocked
    {
        public abstract bool Locked { get; set; }
    }
}
