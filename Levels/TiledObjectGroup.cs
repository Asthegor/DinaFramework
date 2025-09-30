using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DinaFramework.Levels
{
    public class TiledObjectGroup : ILayer
    {
        private List<TiledObject> _objects = [];

        public string Name { get; set; }
        public bool Visible
        {
            get => _visible;
            set
            {
                _visible = value;
                foreach (var obj in _objects)
                    obj.Visible = value;
            }
        }
        private bool _visible;
        public float Opacity { get; set; }
        public IReadOnlyList<TiledObject> Objects => _objects;

        public string Parent { get; internal set; } = string.Empty;

        public void AddObject(TiledObject obj)
        {
            _objects.Add(obj);
        }
    }
}
