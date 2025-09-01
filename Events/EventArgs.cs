using DinaFramework.Graphics;
using DinaFramework.Interfaces;
using DinaFramework.Menus;
using DinaFramework.Scenes;
using DinaFramework.Screen;

using System;

namespace DinaFramework.Events
{
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Button"/>.
    /// </summary>
    public class ButtonEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// Le bouton qui a déclenché l'événement.
        /// </summary>
        public Button Button { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ButtonEventArgs"/> pour le bouton spécifié.
        /// </summary>
        /// <param name="button">Le bouton associé à l'événement.</param>
        public ButtonEventArgs(Button button) => Button = button;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="CheckBox"/>.
    /// </summary>
    public class CheckBoxEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// La case à cocher qui a déclenché l'événement.
        /// </summary>
        public CheckBox CheckBox { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="CheckBoxEventArgs"/> pour la CheckBox spécifiée.
        /// </summary>
        /// <param name="checkBox">La CheckBox associée à l'événement.</param>
        public CheckBoxEventArgs(CheckBox checkBox) => CheckBox = checkBox;
    }
    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Graphics.Text"/>.
    /// </summary>
    public class TextEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// Le Text qui a déclenché l'événement.
        /// </summary>
        public Text Text { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="TextEventArgs"/> pour le Text spécifié.
        /// </summary>
        /// <param name="text">Le Text associé à l'événement.</param>
        public TextEventArgs(Text text) => Text = text;
    }
    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="ListBox"/>.
    /// </summary>
    public class ListBoxEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// La liste qui a déclenché l'événement.
        /// </summary>
        public ListBox ListBox { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ListBoxEventArgs"/> pour la ListBox spécifiée.
        /// </summary>
        /// <param name="listBox">La ListBox associée à l'événement.</param>
        public ListBoxEventArgs(ListBox listBox) => ListBox = listBox;
    }

    /// <summary>
    /// Contient les informations d'un clic sur un élément d'une <see cref="ListBox"/>.
    /// </summary>
    public class ListBoxClickEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// L'index de l'élément cliqué dans la liste.
        /// </summary>
        public int Index { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ListBoxClickEventArgs"/> pour l'index spécifié.
        /// </summary>
        /// <param name="index">L'index de l'élément cliqué.</param>
        public ListBoxClickEventArgs(int index) => Index = index;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="MenuItem"/>.
    /// </summary>
    public class MenuItemEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// Le MenuItem qui a déclenché l'événement.
        /// </summary>
        public MenuItem MenuItem { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="MenuItemEventArgs"/> pour le MenuItem spécifié.
        /// </summary>
        /// <param name="menuitem">Le MenuItem associé à l'événement.</param>
        public MenuItemEventArgs(MenuItem menuitem) => MenuItem = menuitem;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Panel"/>.
    /// </summary>
    public class PanelEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// Le panneau qui a déclenché l'événement.
        /// </summary>
        public Panel Panel { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="PanelEventArgs"/> pour le panneau spécifié.
        /// </summary>
        /// <param name="panel">Le panneau associé à l'événement.</param>
        public PanelEventArgs(Panel panel) => Panel = panel;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à un <see cref="Slider"/>.
    /// </summary>
    public class SliderEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// Le slider qui a déclenché l'événement.
        /// </summary>
        public Slider Slider { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SliderEventArgs"/> pour le slider spécifié.
        /// </summary>
        /// <param name="slider">Le slider associé à l'événement.</param>
        public SliderEventArgs(Slider slider) => Slider = slider;
    }

    /// <summary>
    /// Contient les informations d'un changement de valeur d'un <see cref="Slider"/>.
    /// </summary>
    public class SliderValueEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// La nouvelle valeur du slider.
        /// </summary>
        public float Value { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SliderValueEventArgs"/> avec la valeur spécifiée.
        /// </summary>
        /// <param name="value">La valeur du slider.</param>
        public SliderValueEventArgs(float value) => Value = value;
    }

    /// <summary>
    /// Contient les informations d'un événement lié à une <see cref="Scene"/>.
    /// </summary>
    public class SceneEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// La scène qui a déclenché l'événement.
        /// </summary>
        public Scene Scene { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SceneEventArgs"/> pour la scène spécifiée.
        /// </summary>
        /// <param name="scene">La scène associée à l'événement.</param>
        public SceneEventArgs(Scene scene) => Scene = scene;
    }

    /// <summary>
    /// Contient les informations d'un événement lié au <see cref="SceneManager"/>.
    /// </summary>
    public class SceneManagerEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// Le SceneManager qui a déclenché l'événement.
        /// </summary>
        public SceneManager SceneManager { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="SceneManagerEventArgs"/> pour le SceneManager spécifié.
        /// </summary>
        /// <param name="sceneManager">Le SceneManager associé à l'événement.</param>
        public SceneManagerEventArgs(SceneManager sceneManager) => SceneManager = sceneManager;
    }

    /// <summary>
    /// Contient les informations d'un événement lié au <see cref="ScreenManager"/>.
    /// </summary>
    public class ScreenManagerEventArgs : EventArgs, IEventArgs
    {
        /// <summary>
        /// Le ScreenManager qui a déclenché l'événement.
        /// </summary>
        public ScreenManager ScreenManager { get; }

        /// <summary>
        /// Initialise une nouvelle instance de <see cref="ScreenManagerEventArgs"/> pour le ScreenManager spécifié.
        /// </summary>
        /// <param name="screenManager">Le ScreenManager associé à l'événement.</param>
        public ScreenManagerEventArgs(ScreenManager screenManager) => ScreenManager = screenManager;
    }
}
