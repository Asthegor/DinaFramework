using DinaFramework.Extensions;
using DinaFramework.Graphics;
using DinaFramework.Interfaces;
using DinaFramework.Services;

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using System;
using System.Collections;
using System.Collections.Generic;

namespace DinaFramework.Core
{
    /// <summary>
    /// Représente un groupe d'éléments, gérant leur affichage, visibilité, couleur et interactions.
    /// </summary>
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Naming", "CA1710:Identifiers should have correct suffix",
        Justification = "Group est clair dans le contexte du framework.")]
    public class Group : Base, IDraw, IVisible, IEnumerable<IElement>, ICollide, IUpdate, IClickable, IColor
    {
        private List<IElement> _elements = [];
        private int index;
        private Rectangle _rect;
        private bool _visible;
        private Color _color;
        private Texture2D _pixel;
        private IDrawingElement _title;
        private Rectangle? _titleRect;

        /// <summary>
        /// Initialise une nouvelle instance de la classe Group avec les propriétés spécifiées.
        /// </summary>
        /// <param name="position">Position initiale du groupe. Par défaut, (0,0).</param>
        /// <param name="dimensions">Dimensions initiales du groupe. Par défaut, (0,0).</param>
        /// <param name="zorder">Ordre d'affichage initial du groupe. Par défaut, 0.</param>
        public Group(Vector2 position = default, Vector2 dimensions = default, int zorder = 0) : base(position, dimensions, zorder)
        {
            _color = Color.White;
            Visible = true;
            _pixel = ServiceLocator.Get<Texture2D>(ServiceKey.Texture1px);
        }
        /// <summary>
        /// Initialise une nouvelle instance de la classe Group en copiant les éléments d'un autre groupe.
        /// </summary>
        /// <param name="group">Groupe source à copier.</param>
        /// <param name="duplicate">
        /// Indique si les éléments doivent être dupliqués. 
        /// Si false, les éléments sont simplement référencés.
        /// </param>
        /// <exception cref="ArgumentNullException">Lance une exception si le groupe fourni est null.</exception>
        public Group(Group group, bool duplicate = true)
        {
            ArgumentNullException.ThrowIfNull(group);
            foreach (var item in group._elements)
            {
                if (duplicate)
                    _elements.Add((IElement)Activator.CreateInstance(item.GetType(), item));
                else
                    _elements.Add(item);
            }
            Position = group.Position;
            Dimensions = group.Dimensions;
            ZOrder = group.ZOrder;
            Visible = group.Visible;
            index = 0;
            _color = Color.White;
        }

        /// <summary>
        /// Obtient l'élément actuel lors de l'énumération du groupe.
        /// </summary>
        public object Current => _elements[index];
        /// <summary>
        /// Obtient le rectangle représentant la position et les dimensions du groupe.
        /// </summary>
        public Rectangle Rectangle => _rect;

        /// <summary>
        /// Ajoute un élément au groupe.
        /// </summary>
        /// <param name="element">Élément à ajouter.</param>
        public void Add(IElement element)
        {
            _elements.Add(element);
            if (element is IDimensions)
                UpdateDimensions();
            SortElements();
        }
        
        /// <summary>
        /// Obtient ou définit la position du groupe. La modification de la position déplace également ses éléments.
        /// </summary>
        public override Vector2 Position
        {
            get => base.Position;
            set
            {
                Vector2 offset = value - base.Position;
                foreach (var element in _elements)
                {
                    if (element is IPosition item)
                        item.Position += offset;
                }
                if (_title != null)
                    _title.Position += offset;

                base.Position = value;
                _rect.Location = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        /// <summary>
        /// Obtient ou définit les dimensions du groupe.
        /// </summary>
        public override Vector2 Dimensions
        {
            get => base.Dimensions;
            set
            {
                base.Dimensions = value;
                _rect.Size = new Point(Convert.ToInt32(value.X), Convert.ToInt32(value.Y));
            }
        }
        /// <summary>
        /// Obtient ou définit la visibilité du groupe et de ses éléments.
        /// </summary>
        public bool Visible
        {
            get => _visible;
            set
            {
                foreach (var element in _elements)
                {
                    if (element is IVisible elemvisible)
                        elemvisible.Visible = value;
                }
                _visible = value;
            }
        }

        /// <summary>
        /// Obtient ou définit la couleur du groupe et de ses éléments.
        /// </summary>
        public Color Color
        {
            get => _color;
            set
            {
                foreach (var element in _elements)
                {
                    if (element is IColor elemcolor)
                        elemcolor.Color = value;
                }
                _color = value;
            }
        }

        /// <summary>
        /// Vérifie si un élément du groupe est cliqué.
        /// </summary>
        /// <returns>True si un élément est cliqué, sinon false.</returns>
        public bool IsClicked()
        {
            foreach (var item in _elements)
            {
                if (item is IClickable itemclickable)
                    if (itemclickable.IsClicked() == true)
                        return true;
            }
            return false;
        }

        /// <summary>
        /// Obtient le nombre d'éléments dans le groupe.
        /// </summary>
        /// <returns>Nombre d'éléments.</returns>
        public int Count() => _elements.Count;
        /// <summary>
        /// Passe à l'élément suivant lors de l'énumération.
        /// </summary>
        /// <returns>True si un élément suivant existe, sinon false.</returns>
        public bool MoveNext()
        {
            return (++index < _elements.Count);
        }
        /// <summary>
        /// Réinitialise l'énumération du groupe.
        /// </summary>
        public void Reset() => index = -1;
        /// <summary>
        /// Retourne un énumérateur pour parcourir les éléments du groupe.
        /// </summary>
        public IEnumerator<IElement> GetEnumerator() => _elements.GetEnumerator();

        // Implémentation non générique obligatoire par IEnumerable
        IEnumerator IEnumerable.GetEnumerator() => _elements.GetEnumerator();

        /// <summary>
        /// Vérifie si le groupe entre en collision avec un autre élément.
        /// </summary>
        /// <param name="item">Élément à tester pour la collision.</param>
        /// <returns>True si une collision est détectée, sinon false.</returns>
        public bool Collide(ICollide item)
        {
            if (item == null)
                return false;
            return Rectangle.Intersects(item.Rectangle);
        }
        /// <summary>
        /// Active ou désactive l’affichage d’un cadre autour du groupe.
        /// </summary>
        public bool HasFrame { get; set; }
        /// <summary>
        /// Couleur du cadre ou du fond si <see cref="HasFrame"/> est activé.
        /// </summary>
        public Color FrameColor { get; set; } = new Color(50, 50, 50, 200);
        /// <summary>
        /// Épaisseur du cadre.
        /// </summary>
        public int FrameThickness { get; set; } = 2;
        /// <summary>
        /// Espacement supplémentaire appliqué autour des éléments
        /// lorsque le cadre est dessiné.
        /// </summary>
        public int FramePadding { get; set; } = 8;
        /// <summary>
        /// Ajoute un titre au groupe.
        /// </summary>
        /// <param name="font">Police du titre.</param>
        /// <param name="text">Texte du titre.</param>
        /// 
        /// <param name="textcolor">Couleur du titre.</param>
        /// <param name="shadowcolor">Couleur de l'ombre (facultatif).</param>
        /// <param name="shadowoffset">Décalage de l'ombre (facultatif).</param>
        /// <param name="framecolor">Couleur du cadre (facultatif).</param>
        /// <param name="framepadding">Espacement supplémentaire appliqué autour des éléments.</param>
        /// <param name="framethickness">Épaisseur du cadre.</param>
        /// <param name="zorder">Ordre de superposition du titre (facultatif).</param>
        /// <returns>L'élément titre ajouté.</returns>
        public IDrawingElement AddTitle(SpriteFont font, string text, Color textcolor, Color? framecolor = null, int? framepadding = null, int? framethickness = null, Color? shadowcolor = null, Vector2? shadowoffset = null, int zorder = 0)
        {
            ArgumentNullException.ThrowIfNull(font, nameof(font));
            if (_title != null)
                _title = null;
            // Ajout du cadre
            HasFrame = true;
            FrameColor = framecolor ?? new Color(50, 50, 50, 200);
            FramePadding = framepadding ?? 8;
            if (FramePadding < font.LineSpacing / 2)
                FramePadding = font.LineSpacing / 2 + 1;
            FrameThickness = framethickness ?? 2;

            if (shadowcolor.HasValue && shadowoffset.HasValue)
                _title = new ShadowText(font, text, textcolor, Vector2.Zero, shadowcolor.Value, shadowoffset.Value, zorder: zorder);
            else
                _title = new DFText(font, text, textcolor, Vector2.Zero, zorder: zorder);

            Rectangle bounds = CalculateBounds();
            bounds.Inflate(FramePadding, FramePadding);

            // alignement : centré en haut
            float titleX = bounds.X + FramePadding * 2;
            float titleY = bounds.Y - _title.Dimensions.Y / 2f;

            _title.Position = new Vector2(titleX, titleY);
            _titleRect = new Rectangle((int)titleX, (int)titleY, (int)_title.Dimensions.X, (int)_title.Dimensions.Y);

            return _title;
        }
        /// <summary>
        /// Ajoute un titre à partir d'un élément déjà créé.
        /// </summary>
        /// <param name="title">L'élément titre à ajouter.</param>
        /// <param name="framecolor">Couleur du cadre (facultatif).</param>
        /// <param name="framepadding">Espacement supplémentaire appliqué autour des éléments.</param>
        /// <param name="framethickness">Épaisseur du cadre.</param>
        /// <returns>L'élément titre ajouté.</returns>
        public IDrawingElement AddTitle(IDrawingElement title, Color? framecolor = null, int? framepadding = null, int? framethickness = null)
        {
            if (_title != null && title != null)
                _title = null;
            _title = title;
            // Ajout du cadre
            HasFrame = true;
            FrameColor = framecolor ?? new Color(50, 50, 50, 200);
            FramePadding = framepadding ?? 8;

            Rectangle bounds = CalculateBounds();
            bounds.Inflate(FramePadding, FramePadding);

            // alignement : centré en haut
            float titleX = bounds.X + (bounds.Width - _title.Dimensions.X) / 2f;
            float titleY = bounds.Y - _title.Dimensions.Y / 2f;

            _title.Position = new Vector2(titleX, titleY);
            _titleRect = new Rectangle((int)titleX, (int)titleY, (int)_title.Dimensions.X, (int)_title.Dimensions.Y);

            return _title;
        }

        /// <summary>
        /// Permet d'ajouter un cadre au groupe.
        /// </summary>
        /// <param name="framecolor">Couleur du cadre.</param>
        /// <param name="framepadding">Espacement supplémentaire appliqué autour des éléments.</param>
        /// <param name="framethickness">Épaisseur du cadre.</param>
        public void AddFrame(Color framecolor, int framepadding, int framethickness)
        {
            HasFrame = true;
            FrameColor = framecolor;
            FramePadding = framepadding;
        }

        /// <summary>
        /// Dessine les éléments du groupe.
        /// </summary>
        /// <param name="spritebatch">Objet SpriteBatch utilisé pour le rendu.</param>
        public void Draw(SpriteBatch spritebatch)
        {
            if (!Visible)
                return;

            if (HasFrame)
            {
                Rectangle bounds = CalculateBounds();
                bounds.Inflate(FramePadding, FramePadding);

                if (_titleRect.HasValue)
                {
                    var titleRect = _titleRect.Value;
                    int t = FrameThickness;

                    // cadre avec trou
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.X, bounds.Y, titleRect.Left, t), FrameColor, t);
                    int rightX = bounds.X + titleRect.Right + titleRect.Left + FramePadding;
                    int rightWidth = bounds.Right - rightX;
                    spritebatch.DrawRectangle(_pixel, new Rectangle(rightX, bounds.Y, rightWidth, t), FrameColor, t);
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.X, bounds.Bottom - t, bounds.Width, t), FrameColor, t);
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.X, bounds.Y, t, bounds.Height), FrameColor, t);
                    spritebatch.DrawRectangle(_pixel, new Rectangle(bounds.Right - t, bounds.Y, t, bounds.Height), FrameColor, t);
                }
                else
                {
                    spritebatch.DrawRectangle(_pixel, bounds, FrameColor, FrameThickness, isFilled: false);
                }
            }
            if (_title is IDraw drawingTitle)
                drawingTitle.Draw(spritebatch);

            foreach (var element in _elements)
            {
                if (element is IDraw draw)
                    draw.Draw(spritebatch);
            }
        }
        private Rectangle CalculateBounds()
        {
            if (_elements.Count == 0)
                return Rectangle.Empty;

            int minX = int.MaxValue, minY = int.MaxValue;
            int maxX = int.MinValue, maxY = int.MinValue;

            foreach (var element in _elements)
            {
                Rectangle bounds = new Rectangle(element.Position.ToPoint(), element.Dimensions.ToPoint());
                if (bounds.Left < minX)
                    minX = bounds.Left;
                if (bounds.Top < minY)
                    minY = bounds.Top;
                if (bounds.Right > maxX)
                    maxX = bounds.Right;
                if (bounds.Bottom > maxY)
                    maxY = bounds.Bottom;
            }

            return new Rectangle(minX, minY, maxX - minX, maxY - minY);
        }
        /// <summary>
        /// Trie les éléments du groupe par ordre d'affichage (Z-order).
        /// </summary>
        public void SortElements()
        {
            _elements.Sort(delegate (IElement e1, IElement e2)
            {
                if (e1.ZOrder < e2.ZOrder)
                    return -1;
                if (e1.ZOrder > e2.ZOrder)
                    return 1;
                return 0;
            });
        }
        /// <summary>
        /// Met à jour les dimensions du groupe en fonction de ses éléments.
        /// </summary>
        private void UpdateDimensions()
        {
            float x, y;
            float w, h;
            x = Position.X;
            y = Position.Y;
            w = -1;
            h = -1;
            foreach (var element in _elements)
            {
                if (element is IDimensions elemdim && element is IPosition elempos)
                {
                    Vector2 elemPos = elempos.Position;
                    Vector2 elemDim = elemdim.Dimensions;

                    if (elemPos.X < x)
                        x = elemPos.X;
                    if (elemPos.Y < y)
                        y = elemPos.Y;
                    Vector2 flip = Vector2.One;
                    if (element is IFlip eflip)
                    {
                        // TODO: à corriger dès que la classe Image sera implémentée
                        //flip = eflip.GetFlip();
                    }
                    float cfvx = flip.X > 0 ? 1 : 0;
                    float cfvy = flip.Y > 0 ? 1 : 0;
                    if (w < elemPos.X + elemDim.X * cfvx)
                        w = elemPos.X + elemDim.X * cfvx;
                    if (h < elemPos.Y + elemDim.Y * cfvy)
                        h = elemPos.Y + elemDim.Y * cfvy;
                    //if (w < Math.Abs(elemPos.X) + Math.Abs(elemDim.X * cfvx))
                    //    w = Math.Abs(elemPos.X) + Math.Abs(elemDim.X * cfvx);
                    //if (h < Math.Abs(elemPos.Y) + Math.Abs(elemDim.Y * cfvy))
                    //    h = Math.Abs(elemPos.Y) + Math.Abs(elemDim.Y * cfvy);
                }
            } //foreach
            if (x < float.MaxValue && y < float.MaxValue && w > -1 && h > -1)
            {
                Dimensions = new Vector2(w - (x < 0 ? 0 : x), h - (y < 0 ? 0 : y));
            }

        }
        /// <summary>
        /// Met à jour les éléments du groupe.
        /// </summary>
        /// <param name="gametime">Temps de jeu actuel.</param>
        public void Update(GameTime gametime)
        {
            foreach (var elem in _elements)
            {
                if (elem is IUpdate uelem)
                    uelem.Update(gametime);
            }
        }
    }
}
