using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients.Tiled;
public class Entity
{
    public float X { get; }
    public float Y { get; }
    public float Width { get; }
    public float Height { get; }

    public int Id { get; }

    public Entity(int id, float x, float y, float width, float height)
    {
        Id = id;
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
}
