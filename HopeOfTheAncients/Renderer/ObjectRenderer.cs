using engenious;
using engenious.Graphics;
using engenious.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static HopeOfTheAncients.Tiled.Map;

namespace HopeOfTheAncients.Renderer;
internal class ObjectRenderer : ILayerRenderer
{
    private readonly BaseScreenComponent manager;
    private readonly ObjectGroup layer;
    private readonly NonUniformTileRenderer objectTileRenderer;
    private readonly SpriteBatch batch;

    public ObjectRenderer(BaseScreenComponent manager, ObjectGroup layer, NonUniformTileRenderer objectTileRenderer)
    {
        batch = new SpriteBatch(manager.GraphicsDevice);
        this.manager = manager;
        this.layer = layer;
        this.objectTileRenderer = objectTileRenderer;
    }
    public void Render(Camera camera)
    {
        //TODO Hirnschmalz reinstecken: Matrix korrigieren wegen objekt layer

        batch.Begin(transformMatrix: camera.ViewProjection, useScreenSpace: false);
        foreach(var e in layer.Entities)
        {
            var index = e.Id - objectTileRenderer.FirstId;
            batch.Draw(objectTileRenderer.Textures[index], new RectangleF(e.X, e.Y, e.Width, e.Height), Color.White);
        }
        batch.End();
    }
}
