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
    /// <remarks>
    /// Crée une instance de GamepadControllerKey pour une touche spécifique d'une manette de jeu.
    /// </remarks>
    /// <param name="button"></param>
    /// <param name="player"></param>
    public class GamepadControllerKey(Buttons button, PlayerIndex player = PlayerIndex.One) : ControllerKey
    {
        private readonly GamepadButton _button = new GamepadButton(button);
        private readonly PlayerIndex _player = player;
        private GamePadState _state;

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
