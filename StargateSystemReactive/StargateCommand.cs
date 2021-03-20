using HopeOfTheAncients;
using NonSucking.Framework.Extension.Rx.SumTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StargateSystemReactive
{
    public class StargateCommand : Variant
        <
        StargateCommand.StartDialing,
        StargateCommand.DialGlyph,
        StargateCommand.Incomming,
        StargateCommand.ShutDown,
        StargateCommand.LockAddress
        >
    {

        public StargateCommand(StartDialing value) : base(value)
        {

        }
        public StargateCommand(DialGlyph value) : base(value)
        {

        }
        public StargateCommand(Incomming value) : base(value)
        {

        }
        public StargateCommand(ShutDown value) : base(value)
        {

        }
        public StargateCommand(LockAddress value) : base(value)
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

            public DialGlyph(Glyph glyph)
                => Glyph = glyph;
        }
        public struct Incomming { }
        public struct ShutDown { }
        public struct LockAddress { }

        public static implicit operator StargateCommand(StartDialing obj)
            => new(obj);

        public static implicit operator StargateCommand(DialGlyph obj)
            => new(obj);

        public static implicit operator StargateCommand(Incomming obj)
            => new(obj);

        public static implicit operator StargateCommand(ShutDown obj)
            => new(obj);

        public static implicit operator StargateCommand(LockAddress obj)
            => new(obj);
    }
}
