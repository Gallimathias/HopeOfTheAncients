using engenious.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HopeOfTheAncients.Tiled.Map;

namespace HopeOfTheAncients.Renderer;
internal class GroupRenderer : ILayerRenderer
{
    private List<ILayerRenderer> layerRenderers = new();
    public GroupRenderer(BaseScreenComponent manager, Group layer, UniformTileRenderer mapTileRenderer, NonUniformTileRenderer objectTileRenderer)
    {
        foreach(var c in layer.Children)
        {
            layerRenderers.Add(MapRenderer.CreateLayerRenderer(manager, c, mapTileRenderer, objectTileRenderer));
        }
    }
    public void Render(Camera camera)
    {
        foreach (var renderer in layerRenderers)
        {
            renderer.Render(camera);
        }
    }
}
