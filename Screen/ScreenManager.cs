using DinaFramework.Events;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace DinaFramework.Screen
{
    /// <summary>
    /// Gère les paramètres d'affichage du jeu, tels que le mode plein écran et la résolution d'écran, via le GraphicsDeviceManager.
    /// </summary>
    /// <remarks>
    /// Cette classe centralise la gestion de l'affichage pour éviter de disperser la logique dans plusieurs composants.
    /// Elle est enregistrée dans le ServiceLocator pour être facilement accessible dans tout le projet.
    /// </remarks>
    public class ScreenManager
    {
        private readonly GraphicsDeviceManager _graphics;
        private readonly GameWindow _window;

        /// <summary>
        /// Initialise une nouvelle instance de la classe ScreenManager et l'enregistre éventuellement dans le ServiceLocator.
        /// </summary>
        /// <param name="graphics"> L'instance de GraphicsDeviceManager utilisée pour modifier les paramètres d'affichage.</param>
        /// <param name="window"></param>
        private ScreenManager(GraphicsDeviceManager graphics, GameWindow window)
        {
            _graphics = graphics ?? throw new ArgumentNullException(nameof(graphics));
            _window = window ?? throw new ArgumentNullException(nameof(window));
            _window.ClientSizeChanged += HandleResize!;
        }

        /// <summary>
        /// Permet de récupérer la liste des résolutions de la carte graphique.
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<DisplayMode> AvailableResolutions => GraphicsAdapter.DefaultAdapter.SupportedDisplayModes;

        /// <summary>
        /// Permet de savoir si l'affiche est en mode plein écran (true) ou fenêtré (false).
        /// </summary>
        public bool IsFullScreen => _graphics.IsFullScreen;

        /// <summary>
        /// Retourne la résolution actuelle.
        /// </summary>
        public Point CurrentResolution => new Point(_graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight);


        /// <summary>
        /// Actions lors du changement de résolution.
        /// </summary>
        public event EventHandler<ScreenManagerEventArgs>? OnResolutionChanged;

        /// <summary>
        /// Définit la résolution d'affichage du jeu.
        /// </summary>
        /// <param name="width">Largeur de l'écran en pixels.</param>
        /// <param name="height">Hauteur de l'écran en pixels.</param>
        public void SetResolution(int width, int height)
        {
            _graphics.PreferredBackBufferWidth = width;
            _graphics.PreferredBackBufferHeight = height;
            _graphics.ApplyChanges();
            OnResolutionChanged?.Invoke(this, new ScreenManagerEventArgs(this));
        }

        /// <summary>
        /// Définit le mode plein écran (true) ou non (false).
        /// </summary>
        /// <param name="isChecked"></param>
        public void SetFullScreen(bool isChecked)
        {
            _graphics.IsFullScreen = isChecked;
            _graphics.ApplyChanges();
        }

        /// <summary>
        /// Crée une instance de ScreenManager et l'enregistre dans le ServiceLocator.
        /// </summary>
        /// <param name="graphics"></param>
        /// <param name="window"></param>
        public static void Initialize(GraphicsDeviceManager graphics, GameWindow window)
        {
            ServiceLocator.Register(ServiceKeys.ScreenManager, new ScreenManager(graphics, window));
        }

        private void HandleResize(object sender, EventArgs e)
        {
            int newWidth = _window.ClientBounds.Width;
            int newHeight = _window.ClientBounds.Height;

            if (newWidth != _graphics.PreferredBackBufferWidth ||
                newHeight != _graphics.PreferredBackBufferHeight)
            {
                SetResolution(newWidth, newHeight);
            }
        }
    }

}
