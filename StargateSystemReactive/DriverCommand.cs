using HopeOfTheAncients;
using NonSucking.Framework.Extension.Rx.SumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StargateSystemReactive
{
    public class DriverCommand : Variant
        <
        DriverCommand.StartDialing,
        DriverCommand.DialGlyph,
        DriverCommand.ActivateWormhole,
        DriverCommand.DeactivateWormhole,
        DriverCommand.StopDialing,
        DriverCommand.Lock
        >
    {
        public DriverCommand(StartDialing value) : base(value)
        {

        }
        public DriverCommand(DialGlyph value) : base(value)
        {

        }
        public DriverCommand(ActivateWormhole value) : base(value)
        {

        }
        public DriverCommand(DeactivateWormhole value) : base(value)
        {

        }
        public DriverCommand(StopDialing value) : base(value)
        {

        }
        public DriverCommand(Lock value) : base(value)
        {

        }

        public readonly struct StartDialing 
        {
            public readonly StargateState.DialingMode DialingMode { get; }

            public StartDialing(StargateState.DialingMode dialingMode) 
                => DialingMode = dialingMode;
        }

        public readonly struct DialGlyph
        {
            public readonly Glyph Glyph { get; }
            public readonly byte CurrentChevron { get; }

            public DialGlyph(Glyph glyph, byte currentChevron)
            {
                Glyph = glyph;
                CurrentChevron = currentChevron;
            }
        }


        public struct ActivateWormhole 
        {
        }

        public struct DeactivateWormhole 
        {
        }

        public struct StopDialing 
        {
        }

        public struct Lock 
        {
        }

        public static implicit operator DriverCommand(StartDialing obj)
            => new(obj);
        public static implicit operator DriverCommand(DialGlyph obj)
            => new(obj);
        public static implicit operator DriverCommand(ActivateWormhole obj)
            => new(obj);
        public static implicit operator DriverCommand(DeactivateWormhole obj)
            => new(obj);
        public static implicit operator DriverCommand(StopDialing obj)
            => new(obj);
        public static implicit operator DriverCommand(Lock obj)
            => new(obj);

    }
}
