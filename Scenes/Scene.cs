using DinaFramework.Events;
using DinaFramework.Exceptions;
using DinaFramework.Interfaces;
using DinaFramework.Services;

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
    public abstract class Scene : IFullGameObject, IResource
    {
        /// <summary>
        /// Initialise une nouvelle instance de la classe <see cref="Scene"/> et lie la scène au gestionnaire de scènes spécifié.
        /// S'abonne également à l'événement de changement de résolution pour que la scène puisse réagir aux modifications de l'écran.
        /// </summary>
        /// <param name="sceneManager">Le <see cref="SceneManager"/> qui gère cette scène.</param>
        protected Scene(SceneManager sceneManager)
        {
            SceneManager = sceneManager;
            SceneManager.OnResolutionChanged += (sender, e) => HandleSceneResolutionChanged(e.Scene);
        }

        private bool _isSpritebatchBegin;
        /// <summary>
        /// Vérifie si un type donné est une sous-classe de Scene.
        /// </summary>
        /// <param name="type">Le type à vérifier.</param>
        /// <returns>True si le type est une sous-classe de Scene; sinon, false.</returns>
        public static bool IsAssignableFrom(Type type)
        {
            return (type != null && typeof(Scene).IsAssignableFrom(type));
        }

        /// <summary>
        /// Obtient le gestionnaire de scènes associé à cette scène.
        /// </summary>
        protected SceneManager SceneManager { get; private set; }

        /// <summary>
        /// Indique si la scène est chargée ou non.
        /// </summary>
        public bool Loaded { get; set; }
        /// <summary>
        /// Obtient les dimensions de l'écran du jeu.
        /// </summary>
        protected Vector2 ScreenDimensions => SceneManager.ScreenDimensions;
        /// <summary>
        /// Obtient le SpriteBatch utilisé pour dessiner les éléments graphiques de cette scène.
        /// </summary>
        protected SpriteBatch? SpriteBatch => SceneManager.SpriteBatch;
        /// <summary>
        /// Obtient ou définit le progrès du chargement de la scène, compris entre 0 et 1.
        /// </summary>
        protected float LoadingProgress
        {
            get => SceneManager.LoadingProgress;
            set => SceneManager.LoadingProgress = value;
        }
        /// <summary>
        /// Obtient le gestionnaire de contenu utilisé pour charger les ressources de cette scène.
        /// </summary>
        protected ContentManager Content => SceneManager.Content;
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
        public bool AddResource<T>(string resourceName, T resource) { return SceneManager.AddResource(resourceName, resource); }
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

            return SceneManager.GetResource<T>(resourceName);
        }
        /// <summary>
        /// Supprime une ressource du gestionnaire de ressources.
        /// </summary>
        /// <param name="resourceName">Le nom de la ressource à supprimer.</param>
        public void RemoveResource(string resourceName) { SceneManager.RemoveResource(resourceName); }
        /// <summary>
        /// Quitte le jeu en cours en utilisant le gestionnaire de scènes.
        /// </summary>
        protected void Exit() { SceneManager.Exit(); }
        /// <summary>
        /// Définit la scène actuelle à une nouvelle scène par son nom, avec ou sans écran de chargement.
        /// </summary>
        /// <param name="name">Le nom de la scène à définir comme actuelle.</param>
        /// <param name="withLoadingScreen">Indique si un écran de chargement doit être affiché pendant la transition.</param>
        protected void SetCurrentScene(Key<SceneTag> name, bool withLoadingScreen = false)
        {
            SceneManager.SetCurrentScene(name, withLoadingScreen);
        }
        /// <summary>
        /// Crée un RenderTarget2D avec les dimensions spécifiées.
        /// </summary>
        /// <param name="dimensions">Les dimensions du RenderTarget2D à créer.</param>
        /// <returns>Le RenderTarget2D créé.</returns>
        protected RenderTarget2D CreateRenderTarget2D(Vector2 dimensions)
        {
            return new RenderTarget2D(SceneManager.GraphicsDevice, (int)dimensions.X, (int)dimensions.Y);
        }
        /// <summary>
        /// Définit le RenderTarget2D pour le rendu des graphiques de cette scène.
        /// </summary>
        /// <param name="renderTarget">Le RenderTarget2D à définir.</param>
        protected void SetRenderTarget2D(RenderTarget2D renderTarget)
        {
            SceneManager.GraphicsDevice.SetRenderTarget(renderTarget);
        }
        /// <summary>
        /// Efface l'écran avec la couleur spécifiée.
        /// </summary>
        /// <param name="color">La couleur utilisée pour effacer l'écran.</param>
        protected void ClearScreen(Color color)
        {
            SceneManager.GraphicsDevice.Clear(color);
        }
        /// <summary>
        /// Commence le processus de dessin des sprites, avec des options personnalisables.
        /// </summary>
        /// <param name="spritebatch">Spritebatch</param>
        protected void BeginSpritebatch(SpriteBatch spritebatch)
        {
            SceneManager.EndSpriteBatch(spritebatch);
            SceneManager.BeginSpriteBatch(spritebatch);
            _isSpritebatchBegin = true;
        }
        /// <summary>
        /// Réinitialise l'écran de chargement avec le message spécifié.
        /// </summary>
        /// <param name="message">Le message à afficher sur l'écran de chargement.</param>
        protected void ResetLoadingScreen(string message)
        {
            SceneManager.ResetLoadingScreen(message);
        }

        /// <summary>
        /// Termine le processus de dessin des sprites.
        /// </summary>
        protected void EndSpritebatch(SpriteBatch spritebatch)
        {
            if (!_isSpritebatchBegin)
                throw new SpriteBatchNotBeginException();

            SceneManager.EndSpriteBatch(spritebatch);
            _isSpritebatchBegin = false;
        }


        // Gestion du changement de résolution de l'écran

        /// <summary>
        /// Événement déclenché lorsque la résolution de la scène change.
        /// Les abonnés peuvent réagir à ce changement pour ajuster le rendu ou l'interface.
        /// </summary>
        public event EventHandler<SceneEventArgs>? OnResolutionChanged;
        private void HandleSceneResolutionChanged(Scene scene)
        {
            OnResolutionChanged?.Invoke(scene, new SceneEventArgs(scene));
        }
        /// <summary>
        /// Détache tous les abonnés à l'événement <see cref="OnResolutionChanged"/>.
        /// </summary>
        public virtual void Dispose()
        {
            OnResolutionChanged = null; // Détache tous les abonnés
        }
    }
}
