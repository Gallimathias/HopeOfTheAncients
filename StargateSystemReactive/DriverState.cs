using System;

namespace StargateSystemReactive
{
    public readonly struct DriverState
    {
        public readonly StargateState.WormholeState Wormhole { get; }
        public readonly bool IsReady { get; }
        public readonly TimeSpan ActiveTime { get; }
        public readonly int LockedChevrons { get; }


        public DriverState(StargateState.WormholeState wormhole = default, bool isReady = default, TimeSpan activeTime = default, int lockedChevrons = default)
        {
            Wormhole = wormhole;
            IsReady = isReady;
            ActiveTime = activeTime;
            LockedChevrons = lockedChevrons;
        }

        
    }
}