using DinaFramework.Controls;
using DinaFramework.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DinaFramework.Scenes
{
    /// <summary>
    /// Contient les contrôles par défaut pour les joueurs, définissant les entrées pour le clavier et la manette de jeu.
    /// </summary>
    /// <remarks>
    /// Cette classe fournit deux configurations prédéfinies pour les contrôleurs :
    /// - DefaultKeyboard : Contrôleur pour le clavier avec les touches directionnelles, Pause, Activer et Annuler.
    /// - DefaultGamepad : Contrôleur pour la manette de jeu avec les boutons de direction, Pause, Activer et Annuler.
    /// </remarks>
    public static class DefaultControls
    {
        /// <summary>
        /// Touches du clavier par défaut :
        /// - Up
        /// - Down
        /// - Right
        /// - Left
        /// - RightShift (pause)
        /// - Enter (activate)
        /// - Escape (cancel)
        /// </summary>
        public readonly static PlayerController DefaultKeyboard = new PlayerController
        (
            ControllerType.Keyboard,
            PlayerIndex.One,
            new KeyboardKey(Keys.Up,alias:"up"),
            new KeyboardKey(Keys.Down, alias:"down"),
            new KeyboardKey(Keys.Left, alias:"left"),
            new KeyboardKey(Keys.Right,alias:"right"),
            new KeyboardKey(Keys.RightShift, default, "Pause"),
            new KeyboardKey(Keys.Enter, default, "Activate"),
            new KeyboardKey(Keys.Escape, default, "Cancel")
        );

        /// <summary>
        /// Boutons du gamepad par défaut :
        /// - LeftStick Up
        /// - LeftStick Down
        /// - LeftStick Right
        /// - LeftStick Left
        /// - Start (pause)
        /// - A (activate)
        /// - B (cancel)
        /// </summary>
        public readonly static PlayerController DefaultGamepad = new PlayerController
        (
            ControllerType.Gamepad,
            PlayerIndex.One,
            new GamepadButton(Buttons.LeftStick, default, PlayerIndex.One, "up"),
            new GamepadButton(Buttons.LeftStick, default, PlayerIndex.One, "down"),
            new GamepadButton(Buttons.LeftStick, default, PlayerIndex.One, "left"),
            new GamepadButton(Buttons.LeftStick, default, PlayerIndex.One, "right"),
            new GamepadButton(Buttons.Start, default, default, "Pause"),
            new GamepadButton(Buttons.A, default, default, "Activate"),
            new GamepadButton(Buttons.B, default, default, "Cancel")
        );
    }
}
