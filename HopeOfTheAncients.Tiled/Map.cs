using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients.Tiled
{
    public class Map
    {
        public interface ILayer { }
        public class TileLayer : ILayer
        {

            public TileLayer(int width, int height)
            {
                Data = new int[width * height];
            }

            public int[] Data { get; }
            public int Width { get; }
            public int Height { get; }
            public int this[int x, int y]
            {
                get => Data[y * Width + x];
                set => Data[y * Width + x] = value;
            }
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
