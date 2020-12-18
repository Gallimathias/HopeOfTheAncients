using engenious;
using engenious.Graphics;
using System;

namespace HopeOfTheAncients
{
    internal class Entity
    {
        private GraphicsDevice graphicsDevice;
        private readonly Texture2D texture;


        private bool isSelected;

        public Entity(GraphicsDevice graphicsDevice)
        {
            this.graphicsDevice = graphicsDevice;
            texture = Texture2D.FromFile(graphicsDevice, "Assets/small_jack.png");
        }

        public void CollisionCheck(bool p)
        {
            isSelected = p;
        }

        public static RectangleF Bounds => new(0, 0, 1, 2);

        internal void Render(SpriteBatch spriteBatch)
        {
            var col = isSelected ? Color.Red : Color.White;
            spriteBatch.Draw(texture, new RectangleF(0,0,1,2.5f), col);
        }
    }
}