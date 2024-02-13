using DinaFramework.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Globalization;

namespace DinaFramework.Controls
{
    public abstract class ControllerKey
    {
        public string Alias { get; protected set; }

        private static readonly List<ControllerKey> controllers = new List<ControllerKey>();

        public static List<ControllerKey> GetControllers() => controllers;

        public ControllerAction Action { get; set; }

        protected ControllerKey()
        {
            GetControllers().Add(this);
        }
        public static void ResetAllKeys()
        {
            foreach (var controller in GetControllers())
                controller.Reset();
        }
        public abstract void Reset();
        public abstract bool IsPressed();
        public abstract bool IsContinuousPressed();
        public abstract bool IsReleased();
        public new abstract string ToString();
    }
    public class KeyboardKey : ControllerKey
    {
        private Keys Key { get; set; }
        private KeyboardState _oldState = Keyboard.GetState();

        public KeyboardKey(Keys key, ControllerAction action = ControllerAction.Pressed, string alias = "")
        {
            Key = key;
            Action = action != ControllerAction.None ? action : ControllerAction.Pressed;
            if (string.IsNullOrEmpty(alias))
                Alias = "Key_" + key.ToString();
            else
                Alias = char.ToUpper(alias[0], CultureInfo.CurrentCulture) + alias[1..];
            Reset();
        }
        public override void Reset() { _oldState = Keyboard.GetState(); }
        public override bool IsPressed()
        {
            if (Action == ControllerAction.Released)
                return false;
            bool result = Keyboard.GetState().IsKeyDown(Key);
            if (Action == ControllerAction.Pressed)
                result = !_oldState.IsKeyDown(Key) && Keyboard.GetState().IsKeyDown(Key);
            _oldState = Keyboard.GetState();
            return result;
        }
        public override bool IsReleased() => Keyboard.GetState().IsKeyUp(Key);
        public override string ToString() { return Alias; }

        public override bool IsContinuousPressed()
        {
            return Keyboard.GetState().IsKeyDown(Key);
        }
    }
    public class GamepadButton : ControllerKey
    {
        private readonly PlayerIndex _indexplayer;
        private Buttons Button { get; set; }
        private GamePadState _oldState;
        public GamepadButton(Buttons button, ControllerAction action = ControllerAction.Pressed, PlayerIndex index = PlayerIndex.One, string alias = "")
        {
            Button = button;
            Action = action;
            _indexplayer = index;
            if (string.IsNullOrEmpty(alias))
                Alias = "Button_" + button.ToString();
            else
                Alias = char.ToUpper(alias[0], CultureInfo.CurrentCulture) + alias[1..];
            Reset();
        }
        public override void Reset() { _oldState = GamePad.GetState(_indexplayer); }
        public override bool IsPressed()
        {
            if (Action == ControllerAction.Released)
                return false;
            bool result = GamePad.GetState(_indexplayer).IsButtonDown(Button);
            if (Action == ControllerAction.Pressed)
                result = !_oldState.IsButtonDown(Button) && GamePad.GetState(_indexplayer).IsButtonDown(Button);
            _oldState = GamePad.GetState(_indexplayer);
            return result;
        }
        public override bool IsReleased() => GamePad.GetState(_indexplayer).IsButtonUp(Button);
        public override string ToString() { return Alias; }

        public override bool IsContinuousPressed()
        {
            return GamePad.GetState(_indexplayer).IsButtonDown(Button);
        }
    }
}
