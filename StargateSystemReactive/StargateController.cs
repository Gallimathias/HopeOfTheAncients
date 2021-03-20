using HopeOfTheAncients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Text;
using System.Threading.Tasks;

namespace StargateSystemReactive
{
    public static class StargateController
    {
        public static IObservable<StargateState> Create(IObservable<StargateCommand> gateCommands, Func<IObservable<DriverCommand>, IObservable<DriverState>> driverFactory)
            => Observable.Using(
                    () =>
                    {
                        var subj = new Subject<DriverCommand>();
                        return new DriverHandler(subj, driverFactory(subj));
                    },
                    driverHandler => InternalCreate(gateCommands, driverHandler)
                )
                .DistinctUntilChanged();

        private static IObservable<StargateState> InternalCreate(IObservable<StargateCommand> gateCommands, DriverHandler driverHandler)
        {
            var statesFromCommands =
                gateCommands
                           .Scan(StargateState.Default, (currentState, command) => HandleCommand(currentState, command, driverHandler.Subject));

            return Observable
                .CombineLatest(statesFromCommands, driverHandler.DriverStates, (currentState, currentDriverState) => HandleDriverStates(currentState, currentDriverState, driverHandler.Subject))
                .Scan((currenState, newState) => HandleStateChange(currenState, newState, driverHandler.Subject));
        }

        private static StargateState HandleStateChange(StargateState currentState, StargateState newState, IObserver<DriverCommand> graphicDriver)
        {
            if (currentState.Equals(newState))
                return currentState;

            if (newState.Wormhole != currentState.Wormhole
                && currentState.Wormhole == StargateState.WormholeState.Deactivating
                && newState.Wormhole == StargateState.WormholeState.Off)
            {
                newState = default;
            }

            return newState;
        }

        private static StargateState HandleDriverStates(StargateState currentState, DriverState currentDriverState, IObserver<DriverCommand> graphicDriver)
        {
            if (currentDriverState.IsReady)
            {
                graphicDriver.OnNext(new DriverCommand.ActivateWormhole());
            }

            if(currentDriverState.ActiveTime > TimeSpan.FromMinutes(38) && currentDriverState.Wormhole == StargateState.WormholeState.Active)
            {
                graphicDriver.OnNext(new DriverCommand.DeactivateWormhole());
            }

            return StateWith(currentState, currentDriverState.Wormhole);
        }


        private static StargateState HandleCommand(StargateState currentState, StargateCommand command, IObserver<DriverCommand> graphicDriver)
            => command
                .Map(
                    startDialing => StartDialing(currentState, startDialing, graphicDriver),
                    dialGlyph => DialGlyph(currentState, dialGlyph, graphicDriver),
                    incomming => Incomming(currentState, incomming, graphicDriver),
                    shutDown => ShutDown(currentState, shutDown, graphicDriver),
                    lockAddress => LockAddress(currentState, lockAddress, graphicDriver)
                );

        private static StargateState LockAddress(StargateState currentState, StargateCommand.LockAddress command, IObserver<DriverCommand> graphicDriver)
        {
            if (currentState.State != StargateState.OverallState.Dialing)
                return currentState;

            graphicDriver.OnNext(new DriverCommand.Lock());
            return StateWithLock(currentState);
        }

        private static StargateState ShutDown(StargateState currentState, StargateCommand.ShutDown command, IObserver<DriverCommand> graphicDriver)
        {
            if (currentState.State != StargateState.OverallState.Dialing
                && currentState.State != StargateState.OverallState.Active
                && currentState.State != StargateState.OverallState.Incomming)
                return currentState;

            if (currentState.State == StargateState.OverallState.Active)
                graphicDriver.OnNext(new DriverCommand.DeactivateWormhole());
            else
                graphicDriver.OnNext(new DriverCommand.StopDialing());

            return StateWith(currentState, StargateState.OverallState.ShutDown);
        }

        private static StargateState Incomming(StargateState currentState, StargateCommand.Incomming command, IObserver<DriverCommand> graphicDriver)
        {
            if (currentState.State != StargateState.OverallState.Dialing
                && currentState.State != StargateState.OverallState.Idle)
                return currentState;

            graphicDriver.OnNext(new DriverCommand.ActivateWormhole());
            return StateWith(currentState, StargateState.OverallState.Incomming);
        }

        private static StargateState DialGlyph(StargateState currentState, StargateCommand.DialGlyph command, IObserver<DriverCommand> graphicDriver)
        {
            if (currentState.State != StargateState.OverallState.Dialing || currentState.LockAddress)
                return currentState;

            var state = StateWithNextGlyph(currentState, command.Glyph);
            var chevron = state.LockedChevrons;
            --chevron;
            graphicDriver.OnNext(new DriverCommand.DialGlyph(command.Glyph, chevron));
            return state;
        }


        private static StargateState StartDialing(StargateState currentState, StargateCommand.StartDialing command, IObserver<DriverCommand> graphicDriver)
        {
            if (currentState.State == StargateState.OverallState.Idle)
            {
                graphicDriver.OnNext(new DriverCommand.StartDialing(command.DialingMode));
                return StateWith(currentState, StargateState.OverallState.Dialing, command.DialingMode);
            }
            else
            {
                return currentState;
            }
        }

        private static StargateState StateWith(StargateState currentState, StargateState.OverallState overallState, StargateState.DialingMode dialingMode)
            => StargateState.With(currentState, overallState, dialingMode);

        private static StargateState StateWith(StargateState currentState, StargateState.WormholeState wormhole)
            => StargateState.With(currentState, wormhole);

        private static StargateState StateWith(StargateState currentState, StargateState.OverallState overallState)
            => StargateState.With(currentState, overallState);

        private static StargateState StateWithNextGlyph(StargateState currentState, Glyph glyph)
        {
            var chevrons = currentState.Chevrons.ToArray();
            var nextChevron = currentState.LockedChevrons;
            chevrons[nextChevron] = new StargateState.Chevron(false, glyph);
            ++nextChevron;
            return StargateState.With(currentState, chevrons, nextChevron);
        }

        private static StargateState StateWithLock(StargateState currentState)
            => StargateState.WithLock(currentState);

        private readonly struct DriverHandler : IDisposable
        {
            public readonly Subject<DriverCommand> Subject { get; }
            public readonly IObservable<DriverState> DriverStates { get; }

            public DriverHandler(Subject<DriverCommand> subject, IObservable<DriverState> driverStates)
            {
                Subject = subject;
                DriverStates = driverStates;
            }

            public override bool Equals(object obj) => obj is DriverHandler other && EqualityComparer<Subject<DriverCommand>>.Default.Equals(Subject, other.Subject) && EqualityComparer<IObservable<DriverState>>.Default.Equals(DriverStates, other.DriverStates);
            public override int GetHashCode() => HashCode.Combine(Subject, DriverStates);

            public void Deconstruct(out Subject<DriverCommand> subject, out IObservable<DriverState> driverStates)
            {
                subject = Subject;
                driverStates = DriverStates;
            }

            public void Dispose()
                => Subject.Dispose();

            public static implicit operator (Subject<DriverCommand> Subject, IObservable<DriverState> DriverStates)(DriverHandler value)
                => (value.Subject, value.DriverStates);
            public static implicit operator DriverHandler((Subject<DriverCommand> Subject, IObservable<DriverState> DriverStates) value)
                => new DriverHandler(value.Subject, value.DriverStates);
        }
    }
}


