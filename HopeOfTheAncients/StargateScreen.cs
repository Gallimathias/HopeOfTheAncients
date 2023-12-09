using engenious;
using engenious.Content.Serialization;
using engenious.Graphics;
using engenious.Helper;
using engenious.UI;
using engenious.UI.Controls;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HopeOfTheAncients;

public class StargateScreen : Screen, IDisposable
{
    private readonly StackPanel stackPanel;

    private readonly TextureAnimation eventHorizon;

    private readonly Texture2D baseGate, ring, chevronFrame, chevronLightOff, chevronLightOn, chevronArrowOff, chevronArrowOn;
    private Glyph destinationGlyph;

    private readonly SpriteFont glyphFont;

    private readonly StateMachine stateMachine;

    private bool IsDialing => currentGlyphIndex > 0;

    Glyph[] DialingAddress { get; } = new Glyph[9];

    float[] ChevronLightOpacities { get; } = new float[8];

    private int currentGlyphIndex, currentDialingGlyphIndex = -1;

    private bool isAddressLocked;
    private bool isShutdown;
    private bool isActive;

    private readonly List<TextButton> buttons;

    public void ResetAddress()
    {
        currentGlyphIndex = 0;
        isAddressLocked = false;
        currentDialingGlyphIndex = -1;
        isAddressLocked = false;

        for (int i = 0; i < ChevronLightOpacities.Length; i++)
        {
            ChevronLightOpacities[i] = 0;
        }

        lockChevronLightOpacity = 0;

        foreach (var button in buttons)
        {
            if (!button.Enabled)
                button.Enabled = true;
        }

    }

    public void AppendGlyph(Glyph glyph)
    {
        if (isAddressLocked)
            throw new Exception("Address already locked!");
        if (currentGlyphIndex > 8)
            throw new Exception("Address already full");
        DialingAddress[currentGlyphIndex++] = glyph;
    }

    public StargateScreen(BaseScreenComponent manager) : base(manager)
    {

        buttons = new List<TextButton>();
        Background = new SolidColorBrush(Color.DarkRed);
        glyphFont = manager.Content.Load<SpriteFont>("Fonts/milkyway-glyphs") ?? throw new ArgumentException();

        //var reader = new SpriteFontTypeReader();
        //reader.Read()

        eventHorizon = new TextureAnimation(manager.GraphicsDevice, "Assets/Stargate/event_horizon", 25f);

        baseGate = Texture2D.FromFile(manager.GraphicsDevice, "Assets/Stargate/gate.png");
        ring = Texture2D.FromFile(manager.GraphicsDevice, "Assets/Stargate/ring.png");
        chevronFrame = Texture2D.FromFile(manager.GraphicsDevice, "Assets/Stargate/chevron_7_frame.png");
        chevronLightOff = Texture2D.FromFile(manager.GraphicsDevice, "Assets/Stargate/chevron_7_light_off.png");
        chevronLightOn = Texture2D.FromFile(manager.GraphicsDevice, "Assets/Stargate/chevron_7_light_on.png");
        chevronArrowOff = Texture2D.FromFile(manager.GraphicsDevice, "Assets/Stargate/chevron_7_arrow_off.png");
        chevronArrowOn = Texture2D.FromFile(manager.GraphicsDevice, "Assets/Stargate/chevron_7_arrow_on.png");
        stackPanel = new StackPanel(manager)
        {
            HorizontalAlignment = HorizontalAlignment.Stretch,
            VerticalAlignment = VerticalAlignment.Stretch
        };

        var lockButton = new TextButton(manager, "Lock");
        var glyphStack =
            CreateGlyphButtons(manager, glyphFont!, (button, i) =>
            {
                AppendGlyph(KnownGlyphSets.Milkyway.Glyphs[i]);
                button.Enabled = false;
            });

        lockButton.LeftMouseClick +=(s,e) => LockButton_LeftMouseClick(lockButton, e);
        stackPanel.Controls.Add(lockButton);
        stackPanel.Controls.Add(glyphStack);

        Controls.Add(stackPanel);

        //MoveToGlyph(0, MovingMode.CounterClockwise);
        var startNode = new StateMachine.GenericNode((elapsedTime, totalTime) => true);
        stateMachine = new StateMachine(startNode);
        var dialGlyph = new StateMachine.GenericNode((e, t) =>
        {
            if (currentDialingGlyphIndex >= currentGlyphIndex - 1)
            {
                return false;
            }
            var glyph = DialingAddress[++currentDialingGlyphIndex];
            MoveToGlyph(glyph, MovingMode.Closest);
            return true;
        });
        var moveToGlyph = new StateMachine.GenericNode(UpdateMoveToGlyph);
        var lockChevron = new StateMachine.GenericNode(UpdateLockChevron);
        var lockChevronLightOff = new StateMachine.GenericNode(UpdateLockChevronLightOff);
        var kawoosh = new StateMachine.GenericNode((e, t) => true);
        var active = new StateMachine.GenericNode((e, t) =>
        {
            if (!isActive)
            {
                isActive = true;
            }

            return isShutdown;
        });
        var shutdown = new StateMachine.GenericNode((e, t) =>
        {
            if (isShutdown)
            {
                isShutdown = false;
                isActive = false;

                ResetAddress();
            }

            return !isShutdown;
        });
        stateMachine.AddNode(dialGlyph);
        stateMachine.AddNode(moveToGlyph);
        stateMachine.AddNode(lockChevron);
        stateMachine.AddNode(lockChevronLightOff);
        stateMachine.AddNode(kawoosh);
        stateMachine.AddNode(active);

        stateMachine.AddTransition(startNode, dialGlyph, () => IsDialing);
        stateMachine.AddTransition(dialGlyph, moveToGlyph, () => true);
        stateMachine.AddTransition(moveToGlyph, lockChevron, () => true);
        stateMachine.AddTransition(lockChevron, lockChevronLightOff, () => !IsLastChevron());
        stateMachine.AddTransition(lockChevron, kawoosh, IsLastChevron);
        stateMachine.AddTransition(kawoosh, active, () => isAddressLocked && !isShutdown);
        stateMachine.AddTransition(active, shutdown, () => isShutdown);
        stateMachine.AddTransition(shutdown, startNode, () => !isShutdown);
        stateMachine.AddTransition(lockChevronLightOff, dialGlyph, () => IsDialing);

    }

    private StackPanel CreateGlyphButtons(BaseScreenComponent baseScreenComponent, SpriteFont spriteFont, Action<TextButton, int> glypAction)
    {
        var panel = new StackPanel(baseScreenComponent)
        {
            Orientation = Orientation.Vertical
        };
        var upperPanel = new StackPanel(baseScreenComponent)
        {
            Orientation = Orientation.Horizontal
        };
        var lowerPanel = new StackPanel(baseScreenComponent)
        {
            Orientation = Orientation.Horizontal
        };

        panel.Controls.Add(upperPanel);
        panel.Controls.Add(lowerPanel);

        var glyphs = KnownGlyphSets.Milkyway.Glyphs;
        for (int i = 0; i < glyphs.Length; i++)
        {
            var glyphButton = new TextButton(baseScreenComponent, glyphs[i].Character.ToString())
            {
                Font = spriteFont,
                Tag = i,
                //MaxHeight = 30,
                //MaxWidth = 30,
                VerticalTextAlignment = VerticalAlignment.Center

            };

            glyphButton.LeftMouseClick += (s, e) => glypAction(glyphButton, (int)s.Tag!);

            if (i < 20)
            {
                upperPanel.Controls.Add(glyphButton);

            }
            else
            {
                lowerPanel.Controls.Add(glyphButton);
            }

            buttons.Add(glyphButton);
        }

        return panel;
    }

    private bool IsLastChevron() => isAddressLocked && currentDialingGlyphIndex + 1 == currentGlyphIndex;

    private void LockButton_LeftMouseClick(TextButton sender, MouseEventArgs args)
    {
        if (isActive)
        {
            isShutdown = true;
        }
        else
        {
            sender.Background = new SolidColorBrush(Color.OrangeRed);
            isAddressLocked = true;
        }
    }

    private bool UpdateLockChevronLightOff(float elapsedTime, float totalTime)
    {
        lockChevronLightOpacity = MathF.Max(MathF.Min(1f - (totalTime - 0.1f) * 10f, 1), 0);
        return totalTime >= 0.3f;
    }
    private bool UpdateLockChevron(float elapsedTime, float totalTime)
    {
        float elapsedAnimaton = (float)totalTime / 2f;
        bool isValidOrNotLast = true;
        if (elapsedAnimaton <= 1f)
        {
            currentChevronTime = ChevronCurve(elapsedAnimaton, isValidOrNotLast);
        }
        else
        {
            currentChevronTime = ChevronCurve(1f, isValidOrNotLast);
        }

        return elapsedAnimaton >= 1f;
    }

    private const float fullGateRotationTime = 6.5f;
    private bool UpdateMoveToGlyph(float elapsedTime, float totalTime)
    {
        if (currentRotation != destinationRotation)
        {
            bool isClockwise = rotationDirection > 0;
            var diff = (float)(rotationDirection * elapsedTime / fullGateRotationTime * Math.PI * 2);

            var restDifference = CalcDifference(currentRotation, destinationRotation);

            if (isClockwise)
            {
                if (diff > restDifference)
                    currentRotation = destinationRotation;
                else
                    currentRotation += diff;
            }
            else
            {
                if (diff < -restDifference)
                {
                    currentRotation = destinationRotation;
                }
                else
                    currentRotation += diff;
            }

            currentRotation = WrapAngle(currentRotation);
        }

        return currentRotation == destinationRotation;
    }

    private float currentRotation, destinationRotation, rotationDirection;

    private float lockChevronLightOpacity = 0;

    private float ChevronCurve(float t, bool doLightUp)
    {
        const float movingTime = 0.2f;
        const float movingBackTime = 0.85f;
        if (t < movingTime)
            return MathF.Sin((t * MathF.PI * (1 / movingTime) - MathF.PI / 2f)) / 2f + 0.5f;
        else if (t < movingBackTime)
        {
            lockChevronLightOpacity = doLightUp ? MathF.Min(1f, (t - movingTime) * 5 / (movingBackTime - movingTime)) : 0;
            if (currentDialingGlyphIndex < ChevronLightOpacities.Length && !IsLastChevron())
                ChevronLightOpacities[currentDialingGlyphIndex] = lockChevronLightOpacity;
            return 1f;
        }
        else
        {
            var tmp = 1f - (t - movingBackTime) / (1f - movingBackTime);
            return tmp * tmp;
        }
    }

    private float currentChevronTime;

    protected override void OnUpdate(GameTime gameTime)
    {
        base.OnUpdate(gameTime);
        stateMachine.Update((float)gameTime.ElapsedGameTime.TotalSeconds);
    }
    public enum MovingMode
    {
        Clockwise,
        CounterClockwise,
        Closest,
    }

    private static float CalcDifference(float currentRotation, float destinationRotation)
    {
        float diff = WrapAngle(destinationRotation - currentRotation);

        if (diff > MathF.PI)
        {
            return MathF.PI * 2 - diff;
        }
        return diff;
    }
    private static float WrapAngle(float angle)
    {
        while (angle < 0)
            angle += MathF.PI * 2;
        return angle;
    }

    public void MoveToGlyph(Glyph glyph, MovingMode mode)
    {
        destinationGlyph = glyph;
        const int glyphCount = 39;
        const float glyphAngle = 2 * MathF.PI / glyphCount;
        destinationRotation = WrapAngle(-((glyph.ScreenIndex - 1) * glyphAngle));
        rotationDirection = mode switch
        {
            MovingMode.Clockwise => 1f,
            MovingMode.CounterClockwise => -1f,
            MovingMode.Closest => WrapAngle(destinationRotation - currentRotation) > MathF.PI ? -1 : 1,
            _ => throw new ArgumentException(nameof(mode)),
        };
    }

    protected override void OnDraw(SpriteBatch batch, Rectangle controlArea, GameTime gameTime)
    {
        base.OnDraw(batch, controlArea, gameTime);


        for (int i = 0; i < currentGlyphIndex; i++)
        {
            batch.DrawString(glyphFont, DialingAddress[i].Character.ToString(), new Vector2(3 + i * 60, 15), i == currentDialingGlyphIndex ? Color.Blue : Color.White);
        }

        if (destinationGlyph != default)
        {
            batch.DrawString(Skin.Current.HeadlineFont, destinationGlyph.Name, new Vector2(3, 90), Color.White);
            batch.DrawString(glyphFont, destinationGlyph.Character.ToString(), new Vector2(3, 120), Color.White);
        }
        var maxSize = Math.Min(controlArea.Width, controlArea.Height);

        var pos = controlArea.Location.ToVector2() + (new Point(controlArea.Width - maxSize, controlArea.Height - maxSize)).ToVector2() / 2;

        var drawRectangle = new RectangleF(pos.X, pos.Y, maxSize, maxSize);

        if (isActive)
        {
            eventHorizon.Draw(batch, pos + new Vector2(maxSize / 4), new Vector2(maxSize / 2), (float)gameTime.ElapsedGameTime.TotalSeconds);
        }

        batch.Draw(baseGate, drawRectangle, Color.White);
        batch.Draw(ring, pos, null, Color.White, currentRotation, new Vector2(maxSize / 2f), new Vector2(maxSize), SpriteBatch.SpriteEffects.None, 0);

        DrawChevron(batch, maxSize, pos, 0 * MathF.PI / 180, lockChevronLightOpacity, currentChevronTime);
        DrawChevron(batch, maxSize, pos, 40 * MathF.PI / 180, ChevronLightOpacities[0]);
        DrawChevron(batch, maxSize, pos, 80 * MathF.PI / 180, ChevronLightOpacities[1]);
        DrawChevron(batch, maxSize, pos, 120 * MathF.PI / 180, ChevronLightOpacities[2]);
        DrawChevron(batch, maxSize, pos, 160 * MathF.PI / 180, ChevronLightOpacities[6]);
        DrawChevron(batch, maxSize, pos, 200 * MathF.PI / 180, ChevronLightOpacities[7]);
        DrawChevron(batch, maxSize, pos, 240 * MathF.PI / 180, ChevronLightOpacities[3]);
        DrawChevron(batch, maxSize, pos, 280 * MathF.PI / 180, ChevronLightOpacities[4]);
        DrawChevron(batch, maxSize, pos, 320 * MathF.PI / 180, ChevronLightOpacities[5]);


    }

    private void DrawChevron(SpriteBatch batch, int maxSize, Vector2 pos, float rotation, float lightOpacity, float currentChevronTime = 0f)
    {
        batch.Draw(chevronFrame, pos, null, Color.White, rotation, new Vector2(maxSize / 2f), new Vector2(maxSize), SpriteBatch.SpriteEffects.None, 0);
        batch.Draw(chevronArrowOff, pos + new Vector2(0, currentChevronTime * 12 * maxSize / baseGate.Width), null, Color.White, rotation, new Vector2(maxSize / 2f), new Vector2(maxSize), SpriteBatch.SpriteEffects.None, 0);
        batch.Draw(chevronArrowOn, pos + new Vector2(0, currentChevronTime * 12 * maxSize / baseGate.Width), null, new Color(1, 1, 1, lightOpacity), rotation, new Vector2(maxSize / 2f), new Vector2(maxSize), SpriteBatch.SpriteEffects.None, 0);

        batch.Draw(chevronLightOff, pos - new Vector2(0, currentChevronTime * 4 * maxSize / baseGate.Width), null, Color.White, rotation, new Vector2(maxSize / 2f), new Vector2(maxSize), SpriteBatch.SpriteEffects.None, 0);
        batch.Draw(chevronLightOn, pos - new Vector2(0, currentChevronTime * 4 * maxSize / baseGate.Width), null, new Color(1, 1, 1, lightOpacity), rotation, new Vector2(maxSize / 2f), new Vector2(maxSize), SpriteBatch.SpriteEffects.None, 0);
    }

    private Random rnd = new Random();
    protected override void OnLeftMouseClick(MouseEventArgs args)
    {
        //base.OnLeftMouseClick(args);
        //var index = rnd.Next(0, 39);
        //var glyph = KnownGlyphSets.Milkyway.Glyphs[index];

        //AppendGlyph(glyph);
    }

    public void Dispose()
    {
    }
}
