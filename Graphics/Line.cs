using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;

namespace DinaFramework.Graphics
{
    public class Line : IColor, IDraw, ICopyable<Line>
    {
        private readonly float _distance;
        private readonly float _angle;
        private Vector2 _position;
        private Vector2 _origin;
        private Vector2 _scale;
        private Texture2D _texture;
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
        public Color Color { get; set; }
        public float Thickness { get; set; }

        public void Draw(SpriteBatch spritebatch)
        {
            if (spritebatch == null)
                return;

            if (_texture == null)
            {
                _texture = new Texture2D(spritebatch.GraphicsDevice, 1, 1);
                _texture.SetData(new[] { Color.White });
            }
            spritebatch.Draw(_texture, _position, null, Color, _angle, _origin, _scale, SpriteEffects.None, 0);
        }
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
