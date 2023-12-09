using engenious.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HopeOfTheAncients.Tiled.Map;

namespace HopeOfTheAncients.Renderer;
public class LayerRenderer : ILayerRenderer
{
    private readonly ChunkRenderer chunkRenderer;
    private readonly UniformTileRenderer tileRenderer;

    public LayerRenderer(BaseScreenComponent screenManager, TileLayer layer, UniformTileRenderer tileRenderer)
    {
        chunkRenderer = new ChunkRenderer(screenManager, layer);
        this.tileRenderer = tileRenderer;
    }
    public void Render(Camera camera)
    {
        chunkRenderer.Render(camera, tileRenderer);
    }
}
