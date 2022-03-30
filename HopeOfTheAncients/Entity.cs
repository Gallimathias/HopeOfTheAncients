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

        public Vector2 Position { get; set; }

        public Vector2 TargetPosition { get; set; }

        public RectangleF Bounds => new(Position.X, Position.Y, 1, 2.5f);

        internal void Render(SpriteBatch spriteBatch)
        {
            var col = isSelected ? Color.Red : Color.White;
            spriteBatch.Draw(texture, Bounds, col);

        }

        public void Update(float elapsedTime)
        {
            var dir = TargetPosition - Position;
            if (Math.Abs(dir.X) < 0.001f && Math.Abs(dir.Y) < 0.001f)
            {
                Position = TargetPosition;
                return;
            }
            Position += dir.Normalized() * Math.Min(elapsedTime, dir.Length);
        }
    }
}