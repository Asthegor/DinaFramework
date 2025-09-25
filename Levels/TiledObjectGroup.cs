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
        public bool Visible { get; set; }
        public float Opacity { get; set; }
        public IReadOnlyList<TiledObject> Objects => _objects;
        public void AddObject(TiledObject obj)
        {
            _objects.Add(obj);
        }
    }
}
