using HopeOfTheAncients;
using System;
using System.Diagnostics;
using System.Reactive.Concurrency;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;

namespace StargateSystemReactive
{
    class Program
    {
        static void Main(string[] args)
        {
            using var commandSubj = new Subject<DriverCommand>();
            using var sgCommands = new Subject<StargateCommand>();
            using var driver = new StargateGraphicsDriver(commandSubj);
            IDisposable driverCommandSub = null;

            using var timeLoop = Observable
                .Using(
                () => new TimeWatch(new Stopwatch()),
                timeWatch =>
                    Observable
                        .Interval(TimeSpan.Zero)
                        .Select(i => timeWatch.ElapsedTime))
                .Subscribe(driver.Update);
            using var controller = StargateController
                .Create(sgCommands, driverCommands =>
                    {
                        driverCommandSub = driverCommands.Subscribe(commandSubj);
                        return driver.States;
                    })
                .Subscribe();

            using var dialSub = Observable
                .Timer(TimeSpan.FromSeconds(3))
                .Subscribe(i =>
                    {
                        var glyphs = KnownGlyphSets.Milkyway.Glyphs;
                        sgCommands.OnNext(new StargateCommand.StartDialing(StargateState.DialingMode.LeftAndRight));
                        sgCommands.OnNext(new StargateCommand.DialGlyph(glyphs[26]));
                        sgCommands.OnNext(new StargateCommand.DialGlyph(glyphs[6]));
                        sgCommands.OnNext(new StargateCommand.DialGlyph(glyphs[14]));
                        sgCommands.OnNext(new StargateCommand.DialGlyph(glyphs[31]));
                        sgCommands.OnNext(new StargateCommand.DialGlyph(glyphs[11]));
                        sgCommands.OnNext(new StargateCommand.DialGlyph(glyphs[29]));
                        sgCommands.OnNext(new StargateCommand.DialGlyph(glyphs[0]));
                        sgCommands.OnNext(new StargateCommand.LockAddress());
                    });

            using var reset = new ManualResetEvent(false);
            reset.WaitOne();

            driverCommandSub?.Dispose();
        }

        private readonly struct TimeWatch : IDisposable
        {
            public TimeSpan ElapsedTime => TimeSpan.FromTicks(stopwatch.ElapsedTicks);

            private readonly Stopwatch stopwatch;

            public TimeWatch(Stopwatch stopwatch)
            {
                this.stopwatch = stopwatch;
                stopwatch.Start();
            }

            public void Dispose()
                => stopwatch.Stop();
        }
    }
}
