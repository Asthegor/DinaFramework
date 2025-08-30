using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DinaFramework.Inputs
{
    /// <summary>
    /// Représente un bouton de manette pour un joueur spécifique.
    /// <para>
    /// Cette classe encapsule un <see cref="GamepadButton"/> et gère l'état du bouton entre deux mises à jour (pressé, relâché, maintenu).
    /// </para>
    /// <para>
    /// Elle est conçue pour être utilisée avec un <see cref="PlayerController"/> ou un <see cref="InputManager"/> pour vérifier les actions de jeu abstraites
    /// (par exemple "Validate", "Cancel").
    /// </para>
    /// </summary>
    public class GamepadControllerKey : ControllerKey
    {
        private readonly GamepadButton _button;
        private readonly PlayerIndex _player;
        private GamePadState _state;
        /// <summary>
        /// Crée une instance de GamepadControllerKey pour une touche spécifique d'une manette de jeu.
        /// </summary>
        /// <param name="button"></param>
        /// <param name="player"></param>
        public GamepadControllerKey(Buttons button, PlayerIndex player = PlayerIndex.One)
        {
            _button = new GamepadButton(button);
            _player = player;
        }
        /// <summary>
        /// Met à jour l'état de la touche de la manette.
        /// </summary>
        public override void Update()
        {
            _state = GamePad.GetState(_player);
            _button.Update(_state);
        }
        /// <summary>
        /// Indique si le bouton est actuellement maintenu enfoncé.
        /// </summary>
        public override bool IsDown => _button.IsDown;
        /// <summary>
        /// Indique si le bouton vient d'être pressé depuis la dernière mise à jour.
        /// </summary>
        public override bool IsPressed => _button.IsPressed;
        /// <summary>
        /// Indique si le bouton vient d'être relâché depuis la dernière mise à jour.
        /// </summary>
        public override bool IsReleased => _button.IsReleased;
    }
}
