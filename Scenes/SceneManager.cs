using DinaFramework.Events;
using DinaFramework.Exceptions;
using DinaFramework.Functions;
using DinaFramework.Inputs;
using DinaFramework.Interfaces;
using DinaFramework.Internal;
using DinaFramework.Screen;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading.Tasks;

namespace DinaFramework.Scenes
{
    /// <summary>
    /// Gère les scènes du jeu, le passage d'une scène à l'autre, le chargement des ressources et l'affichage.
    /// Cette classe est un gestionnaire central pour les scènes, les ressources et la gestion des écrans de chargement.
    /// Elle garantit qu'une seule instance de la classe existe pendant l'exécution du jeu (Singleton).
    /// </summary>
    public sealed class SceneManager : IResource, IDisposable
    {
        #region === Initialisation & Singleton ===
        /// <summary>
        /// Obtient l'instance unique du SceneManager. Si l'instance n'existe pas, elle est créée.
        /// </summary>
        /// <param key="game">L'instance du jeu utilisée pour initialiser le gestionnaire de scènes.</param>
        public static void Initialize(Game game)
        {
            ArgumentNullException.ThrowIfNull(game);
            lock (_mutex)
            {
                _singleton ??= new SceneManager(game);
                ServiceLocator.Register(ServiceKeys.SceneManager, _singleton);
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
                return new SceneManager(this, Content.RootDirectory);
            return new SceneManager(this, contentRootDirectory);
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
            _content = new ContentManager(_game.Services, contentRootDirectory);

            _screenManager = ServiceLocator.Get<ScreenManager>(ServiceKeys.ScreenManager);
            if (_screenManager == null)
                throw new InvalidOperationException("ScreenManager non enregistré dans le ServiceLocator.");
            _screenManager.OnResolutionChanged += (sender, e) => HandleSceneManagerResolutionChanged();

            _frameworkLogoShown = true;
            _updateInputManager = false;

            _currentScene = null;
            _loadingScreen = null;
        }
        private SceneManager(Game game)
        {
            _game = game;
            _content = game.Content;
            GraphicsDevice = game.GraphicsDevice;
            _currentScene = null;
            _loadingScreen = null;
            _screenManager = ServiceLocator.Get<ScreenManager>(ServiceKeys.ScreenManager);
            if (_screenManager == null)
                throw new InvalidOperationException("ScreenManager non enregistré dans le ServiceLocator.");
            _screenManager.OnResolutionChanged += (sender, e) => HandleSceneManagerResolutionChanged();

            var pixel = new Texture2D(GraphicsDevice, 1, 1);
            pixel.SetData([Color.White]);

            _updateInputManager = true;

            // Enregistrement des textures dans le ServiceLocator
            ServiceLocator.Register(ServiceKeys.Texture1px, pixel);
            ServiceLocator.Register(ServiceKeys.DropDownArrow, InternalAssets.DropDownArrow(GraphicsDevice));
        }
        #endregion

        #region === Scènes ===
        /// <summary>
        /// Permet d'ajouter une scène au gestionnaire de scènes avec une clé spécifique.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sceneFactory"></param>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="DuplicateDictionaryKeyException"></exception>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddScene(Key<SceneTag> key, Func<Scene> sceneFactory)
        {
            if (key.Equals(default) || string.IsNullOrEmpty(key.Value))
                throw new ArgumentException("SceneKey cannot be null or empty.", nameof(key));

            if (_scenes.ContainsKey(key))
                throw new DuplicateDictionaryKeyException(key.Value);

            if (sceneFactory == null)
                throw new ArgumentNullException(nameof(sceneFactory), "SceneFactory cannot be null.");

            // Crée la scène via la factory et l'ajoute au dictionnaire
            _scenes[key] = sceneFactory();
        }
        /// <summary>
        /// Supprime une scène du gestionnaire de scènes par son nom.
        /// </summary>
        /// <param key="name">Le nom de la scène à supprimer.</param>
        /// <exception cref="ArgumentException">Lancé lorsque le nom est vide.</exception>
        public void RemoveScene(Key<SceneTag> name)
        {
            if (!_scenes.TryGetValue(name, out Scene? value))
                throw new KeyNotFoundException($"The scene '{name}' does not exist and cannot be removed.");

            if (_currentScene == value)
                throw new InvalidOperationException("The current scene cannot be removed.");

            _scenes.Remove(name);
        }
        /// <summary>
        /// Définit la scène actuelle à une nouvelle scène par son nom, avec un écran de chargement optionnel.
        /// </summary>
        /// <param key="name">Le nom de la scène à définir comme actuelle.</param>
        /// <param key="withLoadingScreen">Indique si un écran de chargement doit être affiché pendant la transition.</param>
        public async void SetCurrentScene(Key<SceneTag> name, bool withLoadingScreen = false)
        {
            // Interception du premier appel utilisateur
            if (!_frameworkLogoShown)
            {
                _frameworkLogoShown = true;
                _nextSceneName = name;
                _nextSceneWithLoading = withLoadingScreen;

                // Ajoute la scène interne du framework (non visible par l’utilisateur)
                if (!_scenes.ContainsKey(SceneKeys.FrameworkLogo))
                    AddScene(SceneKeys.FrameworkLogo, () => new FrameworkLogoScene(this));

                DinaFunctions.FireAndForget(BaseSetCurrentScene(SceneKeys.FrameworkLogo, false));
                return;
            }

            await BaseSetCurrentScene(name, withLoadingScreen).ConfigureAwait(false);
        }
        private async Task BaseSetCurrentScene(Key<SceneTag> name, bool withLoadingScreen = false)
        {
            if (!_scenes.TryGetValue(name, out Scene? value))
            {
                Trace.WriteLine("The scene '" + name + "' does not exists.");
                _currentScene = null;
                return;
            }

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
        internal void ContinueToNextScene()
        {
            SetCurrentScene(_nextSceneName, _nextSceneWithLoading);
        }
        /// <summary>
        /// Met à jour la scène actuelle ou l'écran de chargement avec le temps de jeu spécifié.
        /// </summary>
        /// <param key="gameTime">Le temps de jeu utilisé pour mettre à jour la scène.</param>
        public void Update(GameTime gameTime)
        {
            if (_updateInputManager)
                InputManager.Update();
            if (!_currentSceneLoaded)
                _loadingScreen?.Update(gameTime);
            else if (_currentScene != null && _currentScene.Loaded)
                _currentScene.Update(gameTime);
        }
        /// <summary>
        /// Dessine la scène actuelle ou l'écran de chargement si la scène n'est pas encore chargée.
        /// </summary>
        /// <param key="spritebatch">Le SpriteBatch utilisé pour dessiner.</param>
        public void Draw(SpriteBatch spritebatch, bool withBeginEnd = false)
        {
            if (spritebatch == null)
                return;

            if (withBeginEnd)
            {
                BeginSpriteBatch(spritebatch);
                DrawCurrentScene(spritebatch);
                EndSpriteBatch(spritebatch);
            }
            else
            {
                DrawCurrentScene(spritebatch);
            }
        }
        private void DrawCurrentScene(SpriteBatch spritebatch)
        {
            if (!_currentSceneLoaded)
                _loadingScreen?.Draw(spritebatch);
            else if (_currentScene != null && _currentSceneLoaded && _currentScene.Loaded)
                _currentScene.Draw(spritebatch);
        }
        #endregion

        #region === Écran de chargement ===
        /// <summary>
        /// Définit l'écran de chargement actuel à un type spécifique implémentant ILoadingScreen.
        /// </summary>
        /// <typeparam key="T">Le type de l'écran de chargement.</typeparam>
        public void LoadingScreen<T>() where T : Scene, ILoadingScreen
        {
            Type type = typeof(T);
            if (!Scene.IsAssignableFrom(type))
                throw new InvalidSceneTypeException(type);
            _loadingScreen = (Scene)Activator.CreateInstance(type, this)!;
        }
        /// <summary>
        /// Réinitialise l'écran de chargement (progression) et modifie le message affiché.
        /// </summary>
        /// <param name="message">Message affiché dans l'écran de chargement.</param>
        public void ResetLoadingScreen(string message)
        {
            _loadingScreen?.Reset();
            if (_loadingScreen is ILoadingScreen ls)
                ls.Text = message;
        }
        #endregion

        #region === Gestion des ressources ===
        /// <summary>
        /// Ajoute une ressource dans le gestionnaire de ressources, ou la met à jour si elle existe déjà.
        /// </summary>
        /// <typeparam key="T">Le type de la ressource.</typeparam>
        /// <param key="resourceName">Le nom de la ressource.</param>
        /// <param key="resource">La ressource à ajouter ou mettre à jour.</param>
        /// <returns>Vrai si la ressource a été ajoutée ou mise à jour, sinon faux.</returns>
        public bool AddResource<T>(string resourceName, T resource)
        {
            if (resource == null)
                throw new ArgumentNullException(nameof(resource), "Resource cannot be null.");
            if (!_values.TryAdd(resourceName, resource))
            {
                _values[resourceName] = resource;
                return true;
            }
            return false;
        }
        /// <summary>
        /// Récupère une ressource par son nom dans le gestionnaire de ressources.
        /// </summary>
        /// <typeparam key="T">Le type de la ressource à récupérer.</typeparam>
        /// <param key="resourceName">Le nom de la ressource.</param>
        /// <returns>La ressource du type spécifié.</returns>
        /// <exception cref="KeyNotFoundException">Lancé lorsque la ressource n'est pas trouvée.</exception>
        public T? GetResource<T>(string resourceName)
        {
            if (_values.TryGetValue(resourceName, out var value))
                return (T)value;
            Trace.WriteLine($"Resource '{resourceName}' not found in the resource manager.");
            return default;
        }
        /// <summary>
        /// Supprime une ressource du gestionnaire de ressources par son nom.
        /// </summary>
        /// <param key="resourceName">Le nom de la ressource à supprimer.</param>
        public void RemoveResource(string resourceName) { _values.Remove(resourceName); }
        /// <summary>
        /// Permet de décharger les ressources des scènes.
        /// </summary>
        public void Unload()
        {
            // On indique que les scènes n'ont pas été chargée afin que Load soit relancé lors de la prochaine utilisation de la scène.
            foreach (Scene scene in _scenes.Values)
                scene.Loaded = false;

            // On libère les ressources du ContentManager.
            Content?.Unload();
        }
        #endregion

        #region === SpriteBatch / Rendu ===
        /// <summary>
        /// Met à jour les paramètres actuels utilisés pour le rendu des sprites via un <see cref="SpriteBatch"/>.
        /// Chaque paramètre est optionnel ; si aucun paramètre n’est fourni, les valeurs actuelles restent inchangées.
        /// </summary>
        /// <param name="sortMode">Le mode de tri des sprites. Si null, conserve la valeur actuelle.</param>
        /// <param name="blendState">L'état de mélange des couleurs. Si null, conserve la valeur actuelle.</param>
        /// <param name="samplerState">L'état de l'échantillonneur pour le filtrage des textures. Si null, conserve la valeur actuelle.</param>
        /// <param name="depthStencilState">L'état de profondeur/stencil pour le rendu. Si null, conserve la valeur actuelle.</param>
        /// <param name="rasterizerState">L'état de rasterisation pour le rendu. Si null, conserve la valeur actuelle.</param>
        /// <param name="effect">L'effet (shader) à appliquer lors du rendu. Si null, conserve la valeur actuelle.</param>
        /// <param name="matrix">La matrice de transformation pour le rendu des sprites. Si null, conserve la valeur actuelle.</param>

        public void SetSpriteBatchParameters(
            SpriteSortMode? sortMode = null,
            BlendState? blendState = null,
            SamplerState? samplerState = null,
            DepthStencilState? depthStencilState = null,
            RasterizerState? rasterizerState = null,
            Effect? effect = null,
            Matrix? matrix = null)
        {
            _currentSpriteSortMode = sortMode ?? _currentSpriteSortMode;
            _currentBlendState = blendState ?? _currentBlendState;
            _currentSamplerState = samplerState ?? _currentSamplerState;
            _currentDepthStencilState = depthStencilState ?? _currentDepthStencilState;
            _currentRasterizerState = rasterizerState ?? _currentRasterizerState;
            _currentEffect = effect ?? _currentEffect;
            _currentMatrix = matrix ?? _currentMatrix;
        }
        /// <summary>
        /// Démarre le rendu des sprites en utilisant le <see cref="SpriteBatch"/> fourni et les paramètres actuellement configurés.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour le rendu. Si null, la méthode ne fait rien.</param>
        public void BeginSpriteBatch(SpriteBatch spritebatch)
        {
            spritebatch?.Begin(
                _currentSpriteSortMode,
                _currentBlendState,
                _currentSamplerState,
                _currentDepthStencilState,
                _currentRasterizerState,
                _currentEffect,
                _currentMatrix
                );
        }
        /// <summary>
        /// Termine le rendu des sprites sur le <see cref="SpriteBatch"/> fourni.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour le rendu. Si null, la méthode ne fait rien.</param>
        public static void EndSpriteBatch(SpriteBatch spritebatch)
        {
            spritebatch?.End();
        }
        /// <summary>
        /// Crée un RenderTarget2D avec les dimensions spécifiées.
        /// </summary>
        /// <param name="dimensions">Les dimensions du RenderTarget2D à créer.</param>
        /// <returns>Le RenderTarget2D créé.</returns>
        public RenderTarget2D CreateRenderTarget2D(Point dimensions)
        {
            return new RenderTarget2D(GraphicsDevice, dimensions.X, dimensions.Y);
        }

        /// <summary>
        /// Configure le <see cref="GraphicsDevice"/> pour dessiner sur un <see cref="RenderTarget2D"/> spécifique,
        /// puis démarre le rendu des sprites avec le <see cref="SpriteBatch"/> fourni.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour le rendu. Ne peut pas être null.</param>
        /// <param name="renderTarget">Le RenderTarget2D sur lequel dessiner. Ne peut pas être null.</param>
        public void BeginSpriteBatchRenderTarget(SpriteBatch spritebatch, RenderTarget2D renderTarget)
        {
            ArgumentNullException.ThrowIfNull(spritebatch, nameof(spritebatch));
            ArgumentNullException.ThrowIfNull(renderTarget, nameof(renderTarget));
            spritebatch.GraphicsDevice.SetRenderTarget(renderTarget);
            BeginSpriteBatch(spritebatch);
        }
        /// <summary>
        /// Termine le rendu avec le <see cref="SpriteBatch"/> sur un <see cref="RenderTarget2D"/> 
        /// et rétablit le <see cref="GraphicsDevice"/> pour dessiner sur l'écran principal.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour le rendu. Ne peut pas être null.</param>
        public static void EndSpriteBatchRenderTarget(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch, nameof(spritebatch));
            EndSpriteBatch(spritebatch);
            spritebatch.GraphicsDevice.SetRenderTarget(null);
        }
        /// <summary>
        /// Permet de remettre les paramèetres du SpriteBatch aux valeurs par défaut.
        /// </summary>
        public void ResetSpriteBatchParameters()
        {
            _currentSpriteSortMode = _defaultSpriteSortMode;
            _currentBlendState = _defaultBlendState;
            _currentSamplerState = _defaultSamplerState;
            _currentDepthStencilState = _defaultDepthStencilState;
            _currentRasterizerState = _defaultRasterizerState;
            _currentEffect = _defaultEffect;
            _currentMatrix = _defaultMatrix;
        }
        #endregion

        #region === Utilitaires ===
        /// <summary>
        /// Quitte le jeu.
        /// </summary>
        public void Exit() { _game.Exit(); }

        /// <summary>
        /// Événement déclenché lorsque la résolution de la scène change.
        /// Les abonnés peuvent réagir à ce changement pour ajuster le rendu ou l'interface.
        /// </summary>
#pragma warning disable CS0067
        public event EventHandler<SceneEventArgs>? OnResolutionChanged;
#pragma warning restore CS0067

        private void HandleSceneManagerResolutionChanged()
        {
            if (_currentScene != null)
            {
                Unload();
                _currentScene.Dispose();
                _currentScene.Load();
                _currentScene.Loaded = true;
                _currentSceneLoaded = true;
            }
        }
        #endregion

        #region === Attributs & membres internes ===
        // Singleton interne
        private static SceneManager? _singleton;
        private static readonly object _mutex = new object();

        // Game et Content
        private readonly Game _game;
        private readonly ContentManager _content;

        // Scènes
        private readonly Dictionary<Key<SceneTag>, Scene> _scenes = [];
        private Scene? _currentScene;
        private Key<SceneTag> _currentSceneName;
        private Scene? _loadingScreen;
        private bool _currentSceneLoaded;
        private float _loadingProgress;
        private bool _frameworkLogoShown;
        private readonly bool _updateInputManager;
        private Key<SceneTag> _nextSceneName;
        private bool _nextSceneWithLoading;

        // Ressources
        private readonly Dictionary<string, object> _values = [];

        // Screen & RenderTarget
        private readonly ScreenManager? _screenManager;
        private Color _backgroundcolor;

        // SpriteBatch parameters
        private BlendState? _currentBlendState = BlendState.AlphaBlend;
        private SpriteSortMode _currentSpriteSortMode = SpriteSortMode.Deferred;
        private SamplerState? _currentSamplerState;
        private DepthStencilState? _currentDepthStencilState;
        private RasterizerState? _currentRasterizerState;
        private Effect? _currentEffect;
        private Matrix? _currentMatrix;
#pragma warning disable CS0649
        private readonly BlendState? _defaultBlendState = BlendState.AlphaBlend;
        private readonly SpriteSortMode _defaultSpriteSortMode = SpriteSortMode.Deferred;
        private readonly SamplerState? _defaultSamplerState;
        private readonly DepthStencilState? _defaultDepthStencilState;
        private readonly RasterizerState? _defaultRasterizerState;
        private readonly Effect? _defaultEffect;
        private readonly Matrix? _defaultMatrix;
#pragma warning restore CS0649
        #endregion

        #region === Propriétés publiques ===
        /// <summary>
        /// Indique si la fenêtre du jeu a actuellement le focus.
        /// </summary>
        public static bool HasFocus { get; set; }
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
        public Vector2 ScreenDimensions
        {
            get
            {
                if (_screenManager == null)
                    throw new InvalidOperationException("ScreenManager non enregistré dans le ServiceLocator.");
                return _screenManager.CurrentResolution.ToVector2();
            }
        }

        /// <summary>
        /// Obtient le nom de la scène courante.
        /// </summary>
        public Key<SceneTag> CurrentSceneName => _currentSceneName;
        /// <summary>
        /// Obtient la scène courante.
        /// </summary>
        public Scene? CurrentScene => _currentScene;
        /// <summary>
        /// Obtient le ContentManager utilisé pour charger et gérer les ressources du jeu.
        /// </summary>
        public ContentManager Content => _content;
        /// <summary>
        /// Obtient ou définit la couleur de fond utilisée pour le rendu des scènes.
        /// </summary>
        public Color BackgroundColor { get => _backgroundcolor; set => _backgroundcolor = value; }
        /// <summary>
        /// Obtient le périphérique graphique.
        /// </summary>
        public GraphicsDevice GraphicsDevice { get; private set; }
        /// <summary>
        /// Obtient ou définit le SpriteBatch utilisé pour dessiner les sprites 2D.
        /// </summary>
        public SpriteBatch? SpriteBatch { get; set; }
        #endregion

        /// <summary>
        /// Libère les ressources utilisées par le SceneManager.
        /// </summary>
        public void Dispose()
        {
            _content.Dispose();
            _currentBlendState?.Dispose();
            _defaultBlendState?.Dispose();
            _defaultSamplerState?.Dispose();
        }
    }
}

