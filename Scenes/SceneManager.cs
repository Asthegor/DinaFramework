using DinaFramework.Controls;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace DinaFramework.Scenes
{
    public class SceneManager : IValue
    {
        private static SceneManager _instance;
        private static readonly Object _mutex = new Object();
        private readonly Dictionary<string, Object> _values;
        private readonly Game _game;
        private readonly ContentManager _content;
        private readonly Dictionary<string, Scene> _scenes;
        private Scene _currentscene;
        public static SceneManager GetInstance(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);
            if (_instance == null)
                lock (_mutex) { _instance = new SceneManager(game); }
            return _instance;
        }
        public static SceneManager GetInstance() { return _instance; }
        // Gestion des scènes
        public void AddScene(string name, Type type)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The parameter 'name' must not be empty.");
            Debug.Assert(type != null, "The parameter 'type' must not be null.");
            Debug.Assert(typeof(Scene).IsAssignableFrom(type), "The type '" + type.Name + "' is not a valid Scene type.");
            Debug.Assert(!_scenes.ContainsKey(name), "The name '" + name + "' already exists.");

            _scenes[name] = (Scene)Activator.CreateInstance(type, this);
        }
        public void RemoveScene(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The 'name' must not be empty.");
            Debug.Assert(_scenes.ContainsKey(name) && _currentscene == _scenes[name], "The current scene cannot be removed.");

            _scenes.Remove(name);
        }
        public void SetCurrentScene(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), Messages.Messages.SCENE_NAME_MISSING);
            Debug.Assert(_scenes.ContainsKey(name), "The scene '" + name + "' does not exists.");

            ControllerKey.ResetAllKeys();

            _currentscene = _scenes[name];
            if (!_currentscene.Loaded)
            {
                _currentscene.Load(_content);
                _currentscene.Loaded = true;
            }
            _currentscene.Reset();
        }
        public void AddValue(string name, Object value)
        {
            if (!_values.TryAdd(name, value))
                _values[name] = value;
        }
        public T GetValue<T>(string name) { return _values.TryGetValue(name, out object value) ? (T)value : default; }
        public void RemoveValue(string name) { _values.Remove(name); }
        public void Exit() { _game.Exit(); }
        public bool IsMouseVisible { get => _game.IsMouseVisible; set => _game.IsMouseVisible = value; }

        // Fonctions génériques
        public void Update(GameTime gameTime) { _currentscene?.Update(gameTime); }
        public void Draw(SpriteBatch spritebatch)
        {
            if (spritebatch != null)
            {
                spritebatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                _currentscene?.Draw(spritebatch);
                spritebatch.End();
            }
        }

        // Fonctions privées
        private SceneManager(Game game)
        {
            _game = game;
            _content = game.Content;
            _scenes = new Dictionary<string, Scene>();
            _currentscene = null;
            _values = new Dictionary<string, Object>();
        }
    }
}

