using DinaFramework.Controls;
using DinaFramework.Interfaces;
using DinaFramework.Menus;

using DLACrypto;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace DinaFramework.Scenes
{
    public class SceneManager : IValue
    {
        private static SceneManager _instance;
        private static readonly Object _mutex = new Object();
        private static JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };
        private readonly Dictionary<string, Object> _values;
        private readonly Game _game;
        private readonly ContentManager _content;
        private readonly Dictionary<string, Scene> _scenes;
        private Scene _currentScene;
        private LoadingScene _loadingScreen;
        private bool _currentSceneLoaded;
        private float _loadingProgress;
        // Propriétés
        public bool IsMouseVisible { get => _game.IsMouseVisible; set => _game.IsMouseVisible = value; }
        public float LoadingProgress { get => _loadingProgress; set => _loadingProgress = value; }
        public Vector2 ScreenDimensions { get; private set; }
        public PlayerController Controller { get; private set; }
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }

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
        public T GetValue<T>(string name) { return _values.TryGetValue(name, out object value) ? (T)value : default; }
        public void RemoveScene(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The 'name' must not be empty.");
            Debug.Assert(_scenes.ContainsKey(name) && _currentScene == _scenes[name], "The current scene cannot be removed.");

            _scenes.Remove(name);
        }
        public void LoadingScreen(Type type)
        {
            Debug.Assert(type != null, "The parameter 'type' must not be null.");
            Debug.Assert(typeof(LoadingScene).IsAssignableFrom(type), "The type '" + type.Name + "' is not a valid Scene type.");

            _loadingScreen = (LoadingScene)Activator.CreateInstance(type, this);
        }
        public async void SetCurrentScene(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), Messages.Messages.SCENE_NAME_MISSING);
            Debug.Assert(_scenes.ContainsKey(name), "The scene '" + name + "' does not exists.");

            // Obtenir le dernier élément de la pile d'appels
            MethodBase method = GetNextMethodFromSetCurrentScene();
            bool isValid = false;
            if (typeof(Game).IsAssignableFrom(method.DeclaringType) && method.Name == "LoadContent")
                isValid = true;
            if (typeof(Scene).IsAssignableFrom(method.DeclaringType))
            {
                if (method.Name == "Update")
                    isValid = true;
                else
                {
                    if (typeof(MenuItem).IsAssignableFrom(((MethodInfo)method).ReturnType))
                        isValid = true;
                    var parameters = ((MethodInfo)method).GetParameters();
                    if (parameters.Length == 1 && parameters[0].ParameterType == typeof(MenuItem))
                        isValid = true;
                }
            }

            if (isValid)
            {
                ControllerKey.ResetAllKeys();

                _currentScene = _scenes[name];
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
                // Charger la scène courante de manière asynchrone
                await Task.Run(() =>
                {
                    _currentScene.Reset();
                    _currentSceneLoaded = true;
                }).ConfigureAwait(false);
            }
            else
            {
                throw new InvalidOperationException(Messages.Messages.SETCURRENTSCENE_ERROR);
            }
        }
        public void RemoveValue(string name) { _values.Remove(name); }
        public void Update(GameTime gameTime)
        {
            if (!_currentSceneLoaded)
            {
                if (_loadingScreen != null)
                    _loadingScreen.Progress = _loadingProgress;
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
        }
        private static MethodBase GetNextMethodFromSetCurrentScene()
        {
            int index = 0;
            StackTrace stackTrace = new StackTrace();
            var frames = stackTrace.GetFrames();
            foreach (StackFrame frame in frames)
            {
                MethodBase method = frame.GetMethod();
                if (method.Name == "SetCurrentScene")
                    break;
                index++;
            }
            if (index >= frames.Length)
                index = -1;
            return stackTrace.GetFrame(index + 1).GetMethod();
        }
        public void SetGraphicsDeviceManager(GraphicsDeviceManager graphicsDeviceManager)
        {
            GraphicsDeviceManager = graphicsDeviceManager;
        }
    }
}

