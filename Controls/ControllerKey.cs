using DinaFramework.Enums;
using DinaFramework.Scenes;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;

using System.Collections.Generic;
using System.Globalization;

namespace DinaFramework.Controls
{
    /// <summary>
    /// Classe abstraite représentant une touche de contrôleur. Utilisez l'une des classes dérivées : KeyboardKey, GamepadButton.
    /// </summary>
    public abstract class ControllerKey
    {
        /// <summary>
        /// Alias de la touche de contrôleur.
        /// </summary>
        public string Alias { get; protected set; }
        /// <summary>
        /// Action associée à la touche de contrôleur.
        /// </summary>
        public ControllerAction Action { get; set; }


        private static readonly List<ControllerKey> _controllers = [];

        /// <summary>
        /// Retourne la liste de tous les contrôleurs.
        /// </summary>
        /// <returns>La liste de tous les contrôleurs.</returns>
        public static List<ControllerKey> GetControllers() => _controllers;

        /// <summary>
        /// </summary>
        protected ControllerKey()
        {
            GetControllers().Add(this);
        }
        /// <summary>
        /// Réinitialise toutes les touches de contrôleur.
        /// </summary>
        public static void ResetAllKeys()
        {
            foreach (var controller in GetControllers())
                controller.Reset();
        }
        /// <summary>
        /// Réinitialise la touche de contrôleur.
        /// </summary>
        public abstract void Reset();
        /// <summary>
        /// Vérifie si la touche est pressée.
        /// </summary>
        /// <returns>Une valeur entre 0 et 1.</returns>
        public abstract float IsPressed();
        /// <summary>
        /// Vérifie si la touche est pressée de manière continue.
        /// </summary>
        /// <returns>Une valeur entre 0 et 1.</returns>
        public abstract float IsContinuousPressed();
        /// <summary>
        /// Vérifie si la touche est relâchée.
        /// </summary>
        /// <returns>True si la touche est relàchée, sinon false.</returns>
        public abstract bool IsReleased();
        /// <summary>
        /// Retourne le SceneManager s'il est enregistré dans le ServiceLocator, sinon retourne null.
        /// </summary>
        public static SceneManager SceneManager => ServiceLocator.Exists(ServiceKey.SceneManager) ? ServiceLocator.Get<SceneManager>(ServiceKey.SceneManager) : null;
    }
    /// <summary>
    /// Classe représentant une touche de clavier.
    /// </summary>
    public class KeyboardKey : ControllerKey
    {
        private Keys Key { get; set; }
        private KeyboardState _oldState;

        /// <summary>
        /// Initialise une nouvelle instance de la classe KeyboardKey.
        /// </summary>
        /// <param name="key">La touche du clavier.</param>
        /// <param name="action">L'action associée à la touche (par défaut ControllerAction.Pressed).</param>
        /// <param name="alias">L'alias de la touche (par défaut une chaîne vide).</param>
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
        /// <summary>
        /// Réinitialise la touche de clavier.
        /// </summary>
        public override void Reset() { _oldState = Keyboard.GetState(); }
        /// <summary>
        /// Vérifie si la touche de clavier est pressée.
        /// </summary>
        /// <returns>1 si la touche est pressée, sinon 0.</returns>
        public override float IsPressed()
        {
            if (Action == ControllerAction.Released)
                return 0;
            KeyboardState ks = Keyboard.GetState();
            float result = ks.IsKeyDown(Key) ? 1 : 0;
            if (Action == ControllerAction.Pressed)
                result = !_oldState.IsKeyDown(Key) && ks.IsKeyDown(Key) ? 1 : 0;
            _oldState = ks;
            if (result != 0 && SceneManager != null)
                SceneManager.IsMouseVisible = true;
            return result;
        }
        /// <summary>
        /// Vérifie si la touche de clavier est relâchée.
        /// </summary>
        /// <returns>True si la touche est relâchée, sinon false.</returns>
        public override bool IsReleased() => Keyboard.GetState().IsKeyUp(Key);

        /// <summary>
        /// Vérifie si la touche de clavier est pressée de manière continue.
        /// </summary>
        /// <returns>1 si la touche est maintenue appuyée, sinon 0.</returns>
        public override float IsContinuousPressed()
        {
            return Keyboard.GetState().IsKeyDown(Key) ? 1 : 0;
        }
    }
    /// <summary>
    /// Classe représentant un bouton de manette de jeu.
    /// </summary>
    public class GamepadButton : ControllerKey
    {

        private readonly PlayerIndex _indexplayer;
        private readonly float _axisThreshold;
        private readonly Buttons _button;
        //private Buttons Key { get; set; }
        private GamePadState _oldState;

        /// <summary>
        /// Initialise une nouvelle instance de la classe GamepadButton.
        /// </summary>
        /// <param name="button">Le bouton de la manette.</param>
        /// <param name="action">L'action associée au bouton (par défaut ControllerAction.Pressed).</param>
        /// <param name="index">L'index du joueur (par défaut PlayerIndex.One).</param>
        /// <param name="alias">L'alias du bouton (par défaut une chaîne vide).</param>
        /// <param name="axisThreshold">Le seuil de l'axe (par défaut 0.5f).</param>
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
        /// <summary>
        /// Réinitialise le bouton de manette.
        /// </summary>
        public override void Reset() { _oldState = GamePad.GetState(_indexplayer); }
        /// <summary>
        /// Vérifie si le bouton de manette est pressé.
        /// </summary>
        /// <returns>1 si un bouton est appuyé ou une valeur entre 0 et 1 pour les sticks de la manette, sinon 0.</returns>
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
            if (result != 0 && SceneManager != null)
                SceneManager.IsMouseVisible = false;
            return result;
        }
        /// <summary>
        /// Vérifie si le bouton de manette est relâché.
        /// </summary>
        /// <returns>True si le bouton est relâché, sinon false.</returns>
        public override bool IsReleased() => GamePad.GetState(_indexplayer).IsButtonUp(_button);

        /// <summary>
        /// Vérifie si le bouton de manette est pressé de manière continue.
        /// </summary>
        /// <returns>1 si le bouton est pressé en continue, sinon 0.</returns>
        public override float IsContinuousPressed()
        {
            return GamePad.GetState(_indexplayer).IsButtonDown(_button) ? 1 : 0;
        }

    }
}
