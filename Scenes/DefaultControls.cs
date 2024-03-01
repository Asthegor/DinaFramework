using DinaFramework.Controls;
using DinaFramework.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

namespace DinaFramework.Scenes
{
    public static class DefaultControls
    {
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
