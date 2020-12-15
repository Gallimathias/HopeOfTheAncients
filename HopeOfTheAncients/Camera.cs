using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients
{
    public class Camera
    {
        public Matrix Projection { get; private set; }
        public Matrix View { get; private set; }

        public Matrix ViewProjection { get; private set; }

        public void UpdateBounds(int width, int height)
        {
            const int tileCount = 20;

            float aspectRatio = (float)height / width;

            Projection = Matrix.CreateOrthographicOffCenter(0, tileCount / aspectRatio, tileCount, 0, -10, 10);
        }
        public void Update()
        {
            ViewProjection = Projection;
        }
    }
}
