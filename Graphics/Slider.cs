using DinaFramework.Core;
using DinaFramework.Enums;
using DinaFramework.Events;
using DinaFramework.Interfaces;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

using System;

namespace DinaFramework.Graphics
{
    /// <summary>
    /// Composant graphique représentant un slider (curseur) permettant de sélectionner une valeur dans une plage donnée.
    /// Supporte le glissement du curseur ainsi que l’incrément/décrément par clic sur la piste.
    /// Prend en charge différentes directions de progression via l’enum ProgressDirection.
    /// </summary>
    public class Slider : Base, IGameObject
    {
        /// <summary>
        /// Valeur minimale du slider.
        /// </summary>
        public float MinValue { get; set; }

        /// <summary>
        /// Valeur maximale du slider.
        /// </summary>
        public float MaxValue { get; set; }

        /// <summary>
        /// Incrément minimal entre deux valeurs.
        /// </summary>
        public float Step { get; set; }

        /// <summary>
        /// Valeur actuelle du slider.
        /// </summary>
        public float Value { get; private set; }

        /// <summary>
        /// Direction de progression du slider (ex: gauche-droite, droite-gauche, haut-bas, bas-haut).
        /// </summary>
        public ProgressDirection SliderOrientation { get; set; }

        /// <summary>
        /// Action appelée lorsque la valeur du slider change.
        /// </summary>
        public event EventHandler<SliderValueEventArgs> OnValueChanged;
        /// <summary>
        /// Position du slider.
        /// </summary>
        public new Vector2 Position
        {
            get => base.Position;
            set
            {
                base.Position = value;
                _track.Position = value;
                _thumb.Position = value;
            }
        }
        /// <summary>
        /// Dimensions du slider.
        /// </summary>
        public new Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _track.Dimensions = value;
                _thumb.Dimensions = CalculateThumbDimensions(SliderOrientation);
            }
        }

        private readonly Panel _track;
        private readonly Panel _thumb;
        private bool _isDragging;

        /// <summary>
        /// Crée un nouveau slider.
        /// </summary>
        /// <param name="position">Position de la piste.</param>
        /// <param name="dimensions">Dimensions de la piste.</param>
        /// <param name="minValue">Valeur minimale.</param>
        /// <param name="maxValue">Valeur maximale.</param>
        /// <param name="initialValue">Valeur initiale.</param>
        /// <param name="step">Incrément minimal.</param>
        /// <param name="orientation">Direction de progression du slider.</param>
        /// <param name="zorder"></param>
        public Slider(Vector2 position, Vector2 dimensions, float minValue, float maxValue, float initialValue, float step = 1f, ProgressDirection orientation = ProgressDirection.LeftToRight, int zorder = 0)
            : base(position, dimensions, zorder)
        {
            MinValue = minValue;
            MaxValue = maxValue;
            Step = step;
            SliderOrientation = orientation;

            _track = new Panel(position, dimensions, Color.Gray);

            var thumbSize = CalculateThumbDimensions(orientation);
            _thumb = new Panel(Vector2.Zero, thumbSize, Color.White);

            SetValue(initialValue);
            UpdateThumbPosition();
        }

        /// <summary>
        /// Met à jour l'état du slider (clic, glissement, incrément/décrément).
        /// </summary>
        public void Update(GameTime gametime)
        {
            MouseState mouse = Mouse.GetState();
            Vector2 mousePos = new Vector2(mouse.X, mouse.Y);

            Rectangle thumbBounds = new Rectangle(_thumb.Position.ToPoint(), _thumb.Dimensions.ToPoint());

            if (mouse.LeftButton == ButtonState.Pressed && !_isDragging && thumbBounds.Contains(mousePos))
            {
                _isDragging = true;
            }

            if (_isDragging && mouse.LeftButton == ButtonState.Pressed)
            {
                UpdateValueFromMouse(mousePos);
            }

            Rectangle trackBounds = new Rectangle(_track.Position.ToPoint(), _track.Dimensions.ToPoint());
            if (mouse.LeftButton == ButtonState.Pressed && !_isDragging && trackBounds.Contains(mousePos))
            {
                // Gère clic à gauche/droite ou haut/bas selon l’orientation et le sens
                switch (SliderOrientation)
                {
                    case ProgressDirection.LeftToRight:
                        if (mousePos.X < thumbBounds.Left)
                            SetValue(Value - Step);
                        else if (mousePos.X > thumbBounds.Right)
                            SetValue(Value + Step);
                        break;
                    case ProgressDirection.RightToLeft:
                        if (mousePos.X > thumbBounds.Right)
                            SetValue(Value - Step);
                        else if (mousePos.X < thumbBounds.Left)
                            SetValue(Value + Step);
                        break;
                    case ProgressDirection.TopToBottom:
                        if (mousePos.Y < thumbBounds.Top)
                            SetValue(Value + Step);
                        else if (mousePos.Y > thumbBounds.Bottom)
                            SetValue(Value - Step);
                        break;
                    case ProgressDirection.BottomToTop:
                        if (mousePos.Y > thumbBounds.Bottom)
                            SetValue(Value + Step);
                        else if (mousePos.Y < thumbBounds.Top)
                            SetValue(Value - Step);
                        break;
                }
            }

            if (mouse.LeftButton == ButtonState.Released)
            {
                _isDragging = false;
            }
        }

        /// <summary>
        /// Dessine la piste et le curseur.
        /// </summary>
        public void Draw(SpriteBatch spritebatch)
        {
            _track.Draw(spritebatch);
            _thumb.Draw(spritebatch);
        }

        /// <summary>
        /// Définit une nouvelle valeur pour le slider.
        /// </summary>
        /// <param name="newValue">Nouvelle valeur à appliquer.</param>
        public void SetValue(float newValue)
        {
            newValue = MathHelper.Clamp(newValue, MinValue, MaxValue);

            if (Step > 0)
                newValue = (float)Math.Round(newValue / Step) * Step;

            if (Math.Abs(Value - newValue) > float.Epsilon)
            {
                Value = newValue;
                UpdateThumbPosition();
                OnValueChanged?.Invoke(this, new SliderValueEventArgs(Value));
            }
        }

        private void UpdateValueFromMouse(Vector2 mousePos)
        {
            float ratio = 0f;

            switch (SliderOrientation)
            {
                case ProgressDirection.LeftToRight:
                    ratio = MathHelper.Clamp((mousePos.X - _track.Position.X) / _track.Dimensions.X, 0f, 1f);
                    break;
                case ProgressDirection.RightToLeft:
                    ratio = 1f - MathHelper.Clamp((mousePos.X - _track.Position.X) / _track.Dimensions.X, 0f, 1f);
                    break;
                case ProgressDirection.TopToBottom:
                    ratio = MathHelper.Clamp((mousePos.Y - _track.Position.Y) / _track.Dimensions.Y, 0f, 1f);
                    break;
                case ProgressDirection.BottomToTop:
                    ratio = 1f - MathHelper.Clamp((mousePos.Y - _track.Position.Y) / _track.Dimensions.Y, 0f, 1f);
                    break;
            }

            float newValue = MinValue + ratio * (MaxValue - MinValue);
            SetValue(newValue);
        }

        private void UpdateThumbPosition()
        {
            float ratio = (Value - MinValue) / (MaxValue - MinValue);
            Vector2 thumbPos = Vector2.Zero;

            switch (SliderOrientation)
            {
                case ProgressDirection.LeftToRight:
                    thumbPos = new Vector2(
                        _track.Position.X + ratio * (_track.Dimensions.X - _thumb.Dimensions.X),
                        _track.Position.Y
                    );
                    break;

                case ProgressDirection.RightToLeft:
                    thumbPos = new Vector2(
                        _track.Position.X + (1f - ratio) * (_track.Dimensions.X - _thumb.Dimensions.X),
                        _track.Position.Y
                    );
                    break;

                case ProgressDirection.TopToBottom:
                    thumbPos = new Vector2(
                        _track.Position.X,
                        _track.Position.Y + ratio * (_track.Dimensions.Y - _thumb.Dimensions.Y)
                    );
                    break;

                case ProgressDirection.BottomToTop:
                    thumbPos = new Vector2(
                        _track.Position.X,
                        _track.Position.Y + (1f - ratio) * (_track.Dimensions.Y - _thumb.Dimensions.Y)
                    );
                    break;
            }

            _thumb.Position = thumbPos;
        }
        private Vector2 CalculateThumbDimensions(ProgressDirection orientation)
        {
            // Taille du thumb rectangulaire avec ratio 4:1 (H:W si horizontal, W:H si vertical)
            Vector2 thumbSize;
            if (orientation == ProgressDirection.LeftToRight || orientation == ProgressDirection.RightToLeft)
            {
                // Horizontal : thumb fait toute la hauteur, largeur = hauteur / 4
                float height = Dimensions.Y;
                float width = height / 4f;
                thumbSize = new Vector2(width, height);
            }
            else
            {
                // Vertical : thumb fait toute la largeur, hauteur = largeur / 4
                float width = Dimensions.X;
                float height = width / 4f;
                thumbSize = new Vector2(width, height);
            }
            return thumbSize;
        }
    }


}
