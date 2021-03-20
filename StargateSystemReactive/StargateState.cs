using HopeOfTheAncients;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StargateSystemReactive
{
    public readonly struct StargateState
    {
        public static StargateState Default { get; } = 
            new StargateState(OverallState.Idle, false, 0, new Chevron[9], DialingMode.DHD, WormholeState.Off); 

        public readonly OverallState State { get; }
        public readonly ReadOnlySpan<Chevron> Chevrons => chevrons;
        public readonly bool LockAddress { get;  }
        public readonly byte LockedChevrons { get; }
        public readonly DialingMode Dialing { get;  }
        public readonly WormholeState Wormhole { get;  }

        private readonly Chevron[] chevrons;

        public StargateState(OverallState state = default, bool lockAddress = default, byte lockedChevrons = 0, Chevron[] chevrons = null, DialingMode dialing = default, WormholeState wormhole = default)
        {
            State = state;
            LockAddress = lockAddress;
            LockedChevrons = lockedChevrons;
            this.chevrons = chevrons;
            Dialing = dialing;
            Wormhole = wormhole;
        }

        public enum OverallState
        {
            Idle,
            Dialing,
            Incomming,
            Active,
            ShutDown
        }

        public enum DialingMode
        {
            Left,
            Right,
            LeftAndRight,
            FastMode,
            DHD
        }

        public enum WormholeState
        {
            Off,
            Active,
            Activating,
            Deactivating,
            Instable
        }

        public readonly struct Chevron
        {
            public readonly bool Locked { get;  }
            public readonly Glyph Glyph { get;  }

            public Chevron(bool locked, Glyph glyph)
            {
                Locked = locked;
                Glyph = glyph;
            }
        }

        internal static StargateState Copy(StargateState currentState)
            => new(currentState.State, currentState.LockAddress, currentState.LockedChevrons, currentState.chevrons, currentState.Dialing, currentState.Wormhole);
        internal static StargateState With(StargateState currentState, OverallState overallState)
           => new(overallState, currentState.LockAddress, currentState.LockedChevrons, currentState.chevrons, currentState.Dialing, currentState.Wormhole);
        internal static StargateState With(StargateState currentState, OverallState overallState, DialingMode dialingMode)
            => new(overallState, currentState.LockAddress, currentState.LockedChevrons, currentState.chevrons, dialingMode, currentState.Wormhole);
        internal static StargateState With(StargateState currentState, Chevron[] chevrons, byte lockedChevrons)
            => new(currentState.State, currentState.LockAddress, lockedChevrons, chevrons, currentState.Dialing, currentState.Wormhole);
        internal static StargateState With(StargateState currentState, WormholeState wormholeState)
           => new(currentState.State, currentState.LockAddress, currentState.LockedChevrons, currentState.chevrons, currentState.Dialing, wormholeState);
        internal static StargateState WithLock(StargateState currentState)
            => new(currentState.State, true, currentState.LockedChevrons, currentState.chevrons, currentState.Dialing, currentState.Wormhole);
    }
}
