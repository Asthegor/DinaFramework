using DinaFramework.Controls;
using DinaFramework.Exceptions;
using DinaFramework.Interfaces;
using DinaFramework.Screen;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    [EditorBrowsable(EditorBrowsableState.Never)]
    public class SceneManager : IResource
    {
        /// <summary>
        /// Indique si la fenêtre du jeu a actuellement le focus.
        /// </summary>
        public static bool HasFocus { get; set; }


        private static SceneManager _singleton;
        private static readonly object _mutex = new object();
        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { WriteIndented = true };

        private readonly Dictionary<string, object> _values = [];
        private readonly Dictionary<SceneKey, Scene> _scenes = [];

        private readonly Game _game;

        private ContentManager _content;
        private Scene _currentScene;
        private SceneKey _currentSceneName;
        private Scene _loadingScreen;
        private ScreenManager _screenManager;
        private bool _currentSceneLoaded;
        private float _loadingProgress;
        private BlendState _blendState = BlendState.AlphaBlend;
        private bool _temporaryBlendState;

        //private SceneTransitionManager _transitionManager;
        //private bool _transitionActive;

        private bool _frameworkLogoShown;
        private SceneKey _nextSceneName;
        private bool _nextSceneWithLoading;

        private bool _ownsContent;

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
        public Vector2 ScreenDimensions => _screenManager.CurrentResolution.ToVector2();
        /// <summary>
        /// Obtient le contrôleur du joueur pour gérer les entrées du joueur.
        /// </summary>
        public PlayerController Controller { get; set; }
        /// <summary>
        /// Obtient le périphérique graphique.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }
        /// <summary>
        /// Obtient ou définit le SpriteBatch utilisé pour dessiner les sprites 2D.
        /// </summary>
        public SpriteBatch SpriteBatch { get; set; }
        /// <summary>
        /// Obtient le ContentManager utilisé pour charger et gérer les ressources du jeu.
        /// </summary>
        public ContentManager Content { get => _content; private set => _content = value; }
        /// <summary>
        /// Obtient le nom de la scène courante.
        /// </summary>
        public SceneKey CurrentSceneName => _currentSceneName;
        /// <summary>
        /// Obtient la scène courante.
        /// </summary>
        public Scene CurrentScene => _currentScene;

        // Méthodes statiques
        /// <summary>
        /// Obtient l'instance unique du SceneManager. Si l'instance n'existe pas, elle est créée.
        /// </summary>
        /// <param key="game">L'instance du jeu utilisée pour initialiser le gestionnaire de scènes.</param>
        /// <returns>L'instance unique du SceneManager.</returns>
        public static void Initialize(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);
            lock (_mutex)
            {
                _singleton ??= new SceneManager(game);
                ServiceLocator.Register(ServiceKey.SceneManager, _singleton);
            }
        }
        /// <summary>
        /// Retourne l'instance unique du SceneManager.
        /// </summary>
        public static SceneManager Singleton => _singleton ?? throw new InvalidOperationException("SceneManager non initialisé.");
        /// <summary>
        /// Permet de créer une nouvelle instance de SceneManager avec un répertoire racine spécifique pour le ContentManager.
        /// </summary>
        /// <param name="contentRootDirectory"></param>
        /// <returns></returns>
        public SceneManager CreateNewInstance(string contentRootDirectory = "")
        {
            if (string.IsNullOrEmpty(contentRootDirectory))
                return new SceneManager(this);
            return new SceneManager(this, contentRootDirectory);
        }
        /// <summary>
        /// Constructeur de copie pour créer une nouvelle instance de SceneManager à partir d'une instance existante.
        /// Cette version utilise le même Content que le projet d'origine.
        /// </summary>
        /// <param name="source"></param>
        private SceneManager(SceneManager source) : this(source, source.Content.RootDirectory)
        {
        }
        /// <summary>
        /// Constructeur de copie pour créer une nouvelle instance de SceneManager à partir d'une instance existante,
        /// Cette version utilise son propre ContentManager avec un répertoire racine spécifique.
        /// </summary>
        /// <param name="source"></param>
        /// <param name="contentRootDirectory"></param>
        private SceneManager(SceneManager source, string contentRootDirectory)
        {
            ArgumentNullException.ThrowIfNull(source);
            ArgumentNullException.ThrowIfNull(contentRootDirectory);

            _game = source._game;
            GraphicsDevice = source.GraphicsDevice;
            Content = new ContentManager(_game.Services, contentRootDirectory);
            _ownsContent = true;

            _screenManager = ServiceLocator.Get<ScreenManager>(ServiceKey.ScreenManager);
            _screenManager.OnResolutionChanged += HandleResolutionChanged;

            _currentScene = null;
            _loadingScreen = null;
            Controller = DefaultControls.DefaultKeyboard;
        }

        /// <summary>
        /// Permet de décharger les ressources des scènes.
        /// </summary>
        public void Unload()
        {
            // On indique que les scènes n'ont pas été chargée afin que Load soit relancé lors de la prochaine utilisation de la scène.
            foreach(Scene scene in _scenes.Values)
                scene.Loaded = false;

            // On libère les ressources du ContentManager.
            Content?.Unload();
        }

        /// <summary>
        /// Charge un objet depuis un fichier crypté et le désérialise dans le type spécifié.
        /// </summary>
        /// <typeparam key="T">Le type de l'objet à charger.</typeparam>
        /// <param key="filePath">Le chemin du fichier crypté.</param>
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
        /// <typeparam key="T">Le type de l'objet à sauvegarder.</typeparam>
        /// <param key="obj">L'objet à sauvegarder.</param>
        /// <param key="fileFullname">Le chemin complet du fichier.</param>
        /// <param key="overwritten">Indique si le fichier doit être écrasé.</param>
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
        /// <typeparam key="T">Le type de la ressource.</typeparam>
        /// <param key="resourceName">Le nom de la ressource.</param>
        /// <param key="resource">La ressource à ajouter ou mettre à jour.</param>
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
        /// Permet d'ajouter une scène au gestionnaire de scènes avec une clé spécifique.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sceneFactory"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DuplicateDictionaryKeyException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddScene(SceneKey key, Func<Scene> sceneFactory)
        {
            if (key.Equals(default(SceneKey)) || string.IsNullOrEmpty(key.ToString()))
                throw new ArgumentException("SceneKey cannot be null or empty.", nameof(key));

            if (_scenes.ContainsKey(key))
                throw new DuplicateDictionaryKeyException(key.ToString());

            if (sceneFactory == null)
                throw new ArgumentNullException(nameof(sceneFactory), "SceneFactory cannot be null.");

            // Crée la scène via la factory et l'ajoute au dictionnaire
            _scenes[key] = sceneFactory();
        }
        /// <summary>
        /// Dessine la scène actuelle ou l'écran de chargement si la scène n'est pas encore chargée.
        /// </summary>
        /// <param key="spritebatch">Le SpriteBatch utilisé pour dessiner.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (spritebatch != null)
            {
                spritebatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);
                if (!_currentSceneLoaded)
                    _loadingScreen?.Draw(spritebatch);
                else if (_currentScene != null && _currentSceneLoaded && _currentScene.Loaded)
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
        /// <typeparam key="T">Le type de la ressource à récupérer.</typeparam>
        /// <param key="resourceName">Le nom de la ressource.</param>
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
        /// <typeparam key="T">Le type de l'écran de chargement.</typeparam>
        public void LoadingScreen<T>() where T : Scene, ILoadingScreen
        {
            Type type = typeof(T);
            if (!Scene.IsAssignableFrom(type))
                throw new InvalidSceneTypeException(type);
            _loadingScreen = (Scene)Activator.CreateInstance(type, this);
        }
        /// <summary>
        /// Supprime une scène du gestionnaire de scènes par son nom.
        /// </summary>
        /// <param key="name">Le nom de la scène à supprimer.</param>
        /// <exception cref="ArgumentException">Lancé lorsque le nom est vide.</exception>
        public void RemoveScene(SceneKey name)
        {
            if (!_scenes.TryGetValue(name, out Scene value))
                throw new KeyNotFoundException($"The scene '{name}' does not exist and cannot be removed.");

            if (_currentScene == value)
                throw new InvalidOperationException("The current scene cannot be removed.");

            _scenes.Remove(name);
        }

        /// <summary>
        /// Supprime une ressource du gestionnaire de ressources par son nom.
        /// </summary>
        /// <param key="resourceName">Le nom de la ressource à supprimer.</param>
        public void RemoveResource(string resourceName) { _values.Remove(resourceName); }
        /// <summary>
        /// Réinitialise l'écran de chargement avec un nouveau message.
        /// </summary>
        /// <param key="message">Le message à afficher sur l'écran de chargement.</param>
        public void ResetLoadingScreen(string message)
        {
            _loadingScreen.Reset();
            if (_loadingScreen is ILoadingScreen ls)
                ls.Text = message;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param key="blendState"></param>
        /// <param key="temporary"></param>
        public void SetBlendState(BlendState blendState, bool temporary = true)
        {
            _blendState = blendState ?? BlendState.AlphaBlend;
            _temporaryBlendState = temporary;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param key="name"></param>
        /// <param key="withLoadingScreen"></param>
        public async void SetCurrentScene(SceneKey name, bool withLoadingScreen = false)
        {
            // Interception du premier appel utilisateur
            if (!_frameworkLogoShown)
            {
                _frameworkLogoShown = true;
                _nextSceneName = name;
                _nextSceneWithLoading = withLoadingScreen;

                // Ajoute la scène interne du framework (non visible par l’utilisateur)
                if (!_scenes.ContainsKey(SceneKey.FrameworkLogo))
                    _scenes[SceneKey.FrameworkLogo] = new FrameworkLogoScene(this); // Pas besoin de type public

                BaseSetCurrentScene(SceneKey.FrameworkLogo, false); // Pas d'écran de chargement

                return;
            }

            await BaseSetCurrentScene(name, withLoadingScreen);
        }

        /// <summary>
        /// Définit la scène actuelle à une nouvelle scène par son nom, avec un écran de chargement optionnel.
        /// </summary>
        /// <param key="name">Le nom de la scène à définir comme actuelle.</param>
        /// <param key="withLoadingScreen">Indique si un écran de chargement doit être affiché pendant la transition.</param>
        private async Task BaseSetCurrentScene(SceneKey name, bool withLoadingScreen = false)
        {
            if (!_scenes.TryGetValue(name, out Scene value))
            {
                Trace.WriteLine("The scene '" + name + "' does not exists.");
                _currentScene = null;
                return;
            }

            _currentScene?.Dispose(); // Dispose de l'ancienne scène si elle existe

            ControllerKey.ResetAllKeys();

            _currentScene = _scenes[name];
            _currentSceneName = name;

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
        /// Met à jour la scène actuelle ou l'écran de chargement avec le temps de jeu spécifié.
        /// </summary>
        /// <param key="gameTime">Le temps de jeu utilisé pour mettre à jour la scène.</param>
        public void Update(GameTime gameTime)
        {
            if (!_currentSceneLoaded)
                _loadingScreen?.Update(gameTime);
            else if (_currentScene != null && _currentScene.Loaded)
                _currentScene.Update(gameTime);
        }

        internal void EndSpritebatch()
        {
            SpriteBatch.End();
            SpriteBatch.GraphicsDevice.SetRenderTarget(null);

            SpriteBatch.Begin(SpriteSortMode.Deferred, _blendState);

            if (_temporaryBlendState)
            {
                _blendState = BlendState.AlphaBlend;
                _temporaryBlendState = false;
            }
        }

        internal void ContinueToNextScene()
        {
            SetCurrentScene(_nextSceneName, _nextSceneWithLoading);
        }

        // Fonctions privées
        private SceneManager(Game game)
        {
            _game = game;
            Content = game.Content;
            GraphicsDevice = game.GraphicsDevice;
            _currentScene = null;
            _loadingScreen = null;
            _screenManager = ServiceLocator.Get<ScreenManager>(ServiceKey.ScreenManager);

            _screenManager.OnResolutionChanged += HandleResolutionChanged;

            // Par défaut, on utilise le clavier
            Controller = DefaultControls.DefaultKeyboard;

            var pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData([Color.White]);
            ServiceLocator.Register(ServiceKey.Texture1px, pixel);
        }

        /// <summary>
        /// 
        /// </summary>
        public event Action<Vector2> OnResolutionChanged;

        /// <summary>
        /// 
        /// </summary>
        /// <param key="newResolution"></param>
        private void HandleResolutionChanged(Vector2 newResolution)
        {
            OnResolutionChanged?.Invoke(newResolution);
        }

    }
}

