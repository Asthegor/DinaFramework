using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DinaFramework.Scenes
{
    public abstract class Scene : IGameObject, IResource
    {
        public bool Loaded { get; set; }
        private readonly SceneManager _sceneManager;
        protected SceneManager SceneManager => _sceneManager;
        protected Scene(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }

        public abstract void Load(ContentManager content);
        public abstract void Reset();
        public abstract void Update(GameTime gameTime);
        public abstract void Draw(SpriteBatch spritebatch);

        public void AddResource<T>(string resourceName, T resource) { _sceneManager.AddResource(resourceName, resource); }
        public T GetResource<T>(string resourceName) { return _sceneManager.GetResource<T>(resourceName); }
        public void RemoveResource(string resourceName) { _sceneManager.RemoveResource(resourceName); }
        public void Exit() { _sceneManager.Exit(); }
    }
}
