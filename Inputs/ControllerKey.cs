namespace DinaFramework.Inputs
{
    /// <summary>
    /// Classe abstraite représentant une touche de contrôleur. Utilisez l'une des classes dérivées : KeyboardKey, GamepadButton.
    /// </summary>
    public abstract class ControllerKey
    {
        /// <summary>
        /// La touche ou le bouton est appuyé.
        /// </summary>
        public abstract bool IsDown { get; }
        /// <summary>
        /// La touche ou le bouton vient d’être pressé (front montant).
        /// </summary>
        public abstract bool IsPressed { get; }
        /// <summary>
        /// La touche ou le bouton vient d’être relâché (front descendant).
        /// </summary>
        public abstract bool IsReleased { get; }
        /// <summary>
        /// Effectue la mise à jour de l'état de la touche ou du bouton.
        /// </summary>
        public abstract void Update();
    }



}
