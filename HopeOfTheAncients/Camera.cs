using engenious;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients
{
    class Camera
    {
        public Matrix Projection { get; private set; }
        public Matrix View { get; private set; }

        public Matrix ViewProjection { get; private set; }

        public void UpdateBounds(int width, int height)
        {

            Projection = Matrix.CreateOrthographicOffCenter(0, width, height, 0, -10, 10);
        }
        public void Update()
        {
            ViewProjection = Projection;
        }
    }
}
