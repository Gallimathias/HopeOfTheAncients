using engenious;
using engenious.Graphics;
using engenious.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients
{
    public class ChunkRenderer : IDisposable
    {
        private readonly GraphicsDevice graphicsDevice;
        private readonly VertexBuffer vertexBuffer;
        private readonly IndexBuffer indexBuffer;
        private readonly Texture2D grass;
        private readonly engenious.UserDefined.Shaders.map mapEffect;
        

        public ChunkRenderer(BaseScreenComponent manager)
        {
            graphicsDevice = manager.GraphicsDevice;

            grass = Texture2D.FromFile(graphicsDevice, "Assets/grass.png");
            mapEffect = manager.Content.Load<engenious.UserDefined.Shaders.map>("Shaders/map");

            const int width = 100;
            const int height = 100;

            vertexBuffer = new VertexBuffer(graphicsDevice, VertexPositionTexture.VertexDeclaration, width * height * 4);
            indexBuffer = new IndexBuffer(graphicsDevice, DrawElementsType.UnsignedShort, width * height * 6);

            var vertices = new VertexPositionTexture[(int)vertexBuffer.VertexCount];
            var indices = new ushort[indexBuffer.IndexCount];
            int vIndex = 0, iIndex = 0;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    indices[iIndex++] = (ushort)(vIndex + 0);
                    indices[iIndex++] = (ushort)(vIndex + 1);
                    indices[iIndex++] = (ushort)(vIndex + 2);

                    indices[iIndex++] = (ushort)(vIndex + 1);
                    indices[iIndex++] = (ushort)(vIndex + 2);
                    indices[iIndex++] = (ushort)(vIndex + 3);


                    vertices[vIndex++] = new VertexPositionTexture(new Vector3(x + 0, y + 0), new Vector2(0, 0));
                    vertices[vIndex++] = new VertexPositionTexture(new Vector3(x + 1, y + 0), new Vector2(1, 0));
                    vertices[vIndex++] = new VertexPositionTexture(new Vector3(x + 0, y + 1), new Vector2(0, 1));
                    vertices[vIndex++] = new VertexPositionTexture(new Vector3(x + 1, y + 1), new Vector2(1, 1));
                }
            }
            vertexBuffer.SetData(vertices);
            indexBuffer.SetData(indices);
        }

        public void Dispose()
        {
            vertexBuffer.Dispose();
            indexBuffer.Dispose();
            grass?.Dispose();
            mapEffect?.Dispose();
        }

        public void Render(Camera camera)
        {
            foreach(var p in mapEffect.Ambient.Passes)
            {
                p.Apply();
                graphicsDevice.VertexBuffer = vertexBuffer;
                graphicsDevice.IndexBuffer = indexBuffer;

                mapEffect.Ambient.MainPass.WorldViewProj = camera.ViewProjection;
                mapEffect.Ambient.MainPass.grass = grass;

                graphicsDevice.DrawIndexedPrimitives(PrimitiveType.Triangles, 0, 0, (int)vertexBuffer.VertexCount, 0, indexBuffer.IndexCount / 3);

            }
        }
    }
}
