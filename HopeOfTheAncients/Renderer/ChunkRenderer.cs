using engenious;
using engenious.Graphics;
using engenious.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using static HopeOfTheAncients.Tiled.Map;

namespace HopeOfTheAncients.Renderer;

public class ChunkRenderer : IDisposable
{
    private readonly GraphicsDevice graphicsDevice;
    private readonly VertexBuffer vertexBuffer;
    private readonly IndexBuffer indexBuffer;
    private readonly Texture2DArray textures;
    private readonly engenious.UserDefined.Shaders.map mapEffect;


    public ChunkRenderer(BaseScreenComponent manager, TileLayer layer)
    {
        graphicsDevice = manager.GraphicsDevice;

        //grass = Texture2D.FromFile(graphicsDevice, "Assets/grass.png");
        mapEffect = manager.Content.Load<engenious.UserDefined.Shaders.map>("Shaders/map") ?? throw new ArgumentException();


        const int width = 100;
        const int height = 100;
        var vertexCount = width * height * 4;

        var vertices = new List<ChunkVertex>(vertexCount);
        var indices = new List<ushort>(width * height * 6);
        var vIndex = 0;
        for (var y = 0; y < height; y++)
        {
            for (var x = 0; x < width; x++)
            {
                var tile = layer[x, y];
                if (tile == 0)
                    continue;
                indices.Add((ushort)(vIndex + 0));
                indices.Add((ushort)(vIndex + 1));
                indices.Add((ushort)(vIndex + 2));

                indices.Add((ushort)(vIndex + 1));
                indices.Add((ushort)(vIndex + 3));
                indices.Add((ushort)(vIndex + 2));



                vertices.Add(new ChunkVertex(new Vector3(x + 0, y + 0), new Vector2(0, 0), (uint)(tile - 1)));
                vertices.Add(new ChunkVertex(new Vector3(x + 1, y + 0), new Vector2(1, 0), (uint)(tile - 1)));
                vertices.Add(new ChunkVertex(new Vector3(x + 0, y + 1), new Vector2(0, 1), (uint)(tile - 1)));
                vertices.Add(new ChunkVertex(new Vector3(x + 1, y + 1), new Vector2(1, 1), (uint)(tile - 1)));
                vIndex += 4;
            }
        }
        vertexBuffer = new VertexBuffer(graphicsDevice, ChunkVertex.VertexDeclaration, vertices.Count);
        indexBuffer = new IndexBuffer(graphicsDevice, DrawElementsType.UnsignedShort, indices.Count);

        vertexBuffer.SetData<ChunkVertex>(CollectionsMarshal.AsSpan(vertices));
        indexBuffer.SetData<ushort>(CollectionsMarshal.AsSpan(indices));
    }

    public void Dispose()
    {
        vertexBuffer.Dispose();
        indexBuffer.Dispose();
        //grass?.Dispose();
        mapEffect?.Dispose();
    }

    public void Render(Camera camera, UniformTileRenderer tileRenderer)
    {
        foreach (var p in mapEffect.Ambient.Passes)
        {
            p.Apply();
            graphicsDevice.RasterizerState = new RasterizerState() { FillMode = PolygonMode.Fill, CullMode = CullMode.CounterClockwise };
            graphicsDevice.VertexBuffer = vertexBuffer;
            graphicsDevice.IndexBuffer = indexBuffer;

            mapEffect.Ambient.MainPass.WorldViewProj = camera.ViewProjection * Matrix.CreateScaling(new Vector3(1));
            mapEffect.Ambient.MainPass.Textures = tileRenderer.Textures;

            graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, (int)vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);

        }
    }
}
