using engenious.Graphics;
using HopeOfTheAncients.Tiled;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;
public class UniformTileRenderer
{
    public Texture2DArray Textures { get; }
    public UniformTileRenderer(GraphicsDevice graphicsDevice, TileSet set)
    {
        if (!set.IsUniform)
            throw new ArgumentException(null, nameof(set));

        var texts = new List<Texture2D>();
        int sizeX = -1, sizeY = -1;
        foreach (var t in set.Tiles)
        {
            var texPath = Path.Combine(".", "Assets", t.TileName);
            if (!File.Exists(texPath))
                continue;
            var loadedTex = Texture2D.FromFile(graphicsDevice, texPath);
            if (sizeX != -1 && (loadedTex.Width != sizeX || loadedTex.Height != sizeY))
                throw new InvalidProgramException();
            sizeX = loadedTex.Width;
            sizeY = loadedTex.Height;
            texts.Add(loadedTex);
        }

        Textures = new Texture2DArray(graphicsDevice, 1, sizeX, sizeY, texts.ToArray());
    }

}
