using DinaFramework.Controls;
using DinaFramework.Interfaces;

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
    public class SceneManager : IResource
    {
        public static bool HasFocus { get; set; }
        private static SceneManager _instance;
        private static readonly object _mutex = new object();
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        private readonly Dictionary<string, object> _values = new Dictionary<string, object>();
        private readonly Game _game;
        private ContentManager _content;
        private readonly Dictionary<string, Scene> _scenes = new Dictionary<string, Scene>();
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
        public ContentManager Content { get => _content; private set => _content = value; }
        public IServiceProvider Services => _game.Services;

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
                if (string.IsNullOrEmpty(encryptString))
                    return default;
                string jsonString = DLACryptographie.EncryptDecrypt.DecryptText(encryptString);
                return JsonSerializer.Deserialize<T>(jsonString, _jsonOptions);
            }

            return default;
        }
        public static bool SaveObjectToFile<T>(T obj, string fileFullname, bool overwritten = true)
        {
            try
            {
                string jsonString = JsonSerializer.Serialize(obj, _jsonOptions);
                string encryptString = DLACryptographie.EncryptDecrypt.EncryptText(jsonString);

                if (overwritten)
                    File.WriteAllText(fileFullname, encryptString);
                else
                    File.AppendAllText(fileFullname, encryptString);
                return true;
            }
            catch (Exception ex) when (!(ex is OutOfMemoryException || ex is StackOverflowException))
            {
                return false;
            }
        }

        // Méthodes publiques
        public void AddResource<T>(string resourceName, T resource)
        {
            if (!_values.TryAdd(resourceName, resource))
                _values[resourceName] = resource;
        }
        public void AddScene(string name, Type type)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The parameter 'name' must not be empty.");
            Debug.Assert(type != null, "The parameter 'type' must not be null.");
            Debug.Assert(typeof(Scene).IsAssignableFrom(type), "The type '" + type.Name + "' is not a valid Scene type.");
            Debug.Assert(!_scenes.ContainsKey(name), "The name '" + name + "' already exists.");

            _scenes[name] = (Scene)Activator.CreateInstance(type, this);
        }
        public void Draw(SpriteBatch spritebatch)
        {
            if (spritebatch != null)
            {
                spritebatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                if (!_currentSceneLoaded)
                    _loadingScreen?.Draw(spritebatch);
                else if (_currentScene != null && _currentSceneLoaded == true && _currentScene.Loaded)
                    _currentScene.Draw(spritebatch);
                spritebatch.End();
            }
        }
        public void Exit() { _game.Exit(); }
        public T GetResource<T>(string resourceName)
        {
            if (_values.TryGetValue(resourceName, out object value))
                return (T)value;
            throw new KeyNotFoundException($"Resource '{resourceName}' not found in the resource manager.");
        }
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
        public void RemoveResource(string resourceName) { _values.Remove(resourceName); }
        public void ResetLoadingScreen(string message)
        {
            _loadingScreen.Reset();
            if (_loadingScreen is ILoadingScreen ls)
                ls.Text = message;
        }
        public async void SetCurrentScene(string name, bool withLoadingScreen = false)
        {
            if (string.IsNullOrEmpty(name))
            {
                Trace.WriteLine(Messages.Messages.SCENE_NAME_MISSING);
                return;
            }
            if (!_scenes.TryGetValue(name, out Scene value))
            {
                Trace.WriteLine("The scene '" + name + "' does not exists.");
                _currentScene = null;
                return;
            }

            ControllerKey.ResetAllKeys();

            _currentScene = _scenes[name];

            if (withLoadingScreen)
            {
                if (!_currentScene.Loaded)
                {
                    _currentSceneLoaded = false;
                    _loadingScreen?.Load(); // Charger l'écran de chargement, s'il existe
                    _loadingScreen?.Reset(); // Réinitialiser l'écran de chargement, s'il existe

                    // Charger la scène courante de manière asynchrone
                    await Task.Run(() =>
                    {
                        _currentScene.Load(); // Charger la scène courante
                        _currentScene.Loaded = true;
                    }).ConfigureAwait(false);
                }
                // Reset de la scène courante de manière asynchrone
                await Task.Run(() =>
                    {
                        _currentScene.Reset();
                        _currentSceneLoaded = true;
                        _currentScene.Visible = true;
                    }).ConfigureAwait(false);
            }
            else
            {
                if (!_currentScene.Loaded)
                {
                    _currentScene.Load(); // Charger la scène courante
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
                _loadingScreen?.Update(gameTime);
            else if (_currentScene != null &&  _currentScene.Loaded) 
                _currentScene.Update(gameTime);
        }


        // Fonctions privées
        private SceneManager(Game game)
        {
            _game = game;
            Content = game.Content;
            _currentScene = null;
            _loadingScreen = null;
            ScreenDimensions = new Vector2(game.GraphicsDevice.Viewport.Width, game.GraphicsDevice.Viewport.Height);
        }
    }
}

