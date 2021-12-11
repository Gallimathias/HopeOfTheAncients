using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients.Tiled
{
    public class TileSet
    {
        public TileSet(int tileCount)
        {
            Tiles = new string[tileCount];
        }
        public string[] Tiles { get; }
    }
}
