using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente une ligne graphique configurable avec une position de départ, une position de fin (ou une distance et un angle), une épaisseur et une couleur.
    /// </summary>
    public class Line : IColor, IDraw, ICopyable<Line>
    {
        private readonly float _distance;
        private readonly float _angle;
        private Vector2 _position;
        private Vector2 _origin;
        private Vector2 _scale;
        private Texture2D _texture;
        /// <summary>
        /// Initialise une ligne en utilisant une position de départ et une position de fin.
        /// </summary>
        /// <param name="startposition">La position de départ de la ligne.</param>
        /// <param name="endposition">La position de fin de la ligne.</param>
        /// <param name="color">La couleur de la ligne.</param>
        /// <param name="thickness">L'épaisseur de la ligne (par défaut 1.0f).</param>
        public Line(Vector2 startposition, Vector2 endposition, Color color, float thickness = 1.0f)
        {
            Color = color;
            Thickness = thickness;
            _position = startposition;
            _distance = Vector2.Distance(startposition, endposition);
            _angle = Convert.ToSingle(Math.Atan2(endposition.Y - startposition.Y, endposition.X - startposition.X));
            _origin = new Vector2(0f, 0.5f);
            _scale = new Vector2(_distance, thickness);
        }
        /// <summary>
        /// Initialise une ligne en utilisant une position de départ, une distance et un angle.
        /// </summary>
        /// <param name="position">La position de départ de la ligne.</param>
        /// <param name="distance">La longueur de la ligne.</param>
        /// <param name="angle">L'angle de la ligne en radians.</param>
        /// <param name="color">La couleur de la ligne.</param>
        /// <param name="thickness">L'épaisseur de la ligne (par défaut 1.0f).</param>
        public Line(Vector2 position, float distance, float angle, Color color, float thickness = 1.0f)
        {
            Color = color;
            Thickness = thickness;
            _position = position;
            _distance = distance;
            _angle = angle;
            _origin = new Vector2(0f, 0.5f);
            _scale = new Vector2(_distance, thickness);
        }
        /// <summary>
        /// Obtient ou définit la couleur de la ligne.
        /// </summary>
        public Color Color { get; set; }
        /// <summary>
        /// Obtient ou définit l'épaisseur de la ligne.
        /// </summary>
        public float Thickness { get; set; }

        /// <summary>
        /// Dessine la ligne sur un SpriteBatch.
        /// </summary>
        /// <param name="spritebatch">L'objet SpriteBatch utilisé pour dessiner la ligne.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (_texture == null)
            {
                _texture = new Texture2D(spritebatch.GraphicsDevice, 1, 1);
                _texture.SetData([Color.White]);
            }
            spritebatch.Draw(_texture, _position, null, Color, _angle, _origin, _scale, SpriteEffects.None, 0);
        }
        /// <summary>
        /// Crée une copie de la ligne actuelle.
        /// </summary>
        /// <returns>Une nouvelle instance de la classe Line avec les mêmes paramètres que l'instance actuelle.</returns>
        public Line Copy()
        {
            return new Line()
            {
                _origin = _origin,
                _position = _position,
                _scale = _scale,
                _texture = _texture,
                Color = Color,
                Thickness = Thickness,
            };
        }

        private Line() { }
    }
}
