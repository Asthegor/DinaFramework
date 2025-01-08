using DinaFramework.Controls;
using DinaFramework.Exceptions;
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
    /// <summary>
    /// Gère les scènes du jeu, le passage d'une scène à l'autre, le chargement des ressources et l'affichage.
    /// Cette classe est un gestionnaire central pour les scènes, les ressources et la gestion des écrans de chargement.
    /// Elle garantit qu'une seule instance de la classe existe pendant l'exécution du jeu (Singleton).
    /// </summary>
    public class SceneManager : IResource
    {
        /// <summary>
        /// Indique si la fenêtre du jeu a actuellement le focus.
        /// </summary>
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
        /// <summary>
        /// Obtient ou définit la visibilité de la souris dans la fenêtre du jeu.
        /// </summary>
        public bool IsMouseVisible { get => _game.IsMouseVisible; set => _game.IsMouseVisible = value; }
        /// <summary>
        /// Obtient ou définit le progrès du chargement, compris entre 0 et 1.
        /// </summary>
        public float LoadingProgress { get => _loadingProgress; set => _loadingProgress = value; }
        /// <summary>
        /// Obtient les dimensions de l'écran du jeu sous forme de Vector2 (largeur et hauteur).
        /// </summary>
        public Vector2 ScreenDimensions { get; private set; }
        /// <summary>
        /// Obtient le contrôleur du joueur pour gérer les entrées du joueur.
        /// </summary>
        public PlayerController Controller { get; private set; }
        /// <summary>
        /// Obtient le gestionnaire de périphériques graphiques utilisé pour gérer le périphérique graphique et les paramètres de rendu.
        /// </summary>
        public GraphicsDeviceManager GraphicsDeviceManager { get; private set; }
        /// <summary>
        /// Obtient ou définit le SpriteBatch utilisé pour dessiner les sprites 2D.
        /// </summary>
        public SpriteBatch SpriteBatch { get; set; }
        /// <summary>
        /// Obtient le ContentManager utilisé pour charger et gérer les ressources du jeu.
        /// </summary>
        public ContentManager Content { get => _content; private set => _content = value; }
        /// <summary>
        /// Obtient le fournisseur de services qui permet d'accéder aux services du jeu.
        /// </summary>
        public IServiceProvider Services => _game.Services;

        // Méthodes statiques
        /// <summary>
        /// Obtient l'instance unique du SceneManager. Si l'instance n'existe pas, elle est créée.
        /// </summary>
        /// <param name="game">L'instance du jeu utilisée pour initialiser le gestionnaire de scènes.</param>
        /// <returns>L'instance unique du SceneManager.</returns>
        public static SceneManager GetInstance(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);
            lock (_mutex)
            {
                _instance ??= new SceneManager(game);
            }
            return _instance;
        }
        /// <summary>
        /// Obtient l'instance unique du SceneManager.
        /// </summary>
        /// <returns>L'instance unique du SceneManager.</returns>
        public static SceneManager GetInstance() { return _instance; }
        /// <summary>
        /// Charge un objet depuis un fichier crypté et le désérialise dans le type spécifié.
        /// </summary>
        /// <typeparam name="T">Le type de l'objet à charger.</typeparam>
        /// <param name="filePath">Le chemin du fichier crypté.</param>
        /// <returns>L'objet désérialisé, ou la valeur par défaut si le fichier n'existe pas ou est vide.</returns>
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
        /// <summary>
        /// Sauvegarde un objet dans un fichier en format crypté, avec possibilité de remplacer le fichier existant.
        /// </summary>
        /// <typeparam name="T">Le type de l'objet à sauvegarder.</typeparam>
        /// <param name="obj">L'objet à sauvegarder.</param>
        /// <param name="fileFullname">Le chemin complet du fichier.</param>
        /// <param name="overwritten">Indique si le fichier doit être écrasé.</param>
        /// <returns>Vrai si l'objet a été sauvegardé avec succès, sinon faux.</returns>
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
        /// <summary>
        /// Ajoute une ressource dans le gestionnaire de ressources, ou la met à jour si elle existe déjà.
        /// </summary>
        /// <typeparam name="T">Le type de la ressource.</typeparam>
        /// <param name="resourceName">Le nom de la ressource.</param>
        /// <param name="resource">La ressource à ajouter ou mettre à jour.</param>
        /// <returns>Vrai si la ressource a été ajoutée ou mise à jour, sinon faux.</returns>
        public bool AddResource<T>(string resourceName, T resource)
        {
            if (!_values.TryAdd(resourceName, resource))
            {
                _values[resourceName] = resource;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Ajoute une nouvelle scène dans le gestionnaire de scènes par son nom et son type.
        /// </summary>
        /// <param name="name">Le nom de la scène.</param>
        /// <param name="type">Le type de la scène à ajouter.</param>
        /// <exception cref="ArgumentException">Lancé lorsque le nom est vide.</exception>
        /// <exception cref="InvalidSceneTypeException">Lancé lorsque le type ne dérive pas de Scene.</exception>
        /// <exception cref="DuplicateDictionaryKeyException">Lancé lorsque le nom de la scène existe déjà.</exception>
        public void AddScene(string name, Type type)
        {
            if (string.IsNullOrEmpty(name))
                throw new ArgumentException("The parameter 'name' must not be empty.");

            if (!Scene.IsAssignableFrom(type))
                throw new InvalidSceneTypeException(type);

            if (_scenes.ContainsKey(name))
                throw new DuplicateDictionaryKeyException(name);

            _scenes[name] = (Scene)Activator.CreateInstance(type, this);
        }
        /// <summary>
        /// Dessine la scène actuelle ou l'écran de chargement si la scène n'est pas encore chargée.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour dessiner.</param>
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
        /// <summary>
        /// Quitte le jeu.
        /// </summary>
        public void Exit() { _game.Exit(); }
        /// <summary>
        /// Récupère une ressource par son nom dans le gestionnaire de ressources.
        /// </summary>
        /// <typeparam name="T">Le type de la ressource à récupérer.</typeparam>
        /// <param name="resourceName">Le nom de la ressource.</param>
        /// <returns>La ressource du type spécifié.</returns>
        /// <exception cref="KeyNotFoundException">Lancé lorsque la ressource n'est pas trouvée.</exception>
        public T GetResource<T>(string resourceName)
        {
            if (_values.TryGetValue(resourceName, out object value))
                return (T)value;
            throw new KeyNotFoundException($"Resource '{resourceName}' not found in the resource manager.");
        }
        /// <summary>
        /// Définit l'écran de chargement actuel à un type spécifique implémentant ILoadingScreen.
        /// </summary>
        /// <typeparam name="T">Le type de l'écran de chargement.</typeparam>
        public void LoadingScreen<T>() where T : Scene, ILoadingScreen
        {
            Type type = typeof(T);
            Debug.Assert(typeof(T).IsSubclassOf(typeof(Scene)), "The type '" + type.Name + "' is not a valid ILoadingScreen type.");
            _loadingScreen = (Scene)Activator.CreateInstance(type, this);
        }
        /// <summary>
        /// Supprime une scène du gestionnaire de scènes par son nom.
        /// </summary>
        /// <param name="name">Le nom de la scène à supprimer.</param>
        /// <exception cref="ArgumentException">Lancé lorsque le nom est vide.</exception>
        public void RemoveScene(string name)
        {
            Debug.Assert(!string.IsNullOrEmpty(name), "The 'name' must not be empty.");
            Debug.Assert(_scenes.ContainsKey(name) && _currentScene == _scenes[name], "The current scene cannot be removed.");

            _scenes.Remove(name);
        }
        /// <summary>
        /// Supprime une ressource du gestionnaire de ressources par son nom.
        /// </summary>
        /// <param name="resourceName">Le nom de la ressource à supprimer.</param>
        public void RemoveResource(string resourceName) { _values.Remove(resourceName); }
        /// <summary>
        /// Réinitialise l'écran de chargement avec un nouveau message.
        /// </summary>
        /// <param name="message">Le message à afficher sur l'écran de chargement.</param>
        public void ResetLoadingScreen(string message)
        {
            _loadingScreen.Reset();
            if (_loadingScreen is ILoadingScreen ls)
                ls.Text = message;
        }
        /// <summary>
        /// Définit la scène actuelle à une nouvelle scène par son nom, avec un écran de chargement optionnel.
        /// </summary>
        /// <param name="name">Le nom de la scène à définir comme actuelle.</param>
        /// <param name="withLoadingScreen">Indique si un écran de chargement doit être affiché pendant la transition.</param>
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
        /// <summary>
        /// Définit le gestionnaire de périphériques graphiques utilisé pour gérer le périphérique graphique.
        /// </summary>
        /// <param name="graphicsDeviceManager">Le gestionnaire de périphériques graphiques à définir.</param>
        public void SetGraphicsDeviceManager(GraphicsDeviceManager graphicsDeviceManager)
        {
            GraphicsDeviceManager = graphicsDeviceManager;
        }
        /// <summary>
        /// Met à jour la scène actuelle ou l'écran de chargement avec le temps de jeu spécifié.
        /// </summary>
        /// <param name="gameTime">Le temps de jeu utilisé pour mettre à jour la scène.</param>
        public void Update(GameTime gameTime)
        {
            if (!_currentSceneLoaded)
                _loadingScreen?.Update(gameTime);
            else if (_currentScene != null &&  _currentScene.Loaded) 
                _currentScene.Update(gameTime);
        }

        internal void EndSpritebatch()
        {
            SpriteBatch.End();
            SpriteBatch.GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
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

