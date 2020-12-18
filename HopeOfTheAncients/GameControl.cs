using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using System;

namespace HopeOfTheAncients
{
    public sealed class GameControl : Control, IDisposable
    {
        private readonly ChunkRenderer renderer;
        private RenderTarget2D? renderTarget;
        private readonly Camera camera;
        private readonly SpriteBatch spriteBatch;

        private readonly Entity entity;

        public GameControl(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            renderer = new ChunkRenderer(ScreenManager);
            camera = new Camera() { Position = Vector3.UnitZ };
            spriteBatch = new SpriteBatch(manager.GraphicsDevice);

            entity = new Entity(manager.GraphicsDevice);
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

        private Vector2 ScreenToWorld(Vector2 screen)
        {
            var pos = new Vector3(screen.X, screen.Y, 0);

            var viewport = new Viewport(ActualClientArea);

            pos = viewport.Unproject(pos, camera.ViewProjection);

            return new Vector2(pos.X, pos.Y);
        }

        private bool isMouseDown;
        private Vector2 selectionStart, selectionEnd;
        protected override void OnLeftMouseDown(MouseEventArgs args)
        {
            base.OnLeftMouseDown(args);

            var pos = new Vector2(args.LocalPosition.X, ActualClientSize.Y - args.LocalPosition.Y);

            selectionStart = selectionEnd = pos;
            isMouseDown = true;
        }
        protected override void OnLeftMouseUp(MouseEventArgs args)
        {
            base.OnLeftMouseUp(args);

            if (!isMouseDown)
                return;

            var worldStart = ScreenToWorld(selectionStart);
            var worldEnd = ScreenToWorld(selectionEnd);

            var worldRect = RectangleF.FromLTRB(Math.Min(worldStart.X, worldEnd.X), Math.Min(worldStart.Y, worldEnd.Y),
                                            Math.Max(worldStart.X, worldEnd.X), Math.Max(worldStart.Y, worldEnd.Y));

            entity.CollisionCheck(false);
            if (worldRect.Contains(Entity.Bounds))
            {
                entity.CollisionCheck(true);
            }

            isMouseDown = false;



            selectionStart = selectionEnd = Vector2.Zero;
        }

        protected override void OnMouseMove(MouseEventArgs args)
        {
            base.OnMouseMove(args);

            if (isMouseDown)
            {
                selectionEnd = new Vector2(args.LocalPosition.X, ActualClientSize.Y - args.LocalPosition.Y);
            }
        }

        private void DrawRectangle(SpriteBatch batch, RectangleF rectangle)
        {
            if (rectangle.Width == 0)
                return;
            batch.Draw(Skin.Pix, rectangle, new Color(Color.Red, 0.2f));
            var col = Color.Red;
            batch.Draw(Skin.Pix, new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, 1), col);
            batch.Draw(Skin.Pix, new RectangleF(rectangle.X , rectangle.Y + rectangle.Height, rectangle.Width, 1), col);


            batch.Draw(Skin.Pix, new RectangleF(rectangle.X, rectangle.Y, 1, rectangle.Height), col);
            batch.Draw(Skin.Pix, new RectangleF(rectangle.X + rectangle.Width, rectangle.Y, 1, rectangle.Height), col);
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


            spriteBatch.Begin(transformMatrix: camera.ViewProjection, useScreenSpace: false, rasterizerState: RasterizerState.CullCounterClockwise);// (transformMatrix: );


            entity.Render(spriteBatch);


            spriteBatch.End();

            spriteBatch.Begin();


            DrawRectangle(spriteBatch, RectangleF.FromLTRB(Math.Min(selectionStart.X, selectionEnd.X), Math.Min(selectionStart.Y, selectionEnd.Y),
                                            Math.Max(selectionStart.X, selectionEnd.X), Math.Max(selectionStart.Y, selectionEnd.Y)));

            spriteBatch.End();

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
            spriteBatch?.Dispose();
        }
    }
}
