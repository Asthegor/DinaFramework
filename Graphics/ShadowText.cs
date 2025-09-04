using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente un texte avec une ombre, permettant de gérer la couleur, la position, l'alignement et les effets de temporisation.
    /// </summary>
    public class ShadowText : Base, IUpdate, IDraw, IColor, IVisible, IText, ICopyable<ShadowText>, IDrawingElement
    {
        private readonly Text _text;
        private readonly Text _shadow;
        Vector2 _offset;
        /// <summary>
        /// Initialise une nouvelle instance de la classe ShadowText avec les paramètres spécifiés.
        /// </summary>
        /// <param name="font">La police de caractères à utiliser pour le texte.</param>
        /// <param name="content">Le contenu du texte.</param>
        /// <param name="color">La couleur du texte.</param>
        /// <param name="position">La position du texte.</param>
        /// <param name="shadowcolor">La couleur de l'ombre du texte.</param>
        /// <param name="offset">L'offset de l'ombre par rapport au texte.</param>
        /// <param name="halign">L'alignement horizontal du texte. Par défaut, HorizontalAlignment.Center.</param>
        /// <param name="valign">L'alignement vertical du texte. Par défaut, VerticalAlignment.Middle.</param>
        /// <param name="zorder">L'ordre de superposition du texte. Par défaut, 0.</param>
        public ShadowText(SpriteFont font, string content, Color color, Vector2 position, Color shadowcolor, Vector2 offset,
                          HorizontalAlignment halign = default, VerticalAlignment valign = default, int zorder = 0)
        {
            ArgumentNullException.ThrowIfNull(font);

            _text = new Text(font, content, color, position, halign, valign, zorder);
            _shadow = new Text(font, content, shadowcolor, position + offset, halign, valign, zorder - 1);
            Offset = offset;
        }
        /// <summary>
        /// Obtient ou définit la couleur du texte.
        /// </summary>
        public Color Color
        {
            get { return _text.Color; }
            set { _text.Color = value; }
        }
        /// <summary>
        /// Obtient ou définit la couleur de l'ombre du texte.
        /// </summary>
        public Color ShadowColor
        {
            get { return _shadow.Color; }
            set { _shadow.Color = value; }
        }
        /// <summary>
        /// Définit les minuteries pour le texte et son ombre.
        /// </summary>
        /// <param name="waitTime">Le temps d'attente avant d'afficher le texte.</param>
        /// <param name="displayTime">Le temps d'affichage du texte.</param>
        /// <param name="nbLoops">Le nombre de boucles pour l'affichage du texte.</param>
        public void SetTimers(float waitTime = -1.0f, float displayTime = -1.0f, int nbLoops = -1)
        {
            _shadow.SetTimers(waitTime, displayTime, nbLoops);
            _text.SetTimers(waitTime, displayTime, nbLoops);
        }
        /// <summary>
        /// Obtient ou définit le contenu du texte.
        /// </summary>
        public string Content
        {
            get { return _text.Content; }
            set
            {
                _shadow.Content = value;
                _text.Content = value;
            }
        }
        /// <summary>
        /// Met à jour l'état du texte et de son ombre.
        /// </summary>
        /// <param name="gametime">Le temps écoulé depuis le dernier appel.</param>
        public void Update(GameTime gametime)
        {
            _shadow.Update(gametime);
            _text.Update(gametime);
        }
        /// <summary>
        /// Dessine le texte et son ombre sur l'écran.
        /// </summary>
        /// <param name="spritebatch">Le SpriteBatch utilisé pour dessiner le texte.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            _shadow.Draw(spritebatch);
            _text.Draw(spritebatch);
        }
        /// <summary>
        /// Obtient ou définit la position du texte et de son ombre.
        /// </summary>
        public override Vector2 Position
        {
            get { return _text.Position; }
            set
            {
                if (_text != null)
                    _text.Position = value;
                if (_shadow != null)
                    _shadow.Position = value + Offset;
            }
        }
        /// <summary>
        /// Obtient ou définit les dimensions du texte et de son ombre.
        /// </summary>
        public override Vector2 Dimensions
        {
            get { return _text.Dimensions + Offset; }
            set
            {
                if (_text != null)
                    _text.Dimensions = value;
                if (_shadow != null)
                    _shadow.Dimensions = value;
            }
        }
        /// <summary>
        /// Obtient ou définit l'offset de l'ombre par rapport au texte.
        /// </summary>
        public Vector2 Offset
        {
            get { return _offset; }
            set { _offset = value; }
        }
        /// <summary>
        /// Obtient ou définit l'ordre de superposition du texte et de son ombre.
        /// </summary>
        public new int ZOrder
        {
            get { return _text.ZOrder; }
            set
            {
                _text.ZOrder = value;
                _shadow.ZOrder = value - 1;
            }
        }
        /// <summary>
        /// Obtient ou définit la visibilité du texte et de son ombre.
        /// </summary>
        public bool Visible
        {
            get { return _text.Visible; }
            set
            {
                _text.Visible = value;
                _shadow.Visible = value;
            }
        }
        /// <summary>
        /// Obtient les dimensions du texte sans prendre en compte l'ombre.
        /// </summary>
        public Vector2 TextDimensions => _text.TextDimensions;

        /// <summary>
        /// La police du texte avec une ombre.
        /// </summary>
        public SpriteFont Font
        {
            get => _text.Font;
            set
            {
                _text.Font = value;
                _shadow.Font = value;
            }
        }

        /// <summary>
        /// Crée une copie de l'objet ShadowText avec les mêmes valeurs.
        /// </summary>
        /// <returns>Une nouvelle instance de ShadowText.</returns>
        public ShadowText Copy()
        {
            return new ShadowText(_text.Font, Content, Color, Position, ShadowColor, Offset, zorder: ZOrder)
            {
                Dimensions = Dimensions,
                Visible = Visible,
            };
        }
    }
}
