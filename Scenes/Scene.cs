using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace DinaFramework.Scenes
{
    public abstract class Scene : IGameObject, IValue
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

        public void AddValue(string name, object value) { _sceneManager.AddValue(name, value); }
        public T GetValue<T>(string name) { return _sceneManager.GetValue<T>(name); }
        public void RemoveValue(string name) { _sceneManager.RemoveValue(name); }
        public void Exit() { _sceneManager.Exit(); }
    }
}
