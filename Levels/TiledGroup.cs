using Microsoft.Xna.Framework.Graphics;

using System.Collections.Generic;

namespace DinaFramework.Levels
{
    /// <summary>
    /// Représente un groupe de calques dans un fichier Tiled (.tmx).
    /// Un groupe peut contenir d’autres layers, objets, groupes ou images.
    /// </summary>
    public class TiledGroup : ILayer
    {
        /// <summary>
        /// Nom du groupe tel que défini dans Tiled.
        /// </summary>
        public string Name { get; set; } = string.Empty;
        /// <summary>
        /// Opacité du groupe (valeur de 0 à 1).
        /// </summary>
        public float Opacity { get; set; }
        /// <summary>
        /// Indique si le groupe est visible.
        /// </summary>
        public bool Visible { get; set; }
        /// <summary>
        /// Liste des propriétés personnalisées associées au groupe.
        /// </summary>
        public List<IProperty> Properties { get; set; } = [];

        /// <summary>
        /// Calques de tuiles contenus dans ce groupe.
        /// </summary>
        public IReadOnlyList<TiledLayer> Layers => _layers;
        /// <summary>
        /// Groupes d’objets contenus dans ce groupe.
        /// </summary>
        public IReadOnlyList<TiledObjectGroup> Objects => _objects;
        /// <summary>
        /// Groupes imbriqués contenus dans ce groupe.
        /// </summary>
        public IReadOnlyList<TiledGroup> Groups => _groups;
        /// <summary>
        /// Calques d’images contenus dans ce groupe.
        /// </summary>
        public IReadOnlyList<TiledImageLayer> Images => _images;
        /// <summary>
        /// Liste plate de toutes les données de calques (layers, groupes, images, objets).
        /// </summary>
        public IReadOnlyList<ILayer> Datas => _datas;

        private readonly List<TiledLayer> _layers = [];
        private readonly List<TiledObjectGroup> _objects = [];
        private readonly List<TiledGroup> _groups = [];
        private readonly List<TiledImageLayer> _images = [];
        private readonly List<ILayer> _datas = [];


        /// <summary>
        /// Ajoute un layer de tuiles au groupe.
        /// </summary>
        public void AddLayer(TiledLayer layer)
        {
            _layers.Add(layer);
            _datas.Add(layer);
        }

        /// <summary>
        /// Ajoute un groupe d’objets au groupe.
        /// </summary>
        public void AddObject(TiledObjectGroup obj)
        {
            _objects.Add(obj);
            _datas.Add(obj);
        }

        /// <summary>
        /// Ajoute un sous-groupe.
        /// </summary>
        public void AddGroup(TiledGroup group)
        {
            _groups.Add(group);
            _datas.Add(group);
        }

        /// <summary>
        /// Ajoute un calque d’image au groupe.
        /// </summary>
        public void AddImage(TiledImageLayer image)
        {
            _images.Add(image);
            _datas.Add(image);
        }
    }
}
