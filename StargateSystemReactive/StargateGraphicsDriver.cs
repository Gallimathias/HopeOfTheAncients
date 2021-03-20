using HopeOfTheAncients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace StargateSystemReactive
{
    public class StargateGraphicsDriver : IDisposable
    {
        public IObservable<DriverState> States => stateSubject;
        private readonly IDisposable commandSub;
        private readonly Subject<DriverState> stateSubject;

        private int lockedChevrons;
        private int currentChevrons;
        private readonly Glyph[] chevrons;
        private bool addressIsLocked;
        private StargateState.WormholeState wormhole;
        private bool isDialing;
        private bool isReady;

        private StargateState.DialingMode dialingMode;

        public StargateGraphicsDriver(IObservable<DriverCommand> commands)
        {
            commandSub = commands.Subscribe(HandleCommand);
            stateSubject = new Subject<DriverState>();
            chevrons = new Glyph[9];
        }

        private void HandleCommand(DriverCommand obj) => obj.Map(
                 startDialing => StartDialing(startDialing.DialingMode),
                 dialGlyph => DialGlyph(dialGlyph.CurrentChevron, dialGlyph.Glyph),
                 lockAddress => SetWormhole(StargateState.WormholeState.Activating),
                 activate => SetWormhole(StargateState.WormholeState.Deactivating),
                 deactivate => Cancel(),
                 lockAddress => LockAddress()
                );

        private void Cancel()
        {
            lockedChevrons = 0;
            addressIsLocked = false;
            isReady = false;
            isDialing = false;
        }

        private void SetWormhole(StargateState.WormholeState state)
            => wormhole = state;

        private void LockAddress()
            => addressIsLocked = true;

        private void DialGlyph(byte chevron, Glyph glyph)
        {
            currentChevrons = chevron + 1;
            chevrons[chevron] = glyph;
        }

        private void StartDialing(StargateState.DialingMode mode)
        {
            dialingMode = mode;
            isDialing = true;
        }

        private TimeSpan dialingAnimation;
        private TimeSpan lockAnimation;
        private TimeSpan activatingAnimation;
        private TimeSpan deactivatingAnimation;
        private TimeSpan activeTime;
        private int glyphIndex;


        public void Update(TimeSpan time)
        {
            Console.CursorVisible = false;
            Console.ForegroundColor = ConsoleColor.White;
            Write(0, 0, "Stargate Screen", ConsoleColor.Green);
            Write(0, 1, "================================", ConsoleColor.Green);

            var showChevs = isDialing || isReady || wormhole != StargateState.WormholeState.Off;
            if (showChevs)
            {
                for (int i = 0; i < lockedChevrons; i++)
                {
                    Write(0, 3 + i, $"Chevron {i + 1}: {chevrons[i].Name.PadRight(20, '.')}locked", ConsoleColor.Green);
                }
            }
            int startChev = showChevs ? lockedChevrons + 1 : 0;

            for (int i = startChev; i < chevrons.Length; i++)
            {
                Write(0, 3 + i, $"Chevron {i + 1}: <empty>", ConsoleColor.Red);
            }

            var (Left, Top) = Console.GetCursorPosition();
            ++Top;

            if (isDialing && wormhole == StargateState.WormholeState.Off)
            {
                Write(0, Top + 1, "Gate is Dialing: " + dialingMode, ConsoleColor.Gray);

                if (lockedChevrons < currentChevrons)
                {
                    if (glyphIndex != chevrons[lockedChevrons].ScreenIndex)
                    {
                        //PlayDialing animation
                        //Demo 5 seconds

                        if (dialingAnimation == TimeSpan.Zero)
                            dialingAnimation = time.Add(TimeSpan.FromSeconds(5));

                        if (time < dialingAnimation)
                        {
                            if (time.TotalMilliseconds % 4 > 0)
                            {
                                Write(0, Top + 2, "Dialing", ConsoleColor.Gray);
                            }
                            else
                            {
                                Write(0, Top + 2, "", ConsoleColor.Gray);
                            }
                        }
                        else
                        {
                            dialingAnimation = TimeSpan.Zero;
                            glyphIndex = chevrons[lockedChevrons].ScreenIndex;
                        }
                    }
                    else
                    {
                        //PlayLock animation
                        //Demo 3 seconds

                        if (lockAnimation == TimeSpan.Zero)
                            lockAnimation = time.Add(TimeSpan.FromSeconds(5));

                        if (time < lockAnimation)
                        {
                            var text = "Lock Chevron: " + (lockedChevrons + 1);
                            text = text.PadRight(30, '.') + $"{chevrons[lockedChevrons].Name} [{chevrons[lockedChevrons].Index}]";
                            Write(0, Top + 2, text, ConsoleColor.Red);
                        }
                        else
                        {
                            lockAnimation = TimeSpan.Zero;
                            ++lockedChevrons;
                        }
                    }
                }
                else if (addressIsLocked && lockedChevrons == currentChevrons)
                {
                    //last chevron, ready to activate
                    isDialing = false;
                    isReady = true;
                }
            }
            else
            {
                switch (wormhole)
                {
                    case StargateState.WormholeState.Active:
                        //play activ animation
                        Write(0, Top + 1, "Gate is Active", ConsoleColor.Green);
                        Write(0, Top + 2, $"Time: {activeTime:hh\\:mm\\:ss\\:fff}", ConsoleColor.Gray);
                        activeTime += time - activeTime;
                        break;
                    case StargateState.WormholeState.Activating:
                        //play activating animation
                        // DEMo 2 second
                        if (activatingAnimation == TimeSpan.Zero)
                        {
                            activatingAnimation = time.Add(TimeSpan.FromSeconds(2));
                            isReady = false;
                        }

                        if (time < activatingAnimation)
                        {
                            Write(0, Top + 1, "Kaaaaaaaaaaaaawoooooooooooooooosh", ConsoleColor.Blue);
                            Write(0, Top + 2, $"", ConsoleColor.Gray);
                        }
                        else
                        {
                            activatingAnimation = TimeSpan.Zero;
                            wormhole = StargateState.WormholeState.Active;
                        }
                        break;
                    case StargateState.WormholeState.Deactivating:
                        //play deactivating animation
                        if (deactivatingAnimation == TimeSpan.Zero)
                            lockAnimation = time.Add(TimeSpan.FromSeconds(1));

                        if (time < deactivatingAnimation)
                        {
                            Write(0, Top + 1, "swaaap", ConsoleColor.White);
                            Write(0, Top + 2, $"", ConsoleColor.Gray);
                        }
                        else
                        {
                            deactivatingAnimation = TimeSpan.Zero;
                            wormhole = StargateState.WormholeState.Off;
                        }
                        break;
                    case StargateState.WormholeState.Instable:
                        //play instable animation
                        break;
                    case StargateState.WormholeState.Off:
                        activeTime = TimeSpan.Zero;
                        Write(0, Top + 1, "Gate is Offline", ConsoleColor.Gray);
                        Write(0, Top + 2, $"", ConsoleColor.Gray);
                        break;
                }
            }

            stateSubject.OnNext(CreateState(wormhole, isReady, activeTime, lockedChevrons));
        }


        private static void Write(int x, int y, string text, ConsoleColor color)
        {
            var oldColor = Console.ForegroundColor;
            Console.SetCursorPosition(x, y);
            var pad = Console.WindowWidth - text.Length;
            Console.ForegroundColor = color;
            Console.Write(text.PadRight(pad));
            Console.ForegroundColor = oldColor;
        }

        private DriverState CreateState(StargateState.WormholeState wormhole, bool isReady, TimeSpan activeTime, int lockedChevrons)
            => new(wormhole, isReady, activeTime, lockedChevrons);

        public void Dispose()
        {
            commandSub.Dispose();
            stateSubject.Dispose();
        }
    }
}
