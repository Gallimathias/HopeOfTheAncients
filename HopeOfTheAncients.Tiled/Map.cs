using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients.Tiled
{
    public class Map
    {
        public interface ILayer
        {
            string Name { get; }
        }
        public class TileLayer : ILayer
        {

            public TileLayer(string name, int width, int height)
            {
                Name = name;
                Data = new int[width * height];
                Width = width;
                Height = height;
            }

            public int[] Data { get; }
            public int Width { get; }
            public int Height { get; }
            public string Name { get; }

            public int this[int x, int y]
            {
                get => Data[y * Width + x];
                set => Data[y * Width + x] = value;
            }
        }
        public class ObjectGroup : ILayer
        {
            public ObjectGroup(string name)
            {
                Name = name;
                Entities = new();
            }
            public List<Entity> Entities { get; }
            public string Name { get; }
        }
        public class Group : ILayer
        {
            public Group(string name)
            {
                Name = name;
                Children = new();
            }

            public string Name { get; }

            public List<ILayer> Children { get; }
        }

        public Map(TileSet[] tileset, int layerCount)
        {
            TileSet = tileset;
            Layers = new ILayer[layerCount];
        }
        public ILayer[] Layers { get; }

        public TileSet[] TileSet { get; }
    }
}
