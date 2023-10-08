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
            if (_instance == null)
            {
                lock (_mutex)
                {
                    _instance ??= new SceneManager(game);
                }
            }
            return _instance;
        }
        // Gestion des scènes
        public void AddScene(string name, Type type)
        {
            Debug.Assert(name != "", "The 'name' must not be empty.");
            Debug.Assert(type != null, "The 'type' must not be null.");
            Debug.Assert(typeof(Scene).IsAssignableFrom(type), "The type '" + type.Name + "' is not a valid Scene type.");
            Debug.Assert(!_scenes.ContainsKey(name), "The 'name' already exists.");

            _scenes[name] = (Scene)Activator.CreateInstance(type, this);
        }
        public void RemoveScene(string name)
        {
            Debug.Assert(name != "", "The 'name' must not be empty.");
            Debug.Assert(_scenes.ContainsKey(name) && _currentscene == _scenes[name], "The current scene cannot be removed.");

            _scenes.Remove(name);
        }
        public void SetCurrentScene(string name)
        {
            ControllerKey.ResetAllKeys();
            Debug.Assert(name != "", "The 'name' must not be empty.");
            Debug.Assert(_scenes.ContainsKey(name), "The scene '" + name + "' does not exists.");

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
            if (_values.ContainsKey(name))
                _values[name] = value;
            else
                _values.Add(name, value);
        }
        public T GetValue<T>(string name) { return _values.ContainsKey(name) ? (T)_values[name] : default; }
        public void RemoveValue(string name) { if (_values.ContainsKey(name)) { _values.Remove(name); } }
        public void Exit() { _game.Exit(); }

        // Fonctions génériques
        public void Update(GameTime gameTime) { _currentscene?.Update(gameTime); }
        public void Draw(SpriteBatch spritebatch)
        {
            spritebatch.Begin(blendState: BlendState.NonPremultiplied);
            _currentscene?.Draw(spritebatch);
            spritebatch.End();
        }

        // Fonctions privées
        SceneManager(Game game)
        {
            _game = game;
            _content = game.Content;
            _scenes = new Dictionary<string, Scene>();
            _currentscene = null;
            _values = new Dictionary<string, Object>();
        }
    }
}
