using engenious;
using engenious.Graphics;
using engenious.Input;
using engenious.UI;
using HopeOfTheAncients.Tiled;
using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.IO;

namespace HopeOfTheAncients
{
    public sealed class GameControl : Control, IDisposable
    {
        private const int TileCount = 20;
        private readonly ChunkRenderer renderer;
        private RenderTarget2D? gameRenderTarget, hudRenderTarget;
        private readonly Camera camera;
        private readonly Camera pixelCamera;
        private readonly SpriteBatch spriteBatch;
        private readonly SpriteFont spriteFont;

        private readonly Entity entity;

        private readonly List<Entity> selectedEntitites;

        public GameControl(BaseScreenComponent manager, string style = "") : base(manager, style)
        {
            renderer = new ChunkRenderer(ScreenManager);
            camera = new Camera() { Position = Vector3.UnitZ };
            pixelCamera = new Camera() { Position = Vector3.UnitZ };
            spriteBatch = new SpriteBatch(manager.GraphicsDevice);
            selectedEntitites = new List<Entity>();
            spriteFont = manager.Content.Load<SpriteFont>("engenious.UI:///Fonts/GameFont") ?? throw new ArgumentException();

            var map = TileLoader.Load(new FileInfo(Path.Combine(".", "Assets", "map.tmx")));

            entity = new Entity(manager.GraphicsDevice);
        }

        protected override void OnUpdate(GameTime gameTime)
        {
            base.OnUpdate(gameTime);

            entity.Update((float)gameTime.ElapsedGameTime.TotalSeconds);

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
                dir += new Vector2(-1, 0);
            }
            if (keyBoardState.IsKeyDown(Keys.D))
            {
                dir += new Vector2(1, 0);
            }

            camera.Position += new Vector3(dir, 0);
            pixelCamera.Position += new Vector3(dir * ActualClientSize.Y / TileCount, 0);

            
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
        protected override void OnRightMouseUp(MouseEventArgs args)
        {
            base.OnRightMouseUp(args);

            var worldStart = ScreenToWorld(new Vector2(args.LocalPosition.X, ActualClientSize.Y - args.LocalPosition.Y));
            foreach (var selectedEntity in selectedEntitites)
            {
                selectedEntity.TargetPosition = worldStart;
            }
        }

        protected override void OnLeftMouseUp(MouseEventArgs args)
        {
            base.OnLeftMouseUp(args);

            if (!isMouseDown)
                return;
            selectedEntitites.Clear();

            var worldStart = ScreenToWorld(selectionStart);
            if (selectionStart == selectionEnd)
            {
                if (entity.Bounds.Contains(worldStart))
                {
                    selectedEntitites.Add(entity);
                }
            }
            else
            {
                var worldEnd = ScreenToWorld(selectionEnd);

                var worldRect = RectangleF.FromLTRB(Math.Min(worldStart.X, worldEnd.X), Math.Min(worldStart.Y, worldEnd.Y),
                                                Math.Max(worldStart.X, worldEnd.X), Math.Max(worldStart.Y, worldEnd.Y));

                if (worldRect.Contains(entity.Bounds))
                {
                    selectedEntitites.Add(entity);
                }
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

            var col = Color.Red;

            batch.Draw(Skin.Pix, rectangle, new Color(col, 0.5f));

            batch.Draw(Skin.Pix, new RectangleF(rectangle.X, rectangle.Y, rectangle.Width, 1), col);
            batch.Draw(Skin.Pix, new RectangleF(rectangle.X, rectangle.Y + rectangle.Height, rectangle.Width, 1), col);

            batch.Draw(Skin.Pix, new RectangleF(rectangle.X, rectangle.Y, 1, rectangle.Height), col);
            batch.Draw(Skin.Pix, new RectangleF(rectangle.X + rectangle.Width, rectangle.Y, 1, rectangle.Height), col);
        }



        protected override void OnPreDraw(GameTime gameTime)
        {
            base.OnPreDraw(gameTime);

            if (ActualClientSize.X == 0 || ActualClientSize.Y == 0)
                return;

            if (gameRenderTarget == null || hudRenderTarget == null
                || gameRenderTarget.Width != ActualClientSize.X
                || gameRenderTarget.Height != ActualClientSize.Y)
            {
                hudRenderTarget?.Dispose();
                gameRenderTarget?.Dispose();
                gameRenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ActualClientSize.X, ActualClientSize.Y, PixelInternalFormat.Rgba8);
                hudRenderTarget = new RenderTarget2D(ScreenManager.GraphicsDevice, ActualClientSize.X, ActualClientSize.Y, PixelInternalFormat.Rgba8);
                camera.UpdateBounds(ActualClientSize.X, ActualClientSize.Y, TileCount);
                pixelCamera.UpdateBounds(ActualClientSize.X, ActualClientSize.Y);
            }

            camera.Update();
            pixelCamera.Update();

            ScreenManager.GraphicsDevice.SetRenderTarget(gameRenderTarget);

            ScreenManager.GraphicsDevice.Clear(Color.CornflowerBlue);

            renderer.Render(camera);


            spriteBatch.Begin(transformMatrix: camera.ViewProjection, useScreenSpace: false, rasterizerState: RasterizerState.CullCounterClockwise);// (transformMatrix: );


            entity.Render(spriteBatch);


            spriteBatch.End();

            ScreenManager.GraphicsDevice.SetRenderTarget(hudRenderTarget);

            ScreenManager.GraphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(blendState: BlendState.Additive, transformMatrix: pixelCamera.ViewProjection, useScreenSpace: false, rasterizerState: RasterizerState.CullCounterClockwise);

            foreach(var selectedEntity in selectedEntitites)
            {

                string name = "Jack O'Neill";
                var offset = spriteFont.MeasureString(name);
                var pixelPos = selectedEntity.Bounds.Location * camera.Unit;
                pixelPos += new Vector2((selectedEntity.Bounds.Width * camera.Unit.X - offset.X) / 2, -offset.Y);
                spriteBatch.DrawString(spriteFont, name, pixelPos, Color.LightGray);
            }

            spriteBatch.End();
            spriteBatch.Begin(blendState: BlendState.Additive);


            DrawRectangle(spriteBatch, RectangleF.FromLTRB(Math.Min(selectionStart.X, selectionEnd.X), Math.Min(selectionStart.Y, selectionEnd.Y),
                                            Math.Max(selectionStart.X, selectionEnd.X), Math.Max(selectionStart.Y, selectionEnd.Y)));

            spriteBatch.End();

            ScreenManager.GraphicsDevice.SetRenderTarget(null!);
        }

        protected override void OnDrawContent(SpriteBatch batch, Rectangle contentArea, GameTime gameTime, float alpha)
        {
            base.OnDrawContent(batch, contentArea, gameTime, alpha);

            if (gameRenderTarget is null)
                return;

            batch.Draw(gameRenderTarget, contentArea, Color.White);

            if (hudRenderTarget is null)
                return;

            batch.Draw(hudRenderTarget, contentArea, Color.White);
        }

        public void Dispose()
        {
            gameRenderTarget?.Dispose();
            renderer?.Dispose();
            spriteBatch?.Dispose();
            spriteFont?.Dispose();
        }
    }
}
