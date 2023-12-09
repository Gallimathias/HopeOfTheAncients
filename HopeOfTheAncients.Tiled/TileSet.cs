using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients.Tiled
{
    public class TileSet
    {
        public TileSet(Tile[] tiles, int firstId, bool isUniform)
        {
            Tiles = tiles;
            FirstId = firstId;
            IsUniform = isUniform;
        }
        public Tile[] Tiles { get; }
        public bool IsUniform { get; }
        public int FirstId { get; }
    }
}
