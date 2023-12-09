using engenious;
using engenious.Graphics;
using OpenTK.Graphics.ES11;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;

public class TextureAnimation : IDisposable
{
    public TextureAnimation(GraphicsDevice graphicsDevice, string directory, float fps)
    {
        var files = Directory.EnumerateFiles(directory, "*.png").OrderBy(x => x).ToList();
        Frames = new Texture2D[files.Count];
        for (int i=0;i<Frames.Length;i++)
        {
            Frames[i] = Texture2D.FromFile(graphicsDevice, files[i]);
        }

        MaxFrameTime = Frames.Length / fps;
    }
    public Texture2D[] Frames { get; }
    public float FrameTime { get; private set; }
    public float MaxFrameTime { get; set; }

    public void Draw(SpriteBatch batch, Vector2 pos, Vector2 maxSize, float elapsedTime)
    {
        FrameTime = (FrameTime + elapsedTime) % MaxFrameTime;

        float curFrameValue = (FrameTime * Frames.Length) / MaxFrameTime;

        var curFrame = (int)curFrameValue;
        var nextFrame = (curFrame + 1) % Frames.Length;
        var t = curFrameValue - curFrame;

        batch.Draw(Frames[curFrame], pos, null, Color.White, 0f, Vector2.Zero, maxSize, SpriteBatch.SpriteEffects.None, 0f);
        //batch.Draw(Frames[nextFrame], pos, null, new Color(1, 1, 1, t), 0f, Vector2.Zero, maxSize, SpriteBatch.SpriteEffects.None, 0.1f);

    }

    public void Dispose()
    {
        foreach(var f in Frames)
        {
            f.Dispose();
        }
    }
}
