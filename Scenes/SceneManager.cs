using DinaFramework.Controls;
using DinaFramework.Interfaces;

using DLACrypto;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace DinaFramework.Scenes
{
    public class SceneManager : IValue
    {

        private static SceneManager _instance;
        private static readonly object _mutex = new object();
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        private readonly Dictionary<string, object> _values;
        private readonly Game _game;
        private readonly ContentManager _content;
        private readonly Dictionary<string, Scene> _scenes;
        private ResourceManager _resourceManager;
        private Scene _currentScene;
        private Scene _loadingScreen;
        private bool _currentSceneLoaded;
        private float _loadingProgress;
        // Propriétés
        public bool IsMouseVisible { get => _game.IsMouseVisible; set => _game.IsMouseVisible = value; }
        public float LoadingProgress { get => _loadingProgress; set => _loadingProgress = value; }
        public Vector2 ScreenDimensions { get; private set; }
        public PlayerController Controller { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        public SpriteBatch SpriteBatch { get; set; }

        // Méthodes statiques
        public static SceneManager GetInstance(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);
            lock (_mutex)
            {
                _instance ??= new SceneManager(game);
            }
            return _instance;
        }
        public static SceneManager GetInstance() { return _instance; }
        public static T LoadObjectFromEncryptFile<T>(string filePath)
        {
            if (File.Exists(filePath))
            {
                string encryptString = File.ReadAllText(filePath);
                string jsonString = Decryptage.Decrypt(encryptString);
                File.WriteAllText(filePath + ".txt", jsonString);
                return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
            }

            return default;
        }
        public static void SaveObjectToFile<T>(T obj, string filePath)
        {
            string jsonString = JsonSerializer.Serialize(obj, _jsonOptions);
            //string encryptString = Encryptage.Encrypt(jsonString);
            string encryptString = jsonString;
            File.WriteAllText(filePath, encryptString);
        }

        // Méthodes publiques
        public void AddResource<T>(string resourceName, T resource)
        {
            _resourceManager.AddResource(resourceName, resource);
        }
        public void AddScene(string name, Type type)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The parameter 'name' must not be empty.");
            Debug.Assert(type != null, "The parameter 'type' must not be null.");
            Debug.Assert(typeof(Scene).IsAssignableFrom(type), "The type '" + type.Name + "' is not a valid Scene type.");
            Debug.Assert(!_scenes.ContainsKey(name), "The name '" + name + "' already exists.");

            _scenes[name] = (Scene)Activator.CreateInstance(type, this);
        }
        public void AddValue(string name, Object value)
        {
            if (!_values.TryAdd(name, value))
                _values[name] = value;
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (spritebatch != null)
            {
                spritebatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                if (!_currentSceneLoaded)
                    _loadingScreen?.Draw(spritebatch);
                else
                    _currentScene?.Draw(spritebatch);
                spritebatch.End();
            }
        }
        public void Exit() { _game.Exit(); }
        public T GetResource<T>(string resourceName)
        {
            return _resourceManager.GetResource<T>(resourceName);
        }
        public T GetValue<T>(string name) { return _values.TryGetValue(name, out object value) ? (T)value : default; }
        public void LoadingScreen<T>() where T : Scene, ILoadingScreen
        {
            Type type = typeof(T);
            Debug.Assert(typeof(T).IsSubclassOf(typeof(Scene)), "The type '" + type.Name + "' is not a valid ILoadingScreen type.");
            _loadingScreen = (Scene)Activator.CreateInstance(type, this);
        }
        public void RemoveScene(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The 'name' must not be empty.");
            Debug.Assert(_scenes.ContainsKey(name) && _currentScene == _scenes[name], "The current scene cannot be removed.");

            _scenes.Remove(name);
        }
        public void RemoveValue(string name) { _values.Remove(name); }
        public void ResetLoadingScreen(string message)
        {
            _loadingScreen.Reset();
            if (_loadingScreen is ILoadingScreen ls)
                ls.Text = message;
        }
        public async void SetCurrentScene(string name, bool withloadingscreen = false)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), Messages.Messages.SCENE_NAME_MISSING);
            Debug.Assert(_scenes.ContainsKey(name), "The scene '" + name + "' does not exists.");

            ControllerKey.ResetAllKeys();

            _currentScene = _scenes[name];

            if (withloadingscreen)
            {
                if (!_currentScene.Loaded)
                {
                    _currentSceneLoaded = false;
                    _loadingScreen?.Load(_content); // Charger l'écran de chargement, s'il existe
                    _loadingScreen?.Reset(); // Réinitialiser l'écran de chargement, s'il existe

                    // Charger la scène courante de manière asynchrone
                    await Task.Run(() =>
                    {
                        _currentScene.Load(_content); // Charger la scène courante
                        _currentScene.Loaded = true;
                    }).ConfigureAwait(false);
                }
                // Reset de la scène courante de manière asynchrone
                await Task.Run(() =>
                {
                    _currentScene.Reset();
                    _currentSceneLoaded = true;
                }).ConfigureAwait(false);
            }
            else
            {
                if (!_currentScene.Loaded)
                {
                    _currentScene.Load(_content); // Charger la scène courante
                    _currentScene.Loaded = true;
                }
                _currentScene.Reset();
                _currentSceneLoaded = true;
            }
        }
        public void SetGraphicsDeviceManager(GraphicsDeviceManager graphicsDeviceManager)
        {
            GraphicsDeviceManager = graphicsDeviceManager;
        }
        public void Update(GameTime gameTime)
        {
            if (!_currentSceneLoaded)
            {
                //if (_loadingScreen != null)
                //    ((ILoadingScreen)_loadingScreen).Progress = _loadingProgress;
                _loadingScreen?.Update(gameTime);
            }
            else
                _currentScene?.Update(gameTime);
        }


        // Fonctions privées
        private SceneManager(Game game)
        {
            _game = game;
            _content = game.Content;
            _scenes = new Dictionary<string, Scene>();
            _currentScene = null;
            _loadingScreen = null;
            _values = new Dictionary<string, object>();
            ScreenDimensions = new Vector2(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
            _resourceManager = new ResourceManager();
        }
    }
}

