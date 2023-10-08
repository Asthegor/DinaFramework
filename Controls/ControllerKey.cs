using DinaFramework.Enums;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;

namespace DinaFramework.Controls
{
    public abstract class ControllerKey
    {
         ControllerAction _action;
        static List<ControllerKey> _controllers = new List<ControllerKey>();
        public static List<ControllerKey> Controllers
        {
            get { return _controllers; }
            protected set { _controllers = value; }
        }
        public ControllerAction Action
        {
            get { return _action; }
            set { _action = value; }
        }

        protected ControllerKey()
        { 
            Controllers.Add(this);
        }
        public static void ResetAllKeys()
        {
            foreach (var controller in Controllers)
                controller.Reset();
        }
        public abstract void Reset();
        public abstract bool IsPressed();
        public abstract bool IsReleased();
        public new abstract string ToString();
    }
    public class KeyboardKey : ControllerKey
    {
        Keys Key { get; set; }
        KeyboardState oldState = Keyboard.GetState();
        
        public KeyboardKey(Keys key, ControllerAction action = ControllerAction.Pressed)
        {
            Key = key;
            Action = action;
            Reset();
        }
        public override void Reset() { oldState = Keyboard.GetState(); }
        public override bool IsPressed()
        {
            if (Action == ControllerAction.Released)
                return false;
            bool result = Keyboard.GetState().IsKeyDown(Key);
            if (Action == ControllerAction.Pressed)
                result = !oldState.IsKeyDown(Key) && Keyboard.GetState().IsKeyDown(Key);
            oldState = Keyboard.GetState();
            return result;
        }
        public override bool IsReleased() => Keyboard.GetState().IsKeyUp(Key);
        public override string ToString() => Key.ToString();
    }
    public class GamepadButton : ControllerKey
    {
        readonly PlayerIndex _indexplayer;
        Buttons Button { get; set; }
        GamePadState oldState;
        public GamepadButton(Buttons button, ControllerAction action = ControllerAction.Pressed, PlayerIndex index = PlayerIndex.One)
        {
            Button = button;
            Action = action;
            _indexplayer = index;
            Reset();
        }
        public override void Reset() { oldState = GamePad.GetState(_indexplayer); }
        public override bool IsPressed()
        {
            if (Action == ControllerAction.Released)
                return false;
            if (Button == Buttons.LeftThumbstickLeft || Button == Buttons.LeftThumbstickUp || Button == Buttons.LeftThumbstickRight || Button == Buttons.LeftThumbstickDown ||
                Button == Buttons.RightThumbstickLeft || Button == Buttons.RightThumbstickUp || Button == Buttons.RightThumbstickRight || Button == Buttons.RightThumbstickDown)
            {

            }
            bool result = GamePad.GetState(_indexplayer).IsButtonDown(Button);
            if(Action == ControllerAction.Pressed)
                result = !oldState.IsButtonDown(Button) && GamePad.GetState(_indexplayer).IsButtonDown(Button);
            oldState = GamePad.GetState(_indexplayer);
            return result;
        }
        public override bool IsReleased() => GamePad.GetState(_indexplayer).IsButtonUp(Button);
        public override string ToString() { return Button.ToString(); }
    }
}
