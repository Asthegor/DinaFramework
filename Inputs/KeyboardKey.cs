using Microsoft.Xna.Framework.Input;

namespace DinaFramework.Inputs
{
    /// <summary>
    /// Classe représentant une touche du clavier.
    /// </summary>
    public class KeyboardKey
    {
        /// <summary>
        /// Touche du clavier.
        /// </summary>
        public Keys Key { get; }
        private bool _isDown;
        private bool _wasDown;
        /// <summary>
        /// Crée une nouvelle instance de la classe KeyboardKey pour la touche spécifiée.
        /// </summary>
        /// <param name="key"></param>
        public KeyboardKey(Keys key)
        {
            Key = key;
        }
        /// <summary>
        /// Effectue la mise à jour de l'état de la touche en fonction de l'état actuel du clavier.
        /// </summary>
        /// <param name="state"></param>

        public void Update(KeyboardState state)
        {
            _wasDown = _isDown;
            _isDown = state.IsKeyDown(Key);
        }

        /// <summary>
        /// La touche est maintenue appuyée.
        /// </summary>
        public bool IsDown => _isDown;

        /// <summary>
        /// La touche vient d’être pressée (front montant).
        /// </summary>
        public bool IsPressed => _isDown && !_wasDown;

        /// <summary>
        /// La touche vient d’être relâchée (front descendant).
        /// </summary>
        public bool IsReleased => !_isDown && _wasDown;
    }
}
