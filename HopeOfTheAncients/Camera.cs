using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;

public class Camera
{
    public Matrix Projection { get; private set; }
    public Matrix View { get; private set; }

    public Matrix ViewProjection { get; private set; }


    public Vector3 Position { get; set; }

    public Vector2 Unit { get; private set; }

    public void UpdateBounds(int width, int height, int tileCount)
    {
        float aspectRatio = (float)height / width;

        Unit = new Vector2(height / tileCount, height / tileCount);
        Projection = Matrix.CreateOrthographicOffCenter(0, tileCount / aspectRatio, 0, tileCount, -10, 10);
    }

    public void UpdateBounds(int width, int height)
    {
        Unit = Vector2.One;
        Projection = Matrix.CreateOrthographicOffCenter(0, width, 0, height, -10, 10);
    }

    public void Update()
    {
        View = Matrix.CreateLookAt(Position, Position + new Vector3(0, 0, -1), Vector3.UnitY);
        ViewProjection = Projection * View;
    }
}
