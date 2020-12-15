using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients
{
    public sealed class GameControl : Control, IDisposable
    {
        private readonly ChunkRenderer renderer;
        private RenderTarget2D? renderTarget;
        private readonly Camera camera;

        public GameControl(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            renderer = new ChunkRenderer(ScreenManager);
            camera = new Camera() { Position = Vector3.UnitZ };
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            var keyBoardState = Keyboard.GetState();

            var dir = new Vector2(0, 0);
            if (keyBoardState.IsKeyDown(Keys.W))
            {
                dir += new Vector2(0, -1);
            }
            if (keyBoardState.IsKeyDown(Keys.S))
            {
                dir += new Vector2(0, 1);
            }
            if (keyBoardState.IsKeyDown(Keys.A))
            {
                dir += new Vector2( -1,0);
            }
            if (keyBoardState.IsKeyDown(Keys.D))
            {
                dir += new Vector2(1,0);
            }

            camera.Position += new Vector3(dir, 0);
        }

        protected override void OnPreDraw(GameTime gameTime)
        {
            base.OnPreDraw(gameTime);

            if (ActualClientSize.X == 0 || ActualClientSize.Y == 0)
                return;

            if (renderTarget == null
                || renderTarget.Width != ActualClientSize.X
                || renderTarget.Height != ActualClientSize.Y)
            {
                renderTarget?.Dispose();
                renderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ActualClientSize.X, ActualClientSize.Y, PixelInternalFormat.Rgba8);
                camera.UpdateBounds(ActualClientSize.X, ActualClientSize.Y);
            }

            camera.Update();

            ScreenManager.GraphicsDevice.SetRenderTarget(renderTarget);

            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            renderer.Render(camera);

            ScreenManager.GraphicsDevice.SetRenderTarget(null);


        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            base.OnDrawContent(batch, contentArea, gameTime, alpha);

            if (renderTarget == null)
                return;

            batch.Draw(renderTarget, contentArea, Color.White);
        }

        public void Dispose()
        {
            renderTarget?.Dispose();
            renderer?.Dispose();
        }
    }
}
