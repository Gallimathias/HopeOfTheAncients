using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients.Tiled;
public class Tile
{
    public string TileName { get; }
    public int Width { get; }
    public int Height { get; }
    public Tile(string tileName, int width, int height)
    {
        TileName = tileName;
        Width = width;
        Height = height;
    }

}
