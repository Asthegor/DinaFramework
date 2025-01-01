using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Scenes
{
    /// <summary>
    /// Représente une scène dans le jeu, qui contient la logique de chargement, de mise à jour, de dessin et de gestion des ressources.
    /// </summary>
    /// <remarks>
    /// La classe Scene est une classe abstraite qui sert de base pour toutes les scènes du jeu.
    /// Elle permet de gérer les ressources, les dimensions de l'écran, le rendu et les transitions entre les scènes.
    /// Les classes dérivées doivent implémenter les méthodes abstraites Load, Reset, Update, et Draw.
    /// </remarks>
    public abstract class Scene : IGameObject, IResource
    {
        /// <summary>
        /// Vérifie si un type donné est une sous-classe de Scene.
        /// </summary>
        /// <param name="type">Le type à vérifier.</param>
        /// <returns>True si le type est une sous-classe de Scene; sinon, false.</returns>
        public static bool IsAssignableFrom(Type type)
        {
            return (type != null && typeof(Scene).IsAssignableFrom(type));
        }
        private readonly SceneManager _sceneManager;
        /// <summary>
        /// Obtient le gestionnaire de scènes associé à cette scène.
        /// </summary>
        protected SceneManager SceneManager => _sceneManager;
        /// <summary>
        /// Initialise une nouvelle instance de la classe Scene avec un gestionnaire de scènes spécifié.
        /// </summary>
        /// <param name="sceneManager">Le gestionnaire de scènes à associer à cette scène.</param>
        /// <remarks>
        /// La Scene est automatiquement créée lorsqu'on l'ajoute au gestionnaire de scènes par la fonction AddScene.
        /// </remarks>
        protected Scene(SceneManager sceneManager)
        {
            _sceneManager = sceneManager;
        }
        /// <summary>
        /// Indique si la scène est chargée ou non.
        /// </summary>
        public bool Loaded { get; set; }
        /// <summary>
        /// Obtient les dimensions de l'écran du jeu.
        /// </summary>
        protected Vector2 ScreenDimensions => _sceneManager.ScreenDimensions;
        /// <summary>
        /// Obtient le SpriteBatch utilisé pour dessiner les éléments graphiques de cette scène.
        /// </summary>
        protected SpriteBatch SpriteBatch => _sceneManager.SpriteBatch;
        /// <summary>
        /// Obtient ou définit le progrès du chargement de la scène, compris entre 0 et 1.
        /// </summary>
        protected float LoadingProgress
        {
            get => _sceneManager.LoadingProgress;
            set => _sceneManager.LoadingProgress = value;
        }
        /// <summary>
        /// Obtient le gestionnaire de contenu utilisé pour charger les ressources de cette scène.
        /// </summary>
        protected ContentManager Content => _sceneManager.Content;
        private bool _visible;
        /// <summary>
        /// Indique si cette scène est visible ou non.
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }

        /// <summary>
        /// Charge la scène. Cette méthode doit être implémentée par les classes dérivées.
        /// </summary>
        public abstract void Load();
        /// <summary>
        /// Réinitialise la scène. Cette méthode doit être implémentée par les classes dérivées.
        /// </summary>
        public abstract void Reset();
        /// <summary>
        /// Met à jour la scène. Cette méthode doit être implémentée par les classes dérivées.
        /// </summary>
        /// <param name="gametime">Le temps de jeu actuel.</param>
        public abstract void Update(GameTime gametime);
        /// <summary>
        /// Dessine la scène. Cette méthode doit être implémentée par les classes dérivées.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour dessiner les éléments graphiques.</param>
        public abstract void Draw(SpriteBatch spritebatch);

        /// <summary>
        /// Ajoute une ressource à la scène, en utilisant le gestionnaire de scènes.
        /// </summary>
        /// <typeparam name="T">Le type de la ressource.</typeparam>
        /// <param name="resourceName">Le nom de la ressource.</param>
        /// <param name="resource">La ressource à ajouter.</param>
        /// <returns>Vrai si la ressource a été ajoutée avec succès, sinon faux.</returns>
        public bool AddResource<T>(string resourceName, T resource) { return _sceneManager.AddResource(resourceName, resource); }
        /// <summary>
        /// Obtient une ressource par son nom.
        /// </summary>
        /// <typeparam name="T">Le type de la ressource à récupérer.</typeparam>
        /// <param name="resourceName">Le nom de la ressource.</param>
        /// <returns>La ressource du type spécifié.</returns>
        /// <exception cref="ArgumentNullException">Lancé lorsque le nom de la ressource est nul ou vide.</exception>
        public T GetResource<T>(string resourceName)
        {
            if (string.IsNullOrEmpty(resourceName))
                throw new ArgumentNullException(nameof(resourceName), "The parameter 'resourceName' must not be empty.");

            return _sceneManager.GetResource<T>(resourceName);
        }
        /// <summary>
        /// Supprime une ressource du gestionnaire de ressources.
        /// </summary>
        /// <param name="resourceName">Le nom de la ressource à supprimer.</param>
        public void RemoveResource(string resourceName) { _sceneManager.RemoveResource(resourceName); }
        /// <summary>
        /// Quitte le jeu en cours en utilisant le gestionnaire de scènes.
        /// </summary>
        protected void Exit() { _sceneManager.Exit(); }
        /// <summary>
        /// Définit la scène actuelle à une nouvelle scène par son nom, avec ou sans écran de chargement.
        /// </summary>
        /// <param name="name">Le nom de la scène à définir comme actuelle.</param>
        /// <param name="withLoadingScreen">Indique si un écran de chargement doit être affiché pendant la transition.</param>
        protected void SetCurrentScene(string name, bool withLoadingScreen = false)
        {
            _sceneManager.SetCurrentScene(name, withLoadingScreen);
        }
        /// <summary>
        /// Crée un RenderTarget2D avec les dimensions spécifiées.
        /// </summary>
        /// <param name="dimensions">Les dimensions du RenderTarget2D à créer.</param>
        /// <returns>Le RenderTarget2D créé.</returns>
        protected RenderTarget2D CreateRenderTarget2D(Vector2 dimensions)
        {
            return new RenderTarget2D(_sceneManager.GraphicsDeviceManager.GraphicsDevice, (int)dimensions.X, (int)dimensions.Y);
        }
        /// <summary>
        /// Définit le RenderTarget2D pour le rendu des graphiques de cette scène.
        /// </summary>
        /// <param name="renderTarget">Le RenderTarget2D à définir.</param>
        protected void SetRenderTarget2D(RenderTarget2D renderTarget)
        {
            _sceneManager.SpriteBatch.GraphicsDevice.SetRenderTarget(renderTarget);
        }
        /// <summary>
        /// Efface l'écran avec la couleur spécifiée.
        /// </summary>
        /// <param name="color">La couleur utilisée pour effacer l'écran.</param>
        protected void ClearScreen(Color color)
        {
            _sceneManager.SpriteBatch.GraphicsDevice.Clear(color);
        }
        /// <summary>
        /// Commence le processus de dessin des sprites, avec des options personnalisables.
        /// </summary>
        /// <param name="sortMode">Le mode de tri des sprites.</param>
        /// <param name="blendState">L'état de fusion des sprites.</param>
        /// <param name="samplerState">L'état de l'échantillonneur.</param>
        /// <param name="depthStencilState">L'état du stencil de profondeur.</param>
        /// <param name="rasterizerState">L'état du rasterizer.</param>
        /// <param name="effect">L'effet appliqué aux sprites.</param>
        /// <param name="transformMatrix">La matrice de transformation à appliquer aux sprites.</param>
        protected void BeginSpritebatch(SpriteSortMode sortMode = SpriteSortMode.Deferred, BlendState blendState = null, SamplerState samplerState = null, DepthStencilState depthStencilState = null, RasterizerState rasterizerState = null, Effect effect = null, Matrix? transformMatrix = null)
        {
            _sceneManager.SpriteBatch.Begin(sortMode, blendState, samplerState, depthStencilState, rasterizerState, effect, transformMatrix);
        }
        /// <summary>
        /// Réinitialise l'écran de chargement avec le message spécifié.
        /// </summary>
        /// <param name="message">Le message à afficher sur l'écran de chargement.</param>
        protected void ResetLoadingScreen(string message)
        {
            _sceneManager.ResetLoadingScreen(message);
        }
    }
}
