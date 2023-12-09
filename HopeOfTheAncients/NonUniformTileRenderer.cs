using engenious.Graphics;
using HopeOfTheAncients.Tiled;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;
public class NonUniformTileRenderer
{
    public Texture2D[] Textures { get; }
    public int FirstId { get; }
    public NonUniformTileRenderer(GraphicsDevice graphicsDevice, TileSet set)
    {
        FirstId = set.FirstId;
        Textures = new Texture2D[set.Tiles.Length];
        for (var i = 0; i < set.Tiles.Length; i++)
        {
            Tile? t = set.Tiles[i];
            var texPath = Path.Combine(".", "Assets", t.TileName);
            if (!File.Exists(texPath))
                continue;
            var loadedTex = Texture2D.FromFile(graphicsDevice, texPath);
            Textures[i] = loadedTex;
        }
    }
}
