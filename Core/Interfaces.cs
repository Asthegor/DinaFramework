#pragma warning disable CS1591 // Pour ne pas devoir avoir un commentaire sur les membre des interfaces
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace DinaFramework.Interfaces
{
    /// <summary>
    /// Définit un objet de jeu complet comprenant le chargement, la réinitialisation, la mise à jour et l'affichage.
    /// </summary>
    public interface IFullGameObject : ILoad, IReset, IUpdate, IDraw
    {}
    /// <summary>
    /// Définit un objet de jeu comprenant la mise à jour et l'affichage'.
    /// </summary>
    public interface IGameObject : IUpdate, IDraw
    { }

    /// <summary>
    /// Représente un élément ayant une position, des dimensions et un ordre Z et pouvant se dessiner.
    /// </summary>
    public interface IDrawingElement : IElement, IDraw
    { }
    /// <summary>
    /// Représente un élément ayant une position, des dimensions et un ordre Z.
    /// </summary>
    public interface IElement : IPosition, IDimensions, IZOrder
    {}
    /// <summary>
    /// Définit une propriété pour gérer l'ordre d'affichage (Z-order).
    /// </summary>
    public interface IZOrder
    {
        public abstract int ZOrder { get; set; }
    }
    /// <summary>
    /// Définit une méthode pour charger des ressources ou des données.
    /// </summary>
    public interface ILoad
    {
        public abstract void Load();
    }
    /// <summary>
    /// Définit une méthode pour mettre à jour l'état d'un objet.
    /// </summary>
    public interface IUpdate
    {
        public abstract void Update(GameTime gametime);
    }
    /// <summary>
    /// Définit une méthode pour dessiner un objet.
    /// </summary>
    public interface IDraw
    {
        public abstract void Draw(SpriteBatch spritebatch);
    }
    /// <summary>
    /// Définit une méthode pour réinitialiser un objet à son état initial.
    /// </summary>
    public interface IReset
    {
        public abstract void Reset();
    }
    /// <summary>
    /// Définit une propriété pour la position d'un objet.
    /// </summary>
    public interface IPosition
    {
        public abstract Vector2 Position { get; set; }
    }
    /// <summary>
    /// Définit une propriété pour les dimensions d'un objet.
    /// </summary>
    public interface IDimensions
    {
        public abstract Vector2 Dimensions { get; set; }
    }
    /// <summary>
    /// Définit un gestionnaire de ressources capable d'ajouter, d'obtenir et de supprimer des ressources.
    /// </summary>
    public interface IResource
    {
        public abstract bool AddResource<T>(string resourceName, T resource);
        public abstract T GetResource<T>(string resourceName);
        public abstract void RemoveResource(string resourceName);
    }
    /// <summary>
    /// Définit une propriété pour la couleur d'un objet.
    /// </summary>
    public interface IColor
    {
        public abstract Color Color { get; set; }
    }
    /// <summary>
    /// Définit une propriété pour gérer la visibilité d'un objet.
    /// </summary>
    public interface IVisible
    {
        public abstract bool Visible { get; set; }
    }
    /// <summary>
    /// Définit une méthode pour tester les collisions et obtenir un rectangle de collision.
    /// </summary>
    public interface ICollide : IPosition, IDimensions
    {
        public abstract bool Collide(ICollide item);
        public Rectangle Rectangle { get; }
    }
    /// <summary>
    /// Définit une propriété pour gérer le retournement d'un objet (flip horizontal ou vertical).
    /// </summary>
    public interface IFlip
    {
        public abstract Vector2 Flip { get; set; }
    }
    /// <summary>
    /// Définit une méthode pour vérifier si un objet est cliqué.
    /// </summary>
    public interface IClickable
    {
        public abstract bool IsClicked();
    }
    /// <summary>
    /// Définit une interface pour un objet textuel ayant des dimensions spécifiques.
    /// </summary>
    public interface IText : IPosition, IDimensions
    {
        public abstract Vector2 TextDimensions { get; }
    }
    /// <summary>
    /// Définit une interface pour un écran de chargement avec progression et texte.
    /// </summary>
    public interface ILoadingScreen
    {
        public abstract float Progress { get; set; }
        public abstract string Text { get; set; }
    }
    /// <summary>
    /// Définit une méthode pour effectuer une copie d'un objet.
    /// </summary>
    /// <typeparam name="T">Type de l'objet à copier.</typeparam>
    public interface ICopyable<T>
    {
        public abstract T Copy();
    }
    /// <summary>
    /// Définit une propriété pour verrouiller ou déverrouiller un objet.
    /// </summary>
    public interface ILocked
    {
        public abstract bool Locked { get; set; }
    }
}
#pragma warning restore CS1591 // Pour ne pas devoir avoir un commentaire sur les membre des interfaces