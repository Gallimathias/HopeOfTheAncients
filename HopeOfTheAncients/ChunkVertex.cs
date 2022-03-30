using engenious;
using engenious.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;
[StructLayout(LayoutKind.Sequential, Pack = 1)]
internal struct ChunkVertex : IVertexType
{
    public static VertexDeclaration VertexDeclaration;
    static ChunkVertex()
    {
        VertexDeclaration = new VertexDeclaration(sizeof(float) * 5 + sizeof(uint),new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
            new VertexElement(sizeof(float) * 3, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(sizeof(float) * 5, VertexElementFormat.Single, VertexElementUsage.Normal, 0));
    }
    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    public ChunkVertex(Vector3 position, Vector2 textureCoordinate, uint texture)
    {
        Position = position;
        TextureCoordinate = textureCoordinate;
        Texture = texture;
    }
    public Vector3 Position { get; }
    public Vector2 TextureCoordinate { get; }
    public uint Texture { get; }
}
