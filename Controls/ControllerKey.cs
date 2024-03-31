using DinaFramework.Enums;
using DinaFramework.Scenes;

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
        public abstract float IsPressed();
        public abstract float IsContinuousPressed();
        public abstract bool IsReleased();
        public new abstract string ToString();
    }
    public class KeyboardKey : ControllerKey
    {
        private Keys Key { get; set; }
        private KeyboardState _oldState;

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
        public override float IsPressed()
        {
            if (Action == ControllerAction.Released)
                return 0;
            KeyboardState ks = Keyboard.GetState();
            float result = ks.IsKeyDown(Key) ? 1 : 0;
            if (Action == ControllerAction.Pressed)
                result = !_oldState.IsKeyDown(Key) && ks.IsKeyDown(Key) ? 1 : 0;
            _oldState = ks;
            if (result != 0)
                SceneManager.GetInstance().IsMouseVisible = true;
            return result;
        }
        public override bool IsReleased() => Keyboard.GetState().IsKeyUp(Key);
        public override string ToString() { return Alias; }

        public override float IsContinuousPressed()
        {
            return Keyboard.GetState().IsKeyDown(Key) ? 1 : 0;
        }
    }
    public class GamepadButton : ControllerKey
    {

        private readonly PlayerIndex _indexplayer;
        private readonly float _axisThreshold;
        private readonly Buttons _button;
        //private Buttons Key { get; set; }
        private GamePadState _oldState;
        public GamepadButton(Buttons button, ControllerAction action = ControllerAction.Pressed, PlayerIndex index = PlayerIndex.One, string alias = "", float axisThreshold = 0.5f)
        {
            _button = button;
            Action = action;
            _indexplayer = index;
            _axisThreshold = axisThreshold;
            if (string.IsNullOrEmpty(alias))
                Alias = "Button_" + button.ToString();
            else
                Alias = char.ToUpper(alias[0], CultureInfo.CurrentCulture) + alias[1..];
            Reset();
        }
        public override void Reset() { _oldState = GamePad.GetState(_indexplayer); }
        public override float IsPressed()
        {
            if (Action == ControllerAction.Released)
                return 0;
            GamePadState gs = GamePad.GetState(_indexplayer);
            float result = gs.IsButtonDown(_button) ? 1 : 0;
            if (Action == ControllerAction.Pressed)
            {
                result = (_oldState.IsButtonUp(_button) && gs.IsButtonDown(_button)) ? 1 : 0;
                if (_button == Buttons.LeftThumbstickLeft && gs.ThumbSticks.Left.X < -_axisThreshold && _oldState.ThumbSticks.Left.X >= -_axisThreshold)
                {
                    result = gs.ThumbSticks.Left.X;
                }
                else if (_button == Buttons.LeftThumbstickRight && gs.ThumbSticks.Left.X > _axisThreshold && _oldState.ThumbSticks.Left.X <= _axisThreshold)
                {
                    result = gs.ThumbSticks.Left.X;
                }
            }
            _oldState = gs;
            if (result != 0)
                SceneManager.GetInstance().IsMouseVisible = false;
            return result;
        }
        public override bool IsReleased() => GamePad.GetState(_indexplayer).IsButtonUp(_button);
        public override string ToString() { return Alias; }

        public override float IsContinuousPressed()
        {
            return GamePad.GetState(_indexplayer).IsButtonDown(_button) ? 1 : 0;
        }
    }
}
