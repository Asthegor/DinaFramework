using Microsoft.Xna.Framework.Input;

namespace DinaFramework.Inputs
{
    /// <summary>
    /// Représente une touche de contrôleur basée sur une touche de clavier.
    /// </summary>
    /// <remarks>
    /// Initialise une nouvelle instance de la classe KeyboardControllerKey avec la touche spécifiée.
    /// </remarks>
    /// <param name="key"></param>
    public class KeyboardControllerKey(Keys key) : ControllerKey
    {
        private readonly KeyboardKey _key = new KeyboardKey(key);
        private KeyboardState _state;

        /// <summary>
        /// Met à jour l'état de la touche en récupérant l'état actuel du clavier.
        /// </summary>
        public override void Update()
        {
            _state = Keyboard.GetState();
            _key.Update(_state);
        }
        /// <summary>
        /// La touche est maintenue appuyée.
        /// </summary>
        public override bool IsDown => _key.IsDown;
        /// <summary>
        /// La touche vient d’être pressée (front montant).
        /// </summary>
        public override bool IsPressed => _key.IsPressed;
        /// <summary>
        /// La touche vient d’être relâchée (front descendant).
        /// </summary>
        public override bool IsReleased => _key.IsReleased;
    }
}
