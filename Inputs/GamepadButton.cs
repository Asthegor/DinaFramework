using Microsoft.Xna.Framework.Input;

namespace DinaFramework.Inputs
{
    /// <summary>
    /// Classe représentant un bouton de manette de jeu.
    /// </summary>
    public class GamepadButton
    {
        /// <summary>
        /// Bouton de la manette.
        /// </summary>
        public Buttons Button { get; }
        private bool _isDown;
        private bool _wasDown;
        /// <summary>
        /// Crée une nouvelle instance de la classe GamepadButton pour le bouton spécifié.
        /// </summary>
        /// <param name="button"></param>
        public GamepadButton(Buttons button)
        {
            Button = button;
        }

        /// <summary>
        /// Effectue la mise à jour de l'état du bouton en fonction de l'état actuel de la manette.
        /// </summary>
        /// <param name="state"></param>
        public void Update(GamePadState state)
        {
            _wasDown = _isDown;
            _isDown = state.IsButtonDown(Button);
        }
        /// <summary>
        /// Le bouton est maintenu appuyé.
        /// </summary>
        public bool IsDown => _isDown;
        /// <summary>
        /// Le bouton vient d’être pressé (front montant).
        /// </summary>
        public bool IsPressed => _isDown && !_wasDown;
        /// <summary>
        /// Le bouton vient d’être relaché (front descendant).
        /// </summary>
        public bool IsReleased => !_isDown && _wasDown;
    }
}
