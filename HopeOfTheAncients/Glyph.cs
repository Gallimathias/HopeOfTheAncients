using System;

namespace HopeOfTheAncients;

public readonly struct Glyph : IEquatable<Glyph>
{
    public readonly string Name { get;  }
    public readonly int ScreenIndex { get; }
    public readonly int Index { get; }
    public readonly char Character { get; }

    public Glyph(string name, int screenIndex, int index, char character)
    {
        Name = name;
        ScreenIndex = screenIndex;
        Index = index;
        Character = character;
    }

    public override bool Equals(object? obj) 
        => obj is Glyph glyph && Equals(glyph);

    public bool Equals(Glyph other) 
        => ScreenIndex == other.ScreenIndex 
        && Index == other.Index 
        && Character == other.Character;

    public override int GetHashCode() 
        => HashCode.Combine(ScreenIndex, Index, Character);

    public static bool operator ==(Glyph left, Glyph right) 
        => left.Equals(right);

    public static bool operator !=(Glyph left, Glyph right)
        => !(left == right);
}