using engenious.UI;
using HopeOfTheAncients.Tiled;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients.Renderer;
public class MapRenderer : IDisposable
{
    private List<ILayerRenderer> layerRenderers = new();
    private bool disposedValue;
    private readonly UniformTileRenderer mapTileRenderer;
    private readonly NonUniformTileRenderer objectTileRenderer;
    public MapRenderer(BaseScreenComponent manager, Map map)
    {

        foreach (var set in map.TileSet)
        {
            // TODO: support multiple tilesets
            if (set.IsUniform)
                mapTileRenderer = new UniformTileRenderer(manager.GraphicsDevice, set);
            else
                objectTileRenderer = new NonUniformTileRenderer(manager.GraphicsDevice, set);
        }
        foreach (var layer in map.Layers)
        {
            layerRenderers.Add(CreateLayerRenderer(manager, layer, mapTileRenderer, objectTileRenderer));
        }
    }

    internal static ILayerRenderer CreateLayerRenderer(BaseScreenComponent manager, Map.ILayer layer, UniformTileRenderer mapTileRenderer, NonUniformTileRenderer objectTileRenderer)
    {
        if (layer is Map.TileLayer tileLayer)
            return new LayerRenderer(manager, tileLayer, mapTileRenderer);
        else if (layer is Map.Group groupLayer)
            return new GroupRenderer(manager, groupLayer, mapTileRenderer, objectTileRenderer);
        else if (layer is Map.ObjectGroup objectLayer)
            return new ObjectRenderer(manager, objectLayer, objectTileRenderer);

        throw new NotSupportedException();
    }

    public void Render(Camera camera)
    {
        foreach (var renderer in layerRenderers)
        {
            renderer.Render(camera);
        }
    }

    public void Dispose()
    {

    }
}
