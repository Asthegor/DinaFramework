using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Linq;

namespace DinaFramework.Scenes
{
    public abstract class Scene : IGameObject, IResource
    {
        private SceneManager _sceneManager;
        protected SceneManager SceneManager => _sceneManager;
        protected Scene(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }
        public bool Loaded { get; set; }
        protected Vector2 ScreenDimensions => _sceneManager.ScreenDimensions;
        protected SpriteBatch SpriteBatch => _sceneManager.SpriteBatch;
        protected float LoadingProgress
        {
            get => _sceneManager.LoadingProgress;
            set => _sceneManager.LoadingProgress = value;
        }
        protected ContentManager Content => _sceneManager.Content;
        bool _visible;
        public bool Visible { get => _visible; set => _visible = value; }

        public abstract void Load();
        public abstract void Reset();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spritebatch);

        public void AddResource<T>(string resourceName, T resource) { _sceneManager.AddResource(resourceName, resource); }
        public T GetResource<T>(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName), "The parameter 'resourceName' must not be empty.");

            return _sceneManager.GetResource<T>(resourceName);
        }
        public void RemoveResource(string resourceName) { _sceneManager.RemoveResource(resourceName); }
        protected void Exit() { _sceneManager.Exit(); }
        protected void SetCurrentScene(string name, bool withLoadingScreen = false)
        {
            _sceneManager.SetCurrentScene(name, withLoadingScreen);
        }
        protected RenderTarget2D CreateRenderTarget2D(Vector2 dimensions)
        {
            return new RenderTarget2D(_sceneManager.GraphicsDeviceManager.GraphicsDevice, (int)dimensions.X, (int)dimensions.Y);
        }
        protected void SetRenderTarget2D(RenderTarget2D renderTarget)
        {
            _sceneManager.SpriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
        }
        protected void ClearScreen(Color color)
        {
            _sceneManager.SpriteBatch.GraphicsDevice.Clear(color);
        }
        protected void BeginSpritebatch(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            _sceneManager.SpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }
        protected void ResetLoadingScreen(string message)
        {
            _sceneManager.ResetLoadingScreen(message);
        }
    }
}
