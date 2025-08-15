using DinaFramework.Interfaces;
using DinaFramework.Internal;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.ComponentModel;

namespace DinaFramework.Scenes
{
    /// <summary>
    /// Scène d’introduction affichant le logo du framework avec un fondu enchaîné.
    /// </summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    internal sealed class FrameworkLogoScene(SceneManager sceneManager) : Scene(sceneManager)
    {
        private const float WAIT_FADE_DELAY = 2f;
        private const float FADE_DELAY = 0.75f;

        private float _timer;
        private bool _fading;

        private Texture2D logo;
        private Rectangle rect;

        private Color color;

        /// <summary>
        /// Charge les ressources nécessaires à l’affichage du logo (image, position, etc.).
        /// </summary>
        public override void Load()
        {
            logo = InternalAssets.Logo(SceneManager.GraphicsDevice);

            (float screenW, float screenH) = SceneManager.ScreenDimensions;

            float maxWidth = screenW * 0.8f;
            float maxHeight = screenH * 0.8f;

            float ratioX = maxWidth / logo.Width;
            float ratioY = maxHeight / logo.Height;
            float scale = MathF.Min(ratioX, ratioY);

            Vector2 logoDim = new Vector2 (logo.Width * scale, logo.Height * scale);
            Vector2 logoPos = (ScreenDimensions - logoDim) / 2;

            rect = new Rectangle(logoPos.ToPoint(), logoDim.ToPoint());
        }
        /// <summary>
        /// Réinitialise les paramètres internes de la scène (timer, état du fondu, couleur).
        /// </summary>
        public override void Reset()
        {
            _timer = 0;
            _fading = false;
            color = Color.White;
        }
        /// <summary>
        /// Met à jour l’état de la scène, lance le fondu après un délai, puis change de scène.
        /// </summary>
        /// <param name="gametime">Temps écoulé depuis la dernière mise à jour.</param>
        public override void Update(GameTime gametime)
        {
            ArgumentNullException.ThrowIfNull(gametime);

            _timer += (float)gametime.ElapsedGameTime.TotalSeconds;

            if (!_fading)
            {
                if (_timer >= WAIT_FADE_DELAY)
                {
                    _timer = 0;
                    _fading = true;
                }
            }
            else
            {
                if (_timer >= FADE_DELAY)
                {
                    SceneManager.ContinueToNextScene();
                    return;
                }
                float ratio = 1 - (_timer / FADE_DELAY);
                color = Color.White * ratio;
            }
        }
        /// <summary>
        /// Dessine le logo à l’écran avec la transparence actuelle.
        /// </summary>
        /// <param name="spritebatch">Objet utilisé pour dessiner les sprites.</param>
        public override void Draw(SpriteBatch spritebatch)
        {
            spritebatch?.Draw(logo, rect, color);
        }

    }
}

