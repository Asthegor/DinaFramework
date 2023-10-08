using DinaFramework.Enums;

using Microsoft.Xna.Framework;

namespace DinaFramework.Controls
{
    public class PlayerController
    {
        public ControllerType controller = ControllerType.Keyboard;
        public PlayerIndex index = PlayerIndex.One;
        public ControllerKey up = null;
        public ControllerKey down = null;
        public ControllerKey left = null;
        public ControllerKey right = null;
        public ControllerKey pause = null;
        public ControllerKey validate = null;
        public ControllerKey cancel = null;
    }
}
