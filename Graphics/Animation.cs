using DinaFramework.Core;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections.Generic;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Représente une animation graphique composée de plusieurs frames (images).
    /// Gère la répétition, la vitesse, et d'autres propriétés visuelles comme la rotation, l'échelle, ou le retournement (flip).
    /// </summary>
    public class Animation : Base, IReset, IUpdate, IDraw, IColor, ICollide, IVisible
    {
        private readonly List<Texture2D> _frames = [];
        private float _speed;
        private float _currentFrame;
        private Color _color;
        private Vector2 _origin;
        private Vector2 _flip;
        private Rectangle _rect;
        private readonly int _nbRepetitions = -1;
        private int _currentRepetition = -1;
        private Vector2 _scale;
        private float _rotation;
        private bool _visible;

        /// <summary>
        /// Initialise une nouvelle instance de Animation.
        /// </summary>
        /// <param name="frames">Liste des textures représentant les frames de l'animation.</param>
        /// <param name="speed">Vitesse de l'animation (en frames par seconde).</param>
        /// <param name="nbRepetitions">Nombre de répétitions de l'animation (-1 pour une répétition infinie, valeur par défaut : -1).</param>
        /// <param name="position">Position de l'animation (par défaut : (0, 0)).</param>
        /// <param name="dimensions">Dimensions de l'animation (par défaut : dimensions de la première frame).</param>
        /// <param name="origin">Point d'origine de l'animation pour la rotation (par défaut : (0, 0)).</param>
        /// <param name="rotation">Angle de rotation de l'animation (par défaut : 0).</param>
        /// <param name="scale">Facteur d'échelle de l'animation (par défaut : (1, 1)).</param>
        /// <param name="flip">Facteur de retournement de l'animation (par défaut : (1, 1)).</param>
        /// <param name="visible">Indique si l'animation est visible (par défaut : true).</param>
        /// <param name="zorder">Ordre de superposition de l'animation (par défaut : 0).</param>
        public Animation(List<Texture2D> frames, float speed, int nbRepetitions = -1, Vector2 position = default, Vector2 dimensions = default, Vector2 origin = default, float rotation = 0, Vector2 scale = default, Vector2 flip = default, bool visible = true, int zorder = default) : base(position, dimensions, zorder)
        {
            ArgumentNullException.ThrowIfNull(frames);
            if (frames.Count == 0)
                throw new ArgumentException("The list of frames must not be empty.");

            _rect = new Rectangle();
            _frames = frames;
            _currentFrame = 0;
            _nbRepetitions = nbRepetitions;
            _currentRepetition = nbRepetitions;
            Scale = scale;
            Visible = visible;
            Position = position;
            Dimensions = dimensions == default ? new Vector2(_frames[0].Width, _frames[0].Height) : dimensions;
            Color = Color.White;
            Speed = speed;
            Origin = origin;
            Rotation = rotation;
            Flip = flip;
            Visible = visible;
        }

        /// <summary>
        /// Vitesse d'exécution de l'animation.
        /// </summary>
        public float Speed { get => _speed; set => _speed = value; }
        /// <summary>
        /// Position actuelle de l'animation sur l'écran.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _rect.Location = value.ToPoint();
            }
        }
        /// <summary>
        /// Taille de l'animation (largeur et hauteur).
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _rect.Size = value.ToPoint();
            }
        }
        /// <summary>
        /// Couleur appliquée à l'animation lors du rendu.
        /// </summary>
        public Color Color
        {
            get => _color;
            set => _color = value;
        }
        /// <summary>
        /// Point d'origine utilisé pour les transformations, comme la rotation ou le redimensionnement.
        /// </summary>
        public Vector2 Origin
        {
            get => _origin;
            set => _origin = value;
        }
        /// <summary>
        /// Angle de rotation de l'animation en radians.
        /// </summary>
        public float Rotation
        {
            get => _rotation;
            set => _rotation = value;
        }
        /// <summary>
        /// Facteur de redimensionnement de l'animation sur les axes X et Y.
        /// </summary>
        public Vector2 Scale
        {
            get => _scale;
            set => _scale = value;
        }
        /// <summary>
        /// Permet de retourner l'animation horizontalement ou verticalement.
        /// </summary>
        public Vector2 Flip
        {
            get => _flip;
            set
            {
                _flip = value;
                //if (value.X != 0)
                //    value.X /= Math.Abs(value.X);
                //if (value.Y != 0)
                //    value.Y /= Math.Abs(value.Y);
            }
        }
        /// <summary>
        /// Rectangle de délimitation actuel de l'animation.
        /// </summary>
        public Rectangle Rectangle => _rect;
        /// <summary>
        /// Indique si l'animation est visible.
        /// </summary>
        public bool Visible { get => _visible; set => _visible = value; }

        /// <summary>
        /// Met à jour l'état de l'animation.
        /// </summary>
        /// <param name="gametime">Temps de jeu écoulé depuis la dernière mise à jour.</param>
        public void Update(GameTime gametime)
        {
            if (Visible && _currentRepetition != 0)
            {
                ArgumentNullException.ThrowIfNull(gametime);

                _currentFrame += Convert.ToSingle(gametime.ElapsedGameTime.TotalSeconds) * _speed;
                if (_currentFrame >= _frames.Count)
                {
                    _currentFrame = 0;
                    if (_currentRepetition > 0)
                        _currentRepetition--;
                    if (_currentRepetition == 0)
                        Visible = false;
                }
            }
        }

        /// <summary>
        /// Affiche l'animation sur l'écran.
        /// </summary>
        /// <param name="spritebatch">Objet SpriteBatch utilisé pour dessiner l'animation.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            ArgumentNullException.ThrowIfNull(spritebatch);

            if (Visible && _currentRepetition != 0)
                spritebatch.Draw(_frames[(int)_currentFrame], _rect, null, _color, _rotation, _origin, SpriteEffects.None, ZOrder);
        }

        /// <summary>
        /// Vérifie si l'animation entre en collision avec un autre objet.
        /// </summary>
        /// <param name="item">Objet implémentant l'interface ICollide.</param>
        /// <returns>Retourne true si une collision est détectée, sinon false.</returns>
        public bool Collide(ICollide item)
        {
            if (item == null)
                return false;

            return Rectangle.Intersects(item.Rectangle);
        }

        /// <summary>
        /// Indique si l'animation a terminé son cycle.
        /// </summary>
        /// <returns>True si l'animation est terminée, sinon false.</returns>
        public bool IsFinished() => _currentRepetition == 0;

        /// <summary>
        /// Réinitialise l'animation à son état initial.
        /// </summary>
        public void Reset()
        {
            _currentRepetition = _nbRepetitions;
            _currentFrame = 0;
        }

        //public Animation Copy()
        //{
        //    var copiedFrames = new List<Texture2D>(_frames);
        //    return new Animation(copiedFrames, _speed, _nbRepetitions, Position, Dimensions, _origin,_scale, _flip, _visible, ZOrder)
        //    {
        //        _currentFrame = _currentFrame,
        //        _color = _color,
        //        _currentRepetition = _currentRepetition,
        //    };
        //}
    }
}


//using DinaFramework.Graphics;
//using DinaFramework.Interfaces;

//using Microsoft.Xna.Framework;
//using Microsoft.Xna.Framework.Content;
//using Microsoft.Xna.Framework.Graphics;

//using System;
//using System.Collections.Generic;

//namespace DinaFramework.Graphics
//{
//    public class Animation : Base, IReset, IUpdate, IDraw, IColor, ICollide, IVisible, ICopyable<Animation>
//    {
//        private List<Sprite> _frames = new List<Sprite>();
//        private float _speed;
//        private float _currentframe;
//        private float _rotation;
//        private Color _color;
//        private Vector2 _origin;
//        private Vector2 _flip;
//        private Rectangle _rect;
//        private int _nbRepetitions = -1;
//        private int _currentrepetition = -1;
//        private Vector2 _scale;
//        private bool _visible;
//        private Rectangle[] _sourceRectangles; // Les rectangles délimitant chaque frame dans l'image principale

//        public Animation(ContentManager content, string prefix, int nbframes, float speed, int start, int nbRepetitions, Color color,
//                         Vector2 position, Vector2 dimensions, float rotation = default, Vector2 origin = default,
//                         Vector2 flip = default, int zorder = default) : base(position, dimensions, zorder)
//        {
//            ArgumentNullException.ThrowIfNull(content);

//            _speed = speed;
//            _nbRepetitions = nbRepetitions;
//            _currentrepetition = nbRepetitions;
//            Color = color == default ? Color.White : color;
//            Rotation = 0.0f;
//            Origin = origin;
//            Flip = flip == default ? Vector2.One : flip;
//            Scale = Vector2.One;
//            Visible = true;
//            AddFrames(content, prefix, nbframes, start, Dimensions, rotation, Origin);
//        }
//        public Animation(ContentManager content, string prefix, int nbframes, float speed, int start, int nbRepetitions, Color color,
//                         Vector2 position, float rotation, Vector2 origin = default, Vector2 scale = default,
//                         Vector2 flip = default, int zorder = default) : base(position, default, zorder)
//        {
//            ArgumentNullException.ThrowIfNull(content);

//            Origin = origin;
//            Scale = scale == default ? Vector2.One : scale;
//            AddFrames(content, prefix, nbframes, start, rotation, Origin, Scale);
//            _speed = speed;
//            _nbRepetitions = nbRepetitions;
//            _currentrepetition = nbRepetitions;
//            Color = color == default ? Color.White : color;
//            Rotation = rotation;
//            Flip = flip == default ? Vector2.One : flip;
//            Visible = true;
//        }
//        public Animation(ContentManager content, string[] frameNames, float speed, int startframe, int nbRepetitions, Color color,
//                         Vector2 position, float rotation, Vector2 origin = default, Vector2 scale = default,
//                         Vector2 flip = default, int zorder = default) : base(position, default, zorder)
//        {
//            ArgumentNullException.ThrowIfNull(content);
//            ArgumentNullException.ThrowIfNull(frameNames);

//            Flip = flip == default ? Vector2.One : flip;
//            foreach (string name in frameNames)
//            {
//                Texture2D texture = content.Load<Texture2D>(name);
//                //_frames.Add(new Sprite(texture, color, Position, rotation, origin, scale, Flip, ZOrder));
//                _frames.Add(new Sprite(texture, color, Position, null, origin, Flip, rotation, scale, SpriteEffects.None, ZOrder));
//            }
//            _speed = speed;
//            _nbRepetitions = nbRepetitions;
//            _currentrepetition = nbRepetitions;
//            Color = color == default ? Color.White : color;
//            Rotation = rotation;
//            Origin = origin;
//            Scale = scale == default ? Vector2.One : scale;
//            Visible = true;
//            Dimensions = _frames.Count > 0 ? _frames[0].Dimensions : Vector2.Zero;
//            _currentframe = startframe >= 0 && startframe <= _frames.Count ? startframe : 0;
//        }
//        public Animation(ContentManager content, string[] frameNames, float speed, int startframe, int nbRepetitions, Color color,
//                         Vector2 position, Vector2 dimensions, float rotation = default, Vector2 origin = default, Vector2 scale = default,
//                         Vector2 flip = default, int zorder = default) : base(position, dimensions, zorder)
//        {
//            ArgumentNullException.ThrowIfNull(content);
//            ArgumentNullException.ThrowIfNull(frameNames);

//            Color = color == default ? Color.White : color;
//            Flip = flip == default ? Vector2.One : flip;
//            foreach (string name in frameNames)
//            {
//                Texture2D texture = content.Load<Texture2D>(name);
//                if (dimensions == Vector2.Zero)
//                {
//                    dimensions = new Vector2(texture.Width, texture.Height);
//                }
//                //_frames.Add(new Sprite(texture, Color, Position, dimensions, rotation, origin, Flip, ZOrder));
//                _frames.Add(new Sprite(texture, Color, Position, new Rectangle(Point.Zero, dimensions.ToPoint()), origin, Flip, rotation, Vector2.One, SpriteEffects.None, ZOrder));
//            }
//            _speed = speed;
//            _nbRepetitions = nbRepetitions;
//            _currentrepetition = nbRepetitions;
//            Rotation = rotation;
//            Origin = origin;
//            Scale = scale == default ? Vector2.One : scale;
//            Visible = true;

//            _currentframe = startframe >= 0 && startframe <= _frames.Count ? startframe : 0;

//            Dimensions = dimensions;
//        }
//        public Animation(ContentManager content, Texture2D spritesheet, int frameWidth, int frameHeight, int frameCount, float speed, int startframe, int nbRepetitions,
//                         Color color, Vector2 position, Vector2 dimensions, float rotation = 0, Vector2 origin = default, Vector2 scale = default,
//                         Vector2 flip = default, int zorder = default) : base(position, dimensions, zorder)
//        {
//            ArgumentNullException.ThrowIfNull(content);
//            ArgumentNullException.ThrowIfNull(spritesheet);
//            ArgumentNullException.ThrowIfNull(spritesheet);
//            if (frameCount <= 0)
//                throw new ArgumentException("frameCount must be greater than 0");
//            if (frameWidth <= 0 || frameHeight <= 0)
//                throw new ArgumentException("frameWidth and frameHeight must be greater than 0");

//            _speed = speed;
//            _nbRepetitions = nbRepetitions;
//            _currentrepetition = nbRepetitions;
//            _color = color;
//            _origin = origin;
//            _flip = flip == default ? Vector2.One : flip;
//            _scale = Vector2.One;
//            Visible = true;
//            _currentframe = 0;

//            // Calcule le nombre de frames dans l'image principale
//            int framesPerRow = spritesheet.Width / frameWidth;
//            int frameRows = spritesheet.Height / frameHeight;
//            int totalFrames = framesPerRow * frameRows;

//            if (frameCount > totalFrames)
//                throw new ArgumentException("frameCount exceeds the number of frames in the sprite sheet");

//            // Calcule les rectangles source pour chaque frame
//            _sourceRectangles = new Rectangle[frameCount];
//            int frameIndex = 0;
//            for (int row = 0; row < frameRows; row++)
//            {
//                for (int col = 0; col < framesPerRow; col++)
//                {
//                    if (frameIndex >= frameCount)
//                        break;

//                    _sourceRectangles[frameIndex] = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);

//                    Rectangle sourceRect = new Rectangle(col * frameWidth, row * frameHeight, frameWidth, frameHeight);
//                    //_frames[frameIndex] = new Sprite(spritesheet, color, position, dimensions, rotation, origin, flip, sourceRect, zorder);
//                    //Sprite s = new Sprite(spritesheet, Color.White, position, _sourceRectangles[frameIndex], Vector2.Zero, Vector2.One, 0, Vector2.One, SpriteEffects.None, zorder);
//                    //_frames.Add(s);
//                    _frames.Add(new Sprite(spritesheet, color, position, sourceRect, origin, flip, rotation, Vector2.One, SpriteEffects.None, zorder));

//                    frameIndex++;
//                }
//            }
//        }
//        public Animation(List<Texture2D> frames, float speed, int nbRepetitions = -1, Vector2 position = default, Vector2 dimensions = default, Vector2 origin = default, float rotation = 0, Vector2 scale = default, Vector2 flip = default, bool visible = true)
//        {
//            ArgumentNullException.ThrowIfNull(frames);

//            _frames = frames;
//            _currentframe = 0;
//            _rect = new Rectangle(position.ToPoint(), dimensions.ToPoint());
//            _nbRepetitions = nbRepetitions;
//            _currentrepetition = nbRepetitions;
//            Scale = scale;
//            Visible = visible;
//            Position = position;
//            Dimensions = dimensions;
//            Color = Color.White;
//            Speed = speed;
//            Origin = origin;
//            Rotation = rotation;
//            Scale = scale;
//            Flip = flip;
//            Visible = visible;
//        }

//        private void AddFrames(ContentManager content, string prefix, int nbframes, int start, Vector2 dimensions, float rotation, Vector2 origin)
//        {
//            ArgumentNullException.ThrowIfNull(content);

//            Texture2D texture;
//            for (int index = start; index < nbframes + start; index++)
//            {
//                string strIndex = index.ToString();
//                texture = content.Load<Texture2D>(prefix + strIndex);
//                if (dimensions == Vector2.Zero)
//                {
//                    dimensions = new Vector2(texture.Width, texture.Height);
//                }
//                //_frames.Add(new Sprite(texture, Color, Position, dimensions, rotation, origin, Flip, ZOrder));
//                _frames.Add(new Sprite(texture, Color, Position, new Rectangle(Point.Zero, dimensions.ToPoint()), origin, Flip, rotation, Vector2.One, SpriteEffects.None, ZOrder));
//            }
//            Dimensions = dimensions;
//        }
//        private void AddFrames(ContentManager content, string prefix, int nbframes, int start, float rotation, Vector2 origin, Vector2 scale)
//        {
//            Texture2D texture;
//            for (int index = start; index < nbframes + start; index++)
//            {
//                if (ImageExists(content, prefix + index.ToString("00")))
//                    texture = content.Load<Texture2D>(prefix + index.ToString("00"));
//                else
//                    texture = content.Load<Texture2D>(prefix + index.ToString());
//                //_frames.Add(new Sprite(texture, Color, Position, rotation, origin, scale, Flip, ZOrder));
//                _frames.Add(new Sprite(texture, Color, Position, new Rectangle(0, 0, texture.Width, texture.Height), origin, Flip, rotation, scale, SpriteEffects.None, ZOrder));
//            }
//        }
//        private bool ImageExists(ContentManager content, string imageName)
//        {
//            try
//            {
//                content.Load<Texture2D>(imageName);
//                return true; // L'image existe
//            }
//            catch (ContentLoadException)
//            {
//                return false; // L'image n'existe pas
//            }
//        }
//        public float Speed { get => _speed; set => _speed = value; }
//        public override Vector2 Position
//        {
//            get { return base.Position; }
//            set
//            {
//                foreach (Sprite frame in _frames)
//                    frame.Position = value + Origin;
//                base.Position = value;
//                _rect.Location = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
//            }
//        }
//        public override Vector2 Dimensions
//        {
//            get { return base.Dimensions; }
//            set
//            {
//                foreach (Sprite frame in _frames)
//                    frame.Dimensions = value;
//                base.Dimensions = value;
//                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
//            }
//        }
//        public Color Color
//        {
//            get { return _color; }
//            set { _color = value; }
//        }
//        public Vector2 Origin
//        {
//            get { return _origin; }
//            set
//            {
//                foreach (Sprite frame in _frames)
//                    frame.Origin = value;
//                _origin = value;
//            }
//        }
//        public void CenterOrigin()
//        {
//            foreach (Sprite frame in _frames)
//                frame.CenterOrigin();
//        }
//        public float Rotation
//        {
//            get { return _rotation; }
//            set
//            {
//                foreach (Sprite frame in _frames)
//                    frame.Rotation = value;
//                _rotation = value;
//            }
//        }
//        public Vector2 Scale
//        {
//            get => _scale;
//            set
//            {
//                _scale = value;
//                foreach (Sprite frame in _frames)
//                    frame.Scale = value;
//            }
//        }
//        public Vector2 Flip
//        {
//            get { return _flip; }
//            set
//            {
//                foreach (Sprite frame in _frames)
//                    frame.Flip = value;
//                if (value.X != 0)
//                    value.X /= Math.Abs(value.X);
//                if (value.Y != 0)
//                    value.Y /= Math.Abs(value.Y);
//                _flip = value;
//            }
//        }
//        public Rectangle Rectangle { get { return _rect; } }

//        public bool Visible { get => _visible; set => _visible = value; }

//        public void Update(GameTime gametime)
//        {
//            if (Visible && _currentrepetition != 0)
//            {
//                ArgumentNullException.ThrowIfNull(gametime);

//                _currentframe += Convert.ToSingle(gametime.ElapsedGameTime.TotalSeconds) * _speed;
//                if (_currentframe >= _frames.Count)
//                {
//                    _currentframe = 0;
//                    if (_currentrepetition > 0)
//                        _currentrepetition--;
//                    if (_currentrepetition == 0)
//                        Visible = false;
//                }
//            }
//        }
//        public void Draw(SpriteBatch spritebatch)
//        {
//            if (Visible && _currentrepetition != 0)
//                _frames[(int)_currentframe].Draw(spritebatch);
//        }
//        public bool Collide(ICollide item)
//        {
//            if ((item == null))
//                return false;

//            return Rectangle.Intersects(item.Rectangle);
//        }
//        public bool IsFinished() => _currentrepetition == 0;
//        public void Reset()
//        {
//            _currentrepetition = _nbRepetitions;
//            _currentframe = 0;
//        }
//        public Animation Copy()
//        {
//            List<Sprite> copiedFrames = new List<Sprite>();
//            foreach (Sprite sprite in _frames)
//                copiedFrames.Add(sprite.Copy());
//            Rectangle[] copiedRectangles = new Rectangle[_sourceRectangles.Length];
//            Array.Copy(_sourceRectangles, copiedRectangles, _sourceRectangles.Length);
//            return new Animation()
//            {
//                _color = this._color,
//                _currentframe = this._currentframe,
//                _currentrepetition = this._currentrepetition,
//                _frames = copiedFrames,
//                _flip = this._flip,
//                _origin = this._origin,
//                _rect = this._rect,
//                _rotation = this._rotation,
//                _scale = this._scale,
//                _sourceRectangles = copiedRectangles,
//                _speed = this._speed,
//                _visible = this._visible,
//                Color = this.Color,
//                Dimensions = this.Dimensions,
//                Flip = this.Flip,
//                Origin = this.Origin,
//                Position = this.Position,
//                Rotation = this.Rotation,
//                Scale = this.Scale,
//                Speed = this.Speed,
//                Visible = this.Visible,
//                ZOrder = this.ZOrder,
//            };
//        }
//        private Animation() { }
//    }
//}
